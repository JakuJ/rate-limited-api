using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server.Common.UserManagement;

namespace Server.Common.BasicAuth
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string? realm;
        private readonly IUserManager userManager;

        public BasicAuthFilter(IUserManager userManager, string? realm = null)
        {
            this.realm = realm;
            this.userManager = userManager;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string? authHeader = context.HttpContext.Request.Headers["Authorization"];
            string? clientIdHeader = context.HttpContext.Request.Headers["X-Client-ID"];
            if (clientIdHeader != null && authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                string encodedUsernamePassword =
                    authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                // Decode from Base64 to string
                string decodedUsernamePassword =
                    Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                // Split username and password
                string username = decodedUsernamePassword.Split(':', 2)[0];
                string password = decodedUsernamePassword.Split(':', 2)[1];

                // Get Client-ID from header
                bool validId = int.TryParse(clientIdHeader, out int clientId);

                // Check if login is correct
                if (validId && IsAuthorized(clientId, username, password))
                {
                    return;
                }
            }

            // Add realm if it is not null
            if (!string.IsNullOrWhiteSpace(realm))
            {
                context.HttpContext.Response.Headers["WWW-Authenticate"] += $" realm=\"{realm}\"";
            }

            // Return unauthorized
            context.Result = new UnauthorizedResult();
        }

        // Make your own implementation of this
        private bool IsAuthorized(int id, string username, string password)
        {
            // Check that username and password are correct
            return userManager.ValidateUser(id, username, password);
        }
    }
}