using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Internal.Randomness;
using Server.Internal.RateLimiting;
using Server.Internal.UserManagement;

namespace Server
{
    [SuppressMessage("ReSharper", "SA1600", Justification = "Boilerplate")]
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            // Database context
            services.AddDbContext<Database>(options => options.UseInMemoryDatabase("database"));

            // Dependency injection
            services.AddSingleton<IRandomnessSource, CryptoProvider>();
            services.AddSingleton<IPasswordHasher, ArgonHasher>();
            services.AddSingleton<IRateLimiter, TokenBucketLimiter>();
            services.AddSingleton<IRateLimitConfig, AppSettingsConfig>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
