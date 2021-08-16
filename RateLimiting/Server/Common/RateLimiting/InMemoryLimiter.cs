using System;
using System.Collections.Generic;

namespace Server.Common.RateLimiting
{
    public class InMemoryLimiter : IRateLimiter
    {
        private readonly Dictionary<int, int> limits = new();

        public (bool, int) CheckQuota(int user, int usage)
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

        public (int, int) GetUserQuota(int user)
        {
            return (1024, 10); // TODO: Per-user limits
        }
    }
}