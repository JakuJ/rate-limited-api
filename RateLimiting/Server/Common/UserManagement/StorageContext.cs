using Microsoft.EntityFrameworkCore;

#nullable disable
namespace Server.Common.UserManagement
{
    public class StorageContext : DbContext
    {
        public StorageContext(DbContextOptions<StorageContext> options) : base(options)
        {
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<User> Users { get; set; }
    }
}