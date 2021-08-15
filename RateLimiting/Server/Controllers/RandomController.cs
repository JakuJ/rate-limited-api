using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Server.Common.BasicAuth;
using Server.Common.Randomness;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [BasicAuth] // TODO: Add realm?
    public class RandomController : Controller
    {
        private readonly IRandomnessSource generator;

        public RandomController(IRandomnessSource generator) => this.generator = generator;

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