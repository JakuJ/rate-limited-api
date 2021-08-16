using Microsoft.Extensions.Configuration;

namespace Server.Common.RateLimiting
{
    /// <summary>
    /// Provides default configuration for rate limiter
    /// based on the <b>appsettings.json</b> file.
    /// </summary>
    public class AppSettingsLimitConfig : IRandomLimitConfig
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsLimitConfig"/> class.
        /// </summary>
        /// <param name="configuration">The injected instance of <see cref="IConfiguration"/>.</param>
        public AppSettingsLimitConfig(IConfiguration configuration) => this.configuration = configuration;

        /// <inheritdoc/>
        public int DefaultSize => int.Parse(configuration.GetSection("RandomConfig")["DefaultSize"]);

        /// <inheritdoc/>
        public int DefaultLimit => int.Parse(configuration.GetSection("RandomConfig")["DefaultLimit"]);

        /// <inheritdoc/>
        public int DefaultWindow => int.Parse(configuration.GetSection("RandomConfig")["DefaultWindow"]);
    }
}
