using System;
using System.Collections.Generic;

namespace Server.Common.RateLimiting
{
    /// <summary>
    /// An implementation of the Token Bucket rate limiting algorithm.
    /// </summary>
    public class TokenBucketLimiter : IRateLimiter
    {
        private readonly IRandomLimitConfig limitConfig;

        private readonly Dictionary<int, Bucket> limits = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketLimiter"/> class.
        /// </summary>
        /// <param name="limitConfig">An injected instance of <see cref="IRandomLimitConfig"/>.</param>
        public TokenBucketLimiter(IRandomLimitConfig limitConfig) => this.limitConfig = limitConfig;

        /// <inheritdoc/>
        public (bool Success, int Remaining) CheckQuota(int user, int usage)
        {
            lock (limits)
            {
                // If user does not have a limit configured, create a new entry
                if (!limits.ContainsKey(user))
                {
                    (int limit, int window) = limitConfig.GetLimitForUser(user);
                    limits.Add(user, new Bucket(limit, window));
                }

                // Consume the limit
                return limits[user].ConsumeTokens(usage);
            }
        }

        /// <inheritdoc/>
        public (int Bytes, int Seconds) GetUserLimit(int user)
            => limits.TryGetValue(user, out Bucket? data)
                ? (data.Capacity, data.RefillTime)
                : (limitConfig.DefaultLimit, limitConfig.DefaultWindow);

        private class Bucket
        {
            public Bucket(int capacity, int refillTime)
            {
                LastRequest = DateTime.Now;
                Capacity = capacity;
                RefillTime = refillTime;
                Tokens = capacity;
            }

            public int Capacity { get; }

            public int RefillTime { get; }

            private double Tokens { get; set; }

            private DateTime LastRequest { get; set; }

            private double RefillRate => (double)Capacity / RefillTime;

            public (bool Success, int Remaining) ConsumeTokens(int howMany)
            {
                RefillTokens();

                if (howMany > Tokens)
                {
                    return (false, (int)Tokens);
                }

                Tokens -= howMany;
                return (true, (int)Tokens);
            }

            private void RefillTokens()
            {
                DateTime now = DateTime.Now;
                TimeSpan delta = now - LastRequest;
                double gainedSinceLastRequest = delta.TotalSeconds * RefillRate;

                Tokens = Math.Min(Capacity, Tokens + gainedSinceLastRequest);
                LastRequest = now;
            }
        }
    }
}
