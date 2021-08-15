using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Server.Common.UserManagement
{
    [Table("users")]
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }
    }
}