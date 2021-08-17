using Sodium;

namespace Server.Common.UserManagement
{
    /// <summary>
    /// A utility for hashing and verifying passwords using the Argon2id algorithm.
    /// </summary>
    public class ArgonHasher : IPasswordHasher
    {
        /// <inheritdoc/>
        public string HashPassword(string password)
            => PasswordHash.ArgonHashString(password, PasswordHash.StrengthArgon.Medium);

        /// <inheritdoc/>
        public bool VerifyPassword(string hash, string password) => PasswordHash.ArgonHashStringVerify(hash, password);
    }
}
