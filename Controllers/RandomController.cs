using System;
using Microsoft.AspNetCore.Mvc;
using RateLimiting.Helpers;
using RateLimiting.Randomness;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("random")]
    [BasicAuth("rate-limiter.com")]
    public class RandomController : Controller
    {
        private readonly IRandomGenerator generator = new CryptoProvider();

        private class ResponseObject
        {
            public string random { get; }
            public ResponseObject(string random) => this.random = random;
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "len")] string? len)
        {
            var numBytes = 32; // TODO: Move default to config
            if (len != null)
            {
                bool success = int.TryParse(len, out int length);
                if (success && length > 0)
                {
                    numBytes = length;
                }
            }

            byte[] randomness = generator.GenerateRandomness(numBytes);
            string encoded = Convert.ToBase64String(randomness);
            ResponseObject response = new(encoded);

            Response.Headers.Add("X-Rate-Limit", "1024");

            return new JsonResult(response);
        }
    }
}