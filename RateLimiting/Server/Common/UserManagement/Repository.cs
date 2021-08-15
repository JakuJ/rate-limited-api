using Microsoft.EntityFrameworkCore;

#nullable disable
namespace Server.Common.UserManagement
{
    public class Repository : DbContext
    {
        public Repository(DbContextOptions<Repository> options) : base(options)
        {
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<User> Users { get; set; }
    }
}