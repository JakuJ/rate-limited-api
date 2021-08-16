using System;
using System.Collections.Generic;

namespace Server.Common.RateLimiting
{
    public class TokenBucketLimiter : IRateLimiter
    {
        private class LimitState
        {
            private DateTime LastRequest { get; set; }
            private double BytesRemaining { get; set; }
            public int MaxLimit { get; }
            public int WindowWidth { get; }

            public LimitState(int maxLimit, int windowWidth)
            {
                LastRequest = DateTime.Now;
                MaxLimit = maxLimit;
                WindowWidth = windowWidth;
                BytesRemaining = maxLimit;
            }

            public (bool success, int remaining) ConsumeLimit(int howMany)
            {
                UpdateRemaining();

                if (howMany > BytesRemaining)
                {
                    return (false, (int) BytesRemaining);
                }

                BytesRemaining -= howMany;
                return (true, (int) BytesRemaining);
            }

            private void UpdateRemaining()
            {
                DateTime now = DateTime.Now;
                TimeSpan delta = now - LastRequest;
                double gainedSinceLastRequest = delta.TotalSeconds * RefillRate;

                BytesRemaining = Math.Min(MaxLimit, BytesRemaining + gainedSinceLastRequest);
                LastRequest = now;
            }

            private double RefillRate => (double) MaxLimit / WindowWidth;
        }

        private readonly Dictionary<int, LimitState> limits = new();
        private readonly IRandomConfig config;

        public TokenBucketLimiter(IRandomConfig config)
        {
            this.config = config;
        }

        public (bool success, int remaining) CheckQuota(int user, int usage)
        {
            lock (limits)
            {
                // If user does not have a limit configured, create a new entry
                if (!limits.ContainsKey(user))
                {
                    limits.Add(user, new LimitState(config.DefaultLimit, config.DefaultWindow));
                }

                // Consume the limit
                return limits[user].ConsumeLimit(usage);
            }
        }

        public (int bytes, int seconds) GetUserLimit(int user)
        {
            return limits.TryGetValue(user, out LimitState? data)
                ? (data.MaxLimit, data.WindowWidth)
                : (GetDefaultLimit: config.DefaultLimit, GetDefaultWindow: config.DefaultWindow);
        }
    }
}