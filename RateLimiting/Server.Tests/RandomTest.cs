using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FsCheck.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Common;
using Server.Common.RateLimiting;
using Server.Models.Register;
using Server.Tests.Generators;
using Server.Tests.Helpers;
using Server.Tests.Helpers.Fixtures;
using Xunit;

namespace Server.Tests
{
    [Collection("/random")]
    public class RandomTest : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<RandomTestFixture>
    {
        private readonly WebApplicationFactory<Startup> factory;
        private readonly RandomTestFixture fixture;

        public RandomTest(WebApplicationFactory<Startup> factory, RandomTestFixture fixture)
        {
            this.factory = factory;
            this.fixture = fixture;
        }

        private IRandomLimitConfig LimitConfig => factory.Services.GetService<IRandomLimitConfig>()!;

        private static async Task<HttpResponseMessage> Random(HttpClient client, int? bytes = null)
        {
            if (bytes != null)
            {
                return await client.GetAsync($"/random?len={bytes}");
            }

            return await client.GetAsync("/random");
        }

        private async Task<HttpClient> NewContext()
        {
            // Generating unique usernames sure is hard
            var username = $"username_{fixture.UniqueId}";

            HttpClient client = factory.CreateClient();
            HttpResponseMessage response = await RegisterHelper.Register(username, "testpassword", client, factory);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get client ID
            ResponseBody body = await response.Content.ReadFromJsonAsync<ResponseBody>();
            int clientId = body!.Id;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{username}:testpassword");
            string base64 = Convert.ToBase64String(plainTextBytes);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);
            client.DefaultRequestHeaders.Add("X-Client-ID", clientId.ToString());

            return client;
        }

        [Fact]
        public async Task ReturnsConfiguredNumberOfBytesByDefault()
        {
            // Arrange
            HttpClient client = await NewContext();

            // Act
            HttpResponseMessage response = await Random(client);
            Models.Random.ResponseBody body = await response.Content.ReadFromJsonAsync<Models.Random.ResponseBody>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(LimitConfig.DefaultSize, Convert.FromBase64String(body!.Random).Length);
            Assert.Equal(
                (LimitConfig.DefaultLimit - LimitConfig.DefaultSize).ToString(),
                response.Headers.GetValues("X-Rate-Limit").Single());
        }

        [Property(Arbitrary = new[] { typeof(ValidLen) })]
        public void RespectsTheQueryParameter(int len)
        {
            // Arrange
            HttpClient client = NewContext().Result;

            // Act
            HttpResponseMessage response = Random(client, len).Result;
            Models.Random.ResponseBody body = response.Content.ReadFromJsonAsync<Models.Random.ResponseBody>().Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(len, Convert.FromBase64String(body!.Random).Length);
            Assert.Equal((1024 - len).ToString(), response.Headers.GetValues("X-Rate-Limit").Single());
        }

        [Property(Arbitrary = new[] { typeof(InvalidLen) })]
        public void RefusesToProcessInvalidLengths(int len)
        {
            // Arrange
            HttpClient client = NewContext().Result;

            // Act
            HttpResponseMessage response = Random(client, len).Result;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void LimitsRateForConcurrentUsers()
        {
            // Arrange
            IEnumerable<HttpClient> clients = Enumerable.Range(1, 6).Select(_ => NewContext().Result);
            int[] lens = { 32, 64, 100, 200, 500, 1024 };

            Dictionary<int, List<DateTime>> responses = new();
            foreach (int len in lens)
            {
                responses[len] = new List<DateTime>();
            }

            async Task DoWork(HttpClient client, int len)
            {
                DateTime start = DateTime.Now;
                while (true)
                {
                    HttpResponseMessage response = await Random(client, len);
                    if (response.IsSuccessStatusCode)
                    {
                        responses[len].Add(DateTime.Now);
                    }

                    if (DateTime.Now - start > TimeSpan.FromSeconds(30))
                    {
                        break;
                    }
                }
            }

            // Act
            Task[] tasks
                = clients
                    .Zip(lens)
                    .Select(pair => DoWork(pair.First, pair.Second))
                    .ToArray();
            Task.WaitAll(tasks);

            // Assert
            double rechargeRate = LimitConfig.DefaultLimit / (double)LimitConfig.DefaultWindow;
            const double marginOfError = 20.0;

            foreach ((int len, List<DateTime> times) in responses)
            {
                Assert.NotEmpty(times);
                DateTime start = times.First();

                foreach ((int ix, TimeSpan t) in times.Select(t => t - start).Enumerate())
                {
                    double maxExpected = LimitConfig.DefaultLimit + t.TotalSeconds * rechargeRate;
                    Assert.InRange(len * (ix + 1), 0, maxExpected + marginOfError);
                }
            }
        }
    }
}
