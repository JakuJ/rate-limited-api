using System;
using Microsoft.AspNetCore.Mvc;
using Server.Internal.BasicAuth;
using Server.Internal.Randomness;
using Server.Internal.RateLimiting;
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
        /// <param name="configuration">Injected instance of <see cref="IRateLimitConfig"/>.</param>
        public RandomController(IRandomnessSource generator, IRateLimiter rateLimiter, IRateLimitConfig configuration)
        {
            this.generator = generator;
            this.rateLimiter = rateLimiter;
            defaultSize = configuration.DefaultSize;
        }

        /// <summary>
        /// An action handler for the GET endpoint.
        /// </summary>
        /// <param name="len">The number of bytes of randomness to return.</param>
        /// <returns>The result of the action method.</returns>
        [HttpGet]
        public IActionResult Get([FromQuery] string? len)
        {
            // Header correctness is verified while checking BasicAuth
            var userId = int.Parse(Request.Headers["X-Client-ID"]);

            // Parse length of requested randomness if query parameter was given
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
                        message = $"Parameter '{nameof(len)}' must be a number greater than {numBytes}",
                    });
                }
            }

            // Consult the rate limiter
            (bool allowed, int remainingQuota) = rateLimiter.TryConsumeResource(userId, numBytes);

            // Response needs to contain the rate limit in a header
            Response.Headers.Add("X-Rate-Limit", remainingQuota.ToString());

            // Return an error if the rate limit was hit
            if (!allowed)
            {
                (int maxBytes, int seconds) = rateLimiter.GetUserLimit(userId);
                Response.StatusCode = 429;
                return new ObjectResult(new
                {
                    message =
                        $"The request would exceed the rate limit. Quota set to {maxBytes} bytes every {seconds} seconds.",
                });
            }

            // Generate and return randomness
            byte[] randomness = generator.GenerateRandomness(numBytes);
            string encoded = Convert.ToBase64String(randomness);

            return new OkObjectResult(new ResponseBody(encoded));
        }
    }
}
