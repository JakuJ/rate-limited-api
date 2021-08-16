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
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
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
            services.AddSingleton<IRandomConfig, AppSettingsConfig>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}