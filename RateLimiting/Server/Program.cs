using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Common.UserManagement;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IHost? host = CreateHostBuilder(args).Build();

            using (IServiceScope serviceScope = host.Services.CreateScope())
            {
                IServiceProvider services = serviceScope.ServiceProvider;
                IUserManager userManager = services.GetRequiredService<IUserManager>();

                userManager.RegisterUser("someuser", "somepassword");
                userManager.RegisterUser("someotheruser", "somepassword");
            }

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}