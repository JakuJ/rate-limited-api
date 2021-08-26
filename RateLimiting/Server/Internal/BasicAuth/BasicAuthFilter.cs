using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server.Internal.UserManagement;
using Server.Models;

namespace Server.Internal.BasicAuth
{
    /// <summary>
    /// A filter that confirms request authorization using BasicAuth.
    /// </summary>
    internal class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly IPasswordHasher hasher;
        private readonly Database database;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthFilter"/> class.
        /// </summary>
        /// <param name="database">Injected user credential repository.</param>
        /// <param name="hasher">Injected password hashing functions provider.</param>
        public BasicAuthFilter(Database database, IPasswordHasher hasher)
        {
            this.database = database;
            this.hasher = hasher;
        }

        /// <inheritdoc/>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check BasicAuth header
            string? authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader == null || !authHeader.StartsWith("Basic "))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Basic authorization header missing" });
                return;
            }

            // Check X-Client-ID header
            string? clientIdHeader = context.HttpContext.Request.Headers["X-Client-ID"];
            if (clientIdHeader == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Missing X-Client-ID header" });
                return;
            }

            // Get Client-ID from header
            bool validId = int.TryParse(clientIdHeader, out int clientId);
            if (!validId)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Client ID must be a number" });
                return;
            }

            // Find user by ID
            User? user = database.Users.FirstOrDefault(u => u.UserId == clientId);
            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid Client ID" });
                return;
            }

            // Get the encoded username and password
            string encodedUsernamePassword =
                authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

            // Decode from Base64 to string
            string decodedUsernamePassword =
                Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            // Split username and password
            int colonIndex = decodedUsernamePassword.IndexOf(':');
            string username = decodedUsernamePassword[.. colonIndex];
            string password = decodedUsernamePassword[(colonIndex + 1)..];

            // Check if credentials are correct
            if (user.Username != username || !hasher.VerifyPassword(user.PasswordHash, password))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid username or password" });
            }
        }
    }
}
