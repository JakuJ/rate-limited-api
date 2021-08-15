using Sodium;

namespace Server.Common.UserManagement
{
    public class ArgonHasher : IPasswordHasher
    {
        public string HashPassword(string password)
            => PasswordHash.ArgonHashString(password);

        public bool VerifyPassword(string hash, string password)
            => PasswordHash.ArgonHashStringVerify(hash, password);
    }
}