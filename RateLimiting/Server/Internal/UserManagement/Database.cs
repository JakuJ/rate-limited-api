using Microsoft.EntityFrameworkCore;
using Server.Models;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
#nullable disable
namespace Server.Internal.UserManagement
{
    /// <summary>
    /// Represents a session with the database.
    /// </summary>
    public class Database : DbContext
    {
        /// <inheritdoc cref="DbContext"/>
        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the mapping for the table containing users of the <b>/random</b> endpoint.
        /// </summary>
        public DbSet<User> Users { get; set; }
    }
}
