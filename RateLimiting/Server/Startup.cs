using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Common.Randomness;
using Server.Common.UserManagement;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddAuthentication();

            services.AddSingleton<IRandomnessSource, CryptoProvider>();

            services.AddDbContext<Repository>(options => options.UseInMemoryDatabase("database"));
            services.AddSingleton<IPasswordHasher, ArgonHasher>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}