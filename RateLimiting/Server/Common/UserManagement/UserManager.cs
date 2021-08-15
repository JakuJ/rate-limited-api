using System.Linq;

namespace Server.Common.UserManagement
{
    public class UserManager : IUserManager
    {
        private readonly StorageContext context;
        private readonly IPasswordHasher hasher;

        public UserManager(StorageContext ctx, IPasswordHasher hasher)
        {
            context = ctx;
            this.hasher = hasher;
        }

        public int? RegisterUser(string username, string password)
        {
            bool exists = context.Users.Any(u => u.Username == username);
            if (exists)
            {
                return null;
            }

            string hash = hasher.HashPassword(password);

            User user = new() {Username = username, PasswordHash = hash};

            context.Users.Add(user);
            context.SaveChanges();

            return user.UserId;
        }

        public bool ValidateUser(int userId, string username, string password)
        {
            User? user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null || user.Username != username)
            {
                return false;
            }

            return hasher.VerifyPassword(user.PasswordHash, password);
        }
    }
}