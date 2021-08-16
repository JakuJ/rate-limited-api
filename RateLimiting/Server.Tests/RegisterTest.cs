using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using FsCheck.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Server.Models.Register;
using Server.Tests.Generators;
using Xunit;

namespace Server.Tests
{
    [Collection("/register")]
    public class RegisterTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public RegisterTest(WebApplicationFactory<Startup> factory) => this.factory = factory;

        private HttpResponseMessage Register(string username, string password, HttpClient? client = null)
            => Helpers.Register(username, password, client, factory).Result;

        [Property(MaxTest = 20, Arbitrary = new[] {typeof(ValidPassword)})]
        public void CanRegisterUser(string password)
        {
            // Arrange
            var username = string.Concat(password.Skip(2).Reverse());

            // Act
            HttpResponseMessage response = Register(username, password);

            // Assert
            ResponseBody? body = response.Content.ReadFromJsonAsync<ResponseBody>().Result;
            Assert.NotNull(body);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Property(Arbitrary = new[] {typeof(ValidUsername)})]
        public void UsernameAlreadyTaken(string username)
        {
            // Arrange
            HttpClient client = factory.CreateClient();

            // Act
            HttpResponseMessage response1 = Register(username, "password1", client);
            HttpResponseMessage response2 = Register(username, "password2", client);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Property(Arbitrary = new[] {typeof(InvalidUsername)})]
        public void RejectsInvalidUsernames(string username, string password)
        {
            // Act
            HttpResponseMessage response = Register(username, "validpassword");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Property(Arbitrary = new[] {typeof(InvalidPassword)})]
        public void RejectsInvalidPasswords(string password)
        {
            // Act
            HttpResponseMessage response = Register("validusername", password);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}