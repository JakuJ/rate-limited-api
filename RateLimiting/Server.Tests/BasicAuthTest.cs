using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Server.Tests.Helpers;
using Server.Tests.Helpers.Fixtures;
using Xunit;

namespace Server.Tests
{
    public class BasicAuthTest : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<RandomTestFixture>
    {
        private readonly WebApplicationFactory<Startup> factory;
        private readonly RandomTestFixture fixture;

        public BasicAuthTest(WebApplicationFactory<Startup> factory, RandomTestFixture fixture)
        {
            this.factory = factory;
            this.fixture = fixture;
        }

        private static async Task<HttpResponseMessage> Random(HttpClient client) => await client.GetAsync("/random");

        private async Task<HttpClient> NewContext() => await RandomHelper.NewContext(fixture, factory);

        [Fact]
        public async Task RejectsRequestsWithNoBasicAuthHeader()
        {
            // Arrange
            HttpClient client = await NewContext();
            client.DefaultRequestHeaders.Authorization = null;

            // Act
            HttpResponseMessage response = await Random(client);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RejectsRequestsWithNoClientIdHeader()
        {
            // Arrange
            HttpClient client = await NewContext();
            client.DefaultRequestHeaders.Remove("X-Client-ID");

            // Act
            HttpResponseMessage response = await Random(client);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("213769")] // no a registered user
        [InlineData("what")] // not even a valid ID
        public async Task RejectsRequestsWithInvalidClientIdHeader(string clientID)
        {
            // Arrange
            HttpClient client = await NewContext();
            client.DefaultRequestHeaders.Remove("X-Client-ID");
            client.DefaultRequestHeaders.Add("X-Client-ID", clientID); // probably not a registered user

            // Act
            HttpResponseMessage response = await Random(client);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RejectsRequestsWithBadPassword()
        {
            // Arrange
            HttpClient client = await NewContext();
            client.DefaultRequestHeaders.Remove("X-Client-ID");
            client.DefaultRequestHeaders.Add("X-Client-ID", "1");

            byte[] plainTextBytes = Encoding.UTF8.GetBytes("username_1:badpassword");
            string base64 = Convert.ToBase64String(plainTextBytes);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);

            // Act
            HttpResponseMessage response = await Random(client);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RejectsRequestsWithBadUsername()
        {
            // Arrange
            HttpClient client = await NewContext();
            client.DefaultRequestHeaders.Remove("X-Client-ID");

            byte[] plainTextBytes = Encoding.UTF8.GetBytes("username_unknown:testpassword");
            string base64 = Convert.ToBase64String(plainTextBytes);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);

            // Act
            HttpResponseMessage response = await Random(client);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
