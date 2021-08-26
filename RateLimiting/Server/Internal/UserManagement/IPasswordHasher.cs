namespace Server.Internal.UserManagement
{
    /// <summary>
    /// Represents a utility for hashing and verifying stored passwords.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash the password with a random salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>A string to be saved in the database.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verify the password against the hash.
        /// </summary>
        /// <param name="hash">The hash of the password.</param>
        /// <param name="password">The password.</param>
        /// <returns>Whether the password matches the provided hash.</returns>
        bool VerifyPassword(string hash, string password);
    }
}
