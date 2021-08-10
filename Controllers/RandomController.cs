using System;
using Microsoft.AspNetCore.Mvc;
using RateLimiting.Randomness;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("random")]
    public class RandomController : IDisposable
    {
        private readonly IRandomGenerator generator = new CryptoProvider();

        private class Response
        {
            public string random { get; }
            public Response(string random) => this.random = random;
        }

        [HttpGet]
        public IActionResult Get()
        {
            byte[] randomness = generator.GenerateRandomness(32);
            string encoded = Convert.ToBase64String(randomness);
            Response response = new(encoded);
            return new JsonResult(response);
        }

        public void Dispose() => generator.Dispose();
    }
}