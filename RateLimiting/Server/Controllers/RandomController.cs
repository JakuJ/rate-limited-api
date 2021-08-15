using System;
using Microsoft.AspNetCore.Mvc;
using Server.Common.BasicAuth;
using Server.Common.Randomness;
using Server.Models.Random;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [BasicAuth] // TODO: Add realm?
    public class RandomController : Controller
    {
        private readonly IRandomnessSource generator;

        public RandomController(IRandomnessSource generator) => this.generator = generator;

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "len")] string? len)
        {
            var numBytes = 32; // TODO: Move default to config
            if (len != null)
            {
                bool success = int.TryParse(len, out int length);
                if (success && length is >= 32 and <= 1024)
                {
                    numBytes = length;
                }
                else
                {
                    return new BadRequestObjectResult(new {message = "Parameter 'len' must be between 32 and 1024"});
                }
            }

            byte[] randomness = generator.GenerateRandomness(numBytes);
            string encoded = Convert.ToBase64String(randomness);
            ResponseBody response = new(encoded);

            Response.Headers.Add("X-Rate-Limit", (1024 - numBytes).ToString());

            return new OkObjectResult(response);
        }
    }
}