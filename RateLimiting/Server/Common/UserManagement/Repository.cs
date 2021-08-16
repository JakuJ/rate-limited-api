using Microsoft.EntityFrameworkCore;
using Server.Models;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
#nullable disable
namespace Server.Common.UserManagement
{
    /// <summary>
    /// Represents a session with the database.
    /// </summary>
    public class Repository : DbContext
    {
        /// <inheritdoc cref="DbContext"/>
        public Repository(DbContextOptions<Repository> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the mapping for the table containing users of the <b>/random</b> endpoint.
        /// </summary>
        public DbSet<User> Users { get; set; }
    }
}
