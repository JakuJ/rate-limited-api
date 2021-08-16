using Microsoft.Extensions.Configuration;

namespace Server.Common.RateLimiting
{
    public class AppSettingsConfig : IRandomConfig
    {
        private readonly IConfiguration configuration;

        public AppSettingsConfig(IConfiguration configuration) => this.configuration = configuration;

        public int DefaultSize => int.Parse(configuration.GetSection("RandomConfig")["DefaultSize"]);
        public int DefaultLimit => int.Parse(configuration.GetSection("RandomConfig")["DefaultLimit"]);
        public int DefaultWindow => int.Parse(configuration.GetSection("RandomConfig")["DefaultWindow"]);
    }
}