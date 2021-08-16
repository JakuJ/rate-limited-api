using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace Server.Models
{
    /// <summary>
    /// An EFCore model for a user.
    /// </summary>
    [Table("users")]
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        /// <summary>
        /// Gets or sets the User ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [MinLength(6)]
        [MaxLength(64)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the string that contains the hashed password, along with the salt and information on the hashing algorithm.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; }
    }
}
