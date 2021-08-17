using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Server.Common.BasicAuth;
using Server.Common.Randomness;
using Server.Common.RateLimiting;
using Server.Models.Random;

namespace Server.Controllers
{
    /// <summary>A controller for the <b>/random</b> endpoint.</summary>
    [ApiController]
    [Route("[controller]")]
    [BasicAuth]
    public class RandomController : Controller
    {
        private readonly int defaultSize;
        private readonly IRandomnessSource generator;
        private readonly IRateLimiter rateLimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomController"/> class.
        /// </summary>
        /// <param name="generator">Injected instance of <see cref="IRandomnessSource"/>.</param>
        /// <param name="rateLimiter">Injected instance of <see cref="IRateLimiter"/>.</param>
        /// <param name="configuration">Injected instance of <see cref="IConfiguration"/>.</param>
        public RandomController(IRandomnessSource generator, IRateLimiter rateLimiter, IConfiguration configuration)
        {
            this.generator = generator;
            this.rateLimiter = rateLimiter;
            defaultSize = int.Parse(configuration.GetSection("RandomConfig")["DefaultSize"]);
        }

        /// <summary>
        /// An action handler for the GET endpoint.
        /// </summary>
        /// <param name="len">The number of bytes of randomness to return.</param>
        /// <returns>The result of the action method.</returns>
        [HttpGet]
        public IActionResult Get([FromQuery(Name = "len")] string? len)
        {
            var userId = int.Parse(Request.Headers["X-Client-ID"]);
            int numBytes = defaultSize;

            if (len != null)
            {
                bool success = int.TryParse(len, out int length);
                if (success && length >= numBytes)
                {
                    numBytes = length;
                }
                else
                {
                    return new BadRequestObjectResult(new
                    {
                        message = $"Parameter 'len' must be a number greater than {numBytes}",
                    });
                }
            }

            // Check rate limiter
            (bool hasQuota, int remainingQuota) = rateLimiter.CheckQuota(userId, numBytes);

            Response.Headers.Add("X-Rate-Limit", remainingQuota.ToString());

            if (!hasQuota)
            {
                (int maxBytes, int seconds) = rateLimiter.GetUserLimit(userId);
                Response.StatusCode = 429;
                return new ObjectResult(new
                {
                    message =
                        $"The request would exceed the rate limit. Quota set to {maxBytes} bytes every {seconds} seconds.",
                });
            }

            byte[] randomness = generator.GenerateRandomness(numBytes);
            string encoded = Convert.ToBase64String(randomness);
            ResponseBody response = new(encoded);

            return new OkObjectResult(response);
        }
    }
}
