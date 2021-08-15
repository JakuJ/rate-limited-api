using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Common.UserManagement
{
    [Table("users")]
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(64)]
        public string Username { get; set; }

        [Required] public string PasswordHash { get; set; }
    }
}