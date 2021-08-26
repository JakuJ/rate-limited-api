using System;
using Microsoft.Extensions.Configuration;

namespace Server.Internal.RateLimiting
{
    /// <summary>
    /// Provides default configuration for rate limiter
    /// based on the <b>appsettings.json</b> file.
    /// </summary>
    internal class AppSettingsConfig : IRateLimitConfig
    {
        private readonly IConfigurationSection config;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsConfig"/> class.
        /// </summary>
        /// <param name="configuration">The injected instance of <see cref="IConfiguration"/>.</param>
        public AppSettingsConfig(IConfiguration configuration)
        {
            config = configuration.GetSection("RandomConfig");
            if (config == null)
            {
                throw new Exception("RandomConfig section missing from the appsettings file.");
            }
        }

        /// <inheritdoc/>
        public int DefaultSize => int.Parse(config["DefaultSize"]);

        /// <inheritdoc/>
        public int DefaultLimit => int.Parse(config["DefaultLimit"]);

        /// <inheritdoc/>
        public int DefaultWindow => int.Parse(config["DefaultWindow"]);

        /// <inheritdoc/>
        public (int Limit, int Window) GetLimitForUser(int user)
        {
            IConfigurationSection? ov = config.GetSection($"LimitOverrides:{user}");
            return (
                ov?.GetValue<int?>("Limit") ?? DefaultLimit,
                ov?.GetValue<int?>("Window") ?? DefaultWindow);
        }
    }
}
