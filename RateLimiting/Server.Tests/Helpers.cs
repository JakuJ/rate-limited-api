using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Server.Models.Register;
using Xunit;

namespace Server.Tests
{
    public static class Helpers
    {
        public static async Task<HttpResponseMessage> Register(string username, string password, HttpClient client,
            WebApplicationFactory<Startup> factory)
        {
            client ??= factory.CreateClient();
            RequestBody requestBody = new() {password = password, username = username};
            return await client.PostAsJsonAsync("/register", requestBody);
        }
    }
}