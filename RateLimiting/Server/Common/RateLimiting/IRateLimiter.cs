namespace Server.Common.RateLimiting
{
    /// <summary>
    /// Represents a rate limiter that manages access to some resource for different users.
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// Check whether the user can consume a given amount of the limited resource.
        /// </summary>
        /// <param name="user">User ID.</param>
        /// <param name="usage">Size of the resource to be consumed.</param>
        /// <returns>Whether the user is allowed to consume the resource, and how much of it is left.</returns>
        (bool Success, int Remaining) CheckQuota(int user, int usage);

        /// <summary>
        /// Return information on the limits imposed on a user.
        /// </summary>
        /// <param name="user">User ID.</param>
        /// <returns>A pair (X, Y), such that the user is limited to use X resources per Y seconds.</returns>
        (int Bytes, int Seconds) GetUserLimit(int user);
    }
}
