using System;
using Microsoft.AspNetCore.Mvc;
using Server.Common.BasicAuth;
using Server.Common.Randomness;
using Server.Common.RateLimiting;
using Server.Models.Random;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [BasicAuth]
    public class RandomController : Controller
    {
        private readonly IRandomnessSource generator;
        private readonly IRateLimiter rateLimiter;

        public RandomController(IRandomnessSource generator, IRateLimiter rateLimiter)
        {
            this.generator = generator;
            this.rateLimiter = rateLimiter;
        }

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
            
            // Check rate limiter
            var userId = int.Parse(Request.Headers["X-Client-ID"]);
            (bool hasQuota, int remainingQuota) = rateLimiter.CheckQuota(userId, numBytes);

            Response.Headers.Add("X-Rate-Limit", remainingQuota.ToString());
            
            if (!hasQuota)
            {
                (int maxBytes, int seconds) = rateLimiter.GetUserQuota(userId);
                Response.StatusCode = 429;
                return new ContentResult
                {
                    Content = $"Too Many Requests. Quota set to {maxBytes} bytes every {seconds} seconds."
                };
            }

            byte[] randomness = generator.GenerateRandomness(numBytes);
            string encoded = Convert.ToBase64String(randomness);
            ResponseBody response = new(encoded);

            return new OkObjectResult(response);
        }
    }
}