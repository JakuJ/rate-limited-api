namespace Server.Common.UserManagement
{
    /// <summary>
    /// Represents a utility for hashing and verifying stored passwords.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash a password with a random salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>A string to be saved in the database.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verify a password against a hash.
        /// </summary>
        /// <param name="hash">The hash of the password.</param>
        /// <param name="password">The password.</param>
        /// <returns>Whether the password matches the provided hash.</returns>
        bool VerifyPassword(string hash, string password);
    }
}
