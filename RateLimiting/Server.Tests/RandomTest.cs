using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FsCheck.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Common.RateLimiting;
using Server.Models.Random;
using Server.Tests.Generators;
using Xunit;

namespace Server.Tests
{
    [Collection("/random")]
    public class RandomTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;
        private int defaultSize, defaultWindow, defaultLimit;

        public RandomTest(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            (defaultLimit, defaultWindow) = factory.Services.GetService<IRateLimiter>()!.GetUserLimit(1);
            defaultSize =
                int.Parse(factory.Services.GetService<IConfiguration>()!.GetSection("RandomConfig")["DefaultSize"]);
        }

        private async Task<HttpClient> NewContext(string username)
        {
            HttpClient client = factory.CreateClient();
            HttpResponseMessage response = await Helpers.Register(username, "testpassword", client, factory);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get client ID
            var body = await response.Content.ReadFromJsonAsync<Models.Register.ResponseBody>();
            int clientId = body.Id;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{username}:testpassword");
            string base64 = Convert.ToBase64String(plainTextBytes);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
            client.DefaultRequestHeaders.Add("X-Client-ID", clientId.ToString());

            return client;
        }

        private static async Task<HttpResponseMessage> Random(HttpClient client, int? bytes = null)
        {
            if (bytes != null)
            {
                return await client.GetAsync($"/random?len={bytes}");
            }

            return await client.GetAsync("/random");
        }

        [Fact]
        public async Task Returns32BytesByDefault()
        {
            // Arrange
            HttpClient client = await NewContext("randomusername");

            // Act
            HttpResponseMessage response = await Random(client);
            ResponseBody body = await response.Content.ReadFromJsonAsync<ResponseBody>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(defaultSize, Convert.FromBase64String(body!.random).Length);
            Assert.Equal((defaultLimit - defaultSize).ToString(), response.Headers.GetValues("X-Rate-Limit").Single());
        }

        [Property(Arbitrary = new[] {typeof(ValidLen), typeof(ValidUsername)})]
        public void RespectsTheQueryParameter(string username, int len)
        {
            // Arrange
            HttpClient client = NewContext(username).Result;

            // Act
            HttpResponseMessage response = Random(client, len).Result;
            ResponseBody body = response.Content.ReadFromJsonAsync<ResponseBody>().Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(len, Convert.FromBase64String(body!.random).Length);
            Assert.Equal((1024 - len).ToString(), response.Headers.GetValues("X-Rate-Limit").Single());
        }

        [Property(Arbitrary = new[] {typeof(InvalidLen), typeof(ValidUsername)})]
        public void RefusesToProcessInvalidLengths(string username, int len)
        {
            // Arrange
            HttpClient client = NewContext(username).Result;

            // Act
            HttpResponseMessage response = Random(client, len).Result;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Property(Arbitrary = new[] {typeof(ValidUsername), typeof(ValidLen)}, Timeout = 10_000, MaxTest = 20)]
        public void DecrementsRateLimitProperly(string username, int len)
        {
            // Arrange
            HttpClient client = NewContext(username).Result;

            // Act & Assert
            int used = defaultLimit;
            while (true)
            {
                HttpResponseMessage response = Random(client, len).Result;
                if (response.IsSuccessStatusCode)
                {
                    used -= len;
                    var remaining = int.Parse(response.Headers.GetValues("X-Rate-Limit").Single());
                    Assert.Equal(used, remaining);
                }
                else
                {
                    break;
                }
            }
        }
    }
}