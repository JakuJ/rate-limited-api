using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Server.Models.Register;
using Server.Tests.Helpers.Fixtures;
using Xunit;

namespace Server.Tests.Helpers
{
    public static class RandomHelper
    {
        public static async Task<HttpClient> NewContext(
            RandomTestFixture fixture,
            WebApplicationFactory<Startup> factory)
        {
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
    }
}
