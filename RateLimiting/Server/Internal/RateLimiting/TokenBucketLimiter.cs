using System;
using System.Collections.Concurrent;

namespace Server.Internal.RateLimiting
{
    /// <summary>
    /// An implementation of the Token Bucket rate limiting algorithm.
    /// </summary>
    internal class TokenBucketLimiter : IRateLimiter
    {
        private readonly IRateLimitConfig limitConfig;

        private readonly ConcurrentDictionary<int, Bucket> limits;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketLimiter"/> class.
        /// </summary>
        /// <param name="limitConfig">An injected instance of <see cref="IRateLimitConfig"/>.</param>
        public TokenBucketLimiter(IRateLimitConfig limitConfig)
        {
            this.limitConfig = limitConfig;

            // initial capacity of 16 is arbitrary
            limits = new ConcurrentDictionary<int, Bucket>(Environment.ProcessorCount * 2, 16);
        }

        /// <inheritdoc/>
        public (bool Success, int Remaining) TryConsumeResource(int user, int usage)
        {
            var success = false;
            var remaining = 0;

            limits.AddOrUpdate(
                user,
                _ =>
                {
                    // Create a new Bucket
                    (int limit, int window) = limitConfig.GetLimitForUser(user);
                    Bucket bucket = new(limit, window);

                    // Immediately consume the tokens
                    (success, remaining) = bucket.ConsumeTokens(usage);
                    return bucket;
                },
                (_, bucket) =>
                {
                    (success, remaining) = bucket.ConsumeTokens(usage);
                    return bucket;
                });

            return (success, remaining);
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
