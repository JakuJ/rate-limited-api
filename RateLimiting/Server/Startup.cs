using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Common.Randomness;
using Server.Common.RateLimiting;
using Server.Common.UserManagement;

namespace Server
{
    [SuppressMessage("ReSharper", "SA1600", Justification = "Boilerplate")]
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            // Database
            services.AddDbContext<Repository>(options => options.UseInMemoryDatabase("database"));

            // DI
            services.AddSingleton<IRandomnessSource, CryptoProvider>();
            services.AddSingleton<IPasswordHasher, ArgonHasher>();
            services.AddSingleton<IRateLimiter, TokenBucketLimiter>();
            services.AddSingleton<IRandomLimitConfig, AppSettingsConfig>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
