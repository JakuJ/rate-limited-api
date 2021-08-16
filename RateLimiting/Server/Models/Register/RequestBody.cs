namespace Server.Models.Register
{
    /// <summary>
    /// Body of a valid request to the <b>/register</b> endpoint.
    /// </summary>
    public record RequestBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBody"/> class.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public RequestBody(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Gets the provided username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the provided password.
        /// </summary>
        public string Password { get; }
    }
}
