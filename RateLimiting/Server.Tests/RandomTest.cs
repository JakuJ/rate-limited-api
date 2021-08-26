using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FsCheck.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Internal.RateLimiting;
using Server.Models.Random;
using Server.Tests.Generators;
using Server.Tests.Helpers;
using Server.Tests.Helpers.Fixtures;
using Xunit;

namespace Server.Tests
{
    public class RandomTest : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<RandomTestFixture>
    {
        private readonly WebApplicationFactory<Startup> factory;
        private readonly RandomTestFixture fixture;

        public RandomTest(WebApplicationFactory<Startup> factory, RandomTestFixture fixture)
        {
            this.factory = factory;
            this.fixture = fixture;
        }

        private IRateLimitConfig LimitConfig => factory.Services.GetService<IRateLimitConfig>()!;

        private static async Task<HttpResponseMessage> Random(HttpClient client, int? bytes = null)
        {
            if (bytes != null)
            {
                return await client.GetAsync($"/random?len={bytes}");
            }

            return await client.GetAsync("/random");
        }

        private async Task<HttpClient> NewContext() => await RandomHelper.NewContext(fixture, factory);

        [Fact]
        public async Task ReturnsConfiguredNumberOfBytesByDefault()
        {
            // Arrange
            HttpClient client = await NewContext();

            // Act
            HttpResponseMessage response = await Random(client);
            ResponseBody body = await response.Content.ReadFromJsonAsync<ResponseBody>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(LimitConfig.DefaultSize, Convert.FromBase64String(body.Random).Length);
            Assert.Equal(
                (LimitConfig.DefaultLimit - LimitConfig.DefaultSize).ToString(),
                response.Headers.GetValues("X-Rate-Limit").Single());
        }

        [Fact]
        public async Task CanRequestAllAvailableBytesAtOnce()
        {
            // Arrange
            HttpClient client = await NewContext();
            var clientID = int.Parse(client.DefaultRequestHeaders.GetValues("X-Client-ID").Single());
            (int len, _) = LimitConfig.GetLimitForUser(clientID);

            // Act
            HttpResponseMessage response = await Random(client, len);
            ResponseBody body = await response.Content.ReadFromJsonAsync<ResponseBody>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(len, Convert.FromBase64String(body.Random).Length);
            Assert.Equal("0", response.Headers.GetValues("X-Rate-Limit").Single());
        }

        [Property(Arbitrary = new[] { typeof(ValidLen) })]
        public void RespectsTheQueryParameter(int len)
        {
            // Arrange
            HttpClient client = NewContext().Result;

            // Act
            HttpResponseMessage response = Random(client, len).Result;
            ResponseBody body = response.Content.ReadFromJsonAsync<ResponseBody>().Result;

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
        public async Task LimitsRateForConcurrentRequestsFromTheSameUser()
        {
            // Arrange
            HttpClient client = await NewContext();
            int[] lens = { 32, 37, 48, 53, 64, 70 };

            List<(DateTime, int)> responses = new();

            async Task DoWork(int len)
            {
                DateTime start = DateTime.Now;
                while (true)
                {
                    HttpResponseMessage response = await Random(client, len);
                    if (response.IsSuccessStatusCode)
                    {
                        responses.Add((DateTime.Now, len));
                    }

                    if (DateTime.Now - start > TimeSpan.FromSeconds(30))
                    {
                        break;
                    }
                }
            }

            // Act
            Task[] tasks = lens.Select(DoWork).ToArray();
            Task.WaitAll(tasks);

            // Assert
            const double marginOfError = 20.0;

            // Note: The 'Testing' environment does not override any rate limits
            double rechargeRate = LimitConfig.DefaultLimit / (double)LimitConfig.DefaultWindow;

            Assert.NotEmpty(responses);
            DateTime start = responses.First().Item1;

            // At no point in time the total received number of bytes can exceed the following value:
            // maxBytes[b] = Limit[b] + Time[s] * (Limit[b] / Window[s]), where b - bytes, s - seconds
            // Reference: http://intronetworks.cs.luc.edu/current/html/tokenbucket.html
            var totalLen = 0;
            foreach ((TimeSpan t, int len) in responses.Select(t => (t.Item1 - start, t.Item2)))
            {
                totalLen += len;
                double maxExpected = LimitConfig.DefaultLimit + t.TotalSeconds * rechargeRate;
                Assert.InRange(totalLen, 0, maxExpected + marginOfError);
            }
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
            Task[] tasks = clients.ZipWith(lens, DoWork).ToArray();
            Task.WaitAll(tasks);

            // Assert
            const double marginOfError = 20.0;

            // Note: The 'Testing' environment does not override any rate limits
            double rechargeRate = LimitConfig.DefaultLimit / (double)LimitConfig.DefaultWindow;

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
