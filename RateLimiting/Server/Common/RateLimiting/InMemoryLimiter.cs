using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Server.Common.RateLimiting
{
    public class InMemoryLimiter : IRateLimiter
    {
        private readonly Dictionary<int, int> limits = new();
        private readonly IConfiguration configuration;

        public InMemoryLimiter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public (bool success, int remaining) CheckQuota(int user, int usage)
        {
            lock (limits)
            {
                if (!limits.ContainsKey(user))
                {
                    limits.Add(user, 1024);
                }

                int remaining = limits[user];

                if (remaining < usage)
                {
                    return (false, remaining);
                }

                limits[user] = remaining - usage;
                return (true, remaining - usage);
            }
        }

        public (int bytes, int seconds) GetUserLimit(int user)
        {
            string? limit = configuration.GetSection("RandomConfig")["DefaultLimit"];
            string? window = configuration.GetSection("RandomConfig")["DefaultWindow"];
            return (int.Parse(limit), int.Parse(window));
        }
    }
}