namespace Server.Internal.RateLimiting
{
    /// <summary>
    /// Represents a source of default values for configuring the rate limiter for the <b>/random</b> endpoint.
    /// </summary>
    public interface IRateLimitConfig
    {
        /// <summary>
        /// Gets the default size for the byte array returned by the endpoint.
        /// </summary>
        int DefaultSize { get; }

        /// <summary>
        /// Gets the default limit for the number of bytes returned in <see cref="DefaultWindow"/> seconds.
        /// </summary>
        int DefaultLimit { get; }

        /// <summary>
        /// Gets the number of seconds used for calculating the rate limit.
        /// </summary>
        int DefaultWindow { get; }

        /// <summary>
        /// Check the rate limits for a specific user.
        /// </summary>
        /// <param name="user">User ID.</param>
        /// <returns>A pair (X, Y) such that the user can request X bytes per Y seconds.</returns>
        (int Limit, int Window) GetLimitForUser(int user);
    }
}
