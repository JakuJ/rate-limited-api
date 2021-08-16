using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Common.UserManagement;
using Server.Models;
using Server.Models.Register;

namespace Server.Controllers
{
    /// <summary>A controller for the <b>/register</b> endpoint.</summary>
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private readonly IPasswordHasher hasher;
        private readonly Repository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterController"/> class.
        /// </summary>
        /// <param name="repository">An injected instance of <see cref="Repository"/>.</param>
        /// <param name="hasher">An injected instance of <see cref="IPasswordHasher"/>.</param>
        public RegisterController(Repository repository, IPasswordHasher hasher)
        {
            this.repository = repository;
            this.hasher = hasher;
        }

        /// <summary>
        /// An action handler for the POST endpoint.
        /// </summary>
        /// <param name="body">The body of the request.</param>
        /// <returns><see cref="OkObjectResult"/> with registered client ID on success,
        /// <see cref="BadRequestObjectResult"/> with error messages on failure.</returns>
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] RequestBody body)
        {
            // Validation
            bool exists = repository.Users.Any(u => u.Username == body.Username);
            if (exists)
            {
                ModelState.AddModelError("username", "Username already in use");
            }

            if (body.Username is { Length: < 6 or > 64 })
            {
                ModelState.AddModelError("username", "Username must be between 6 and 64 character long");
            }

            if (body.Password is { Length: < 8 or > 64 })
            {
                ModelState.AddModelError("password", "Password must be between 8 and 64 character long");
            }

            if (body.Username.Contains(':'))
            {
                ModelState.AddModelError("username", "Username cannot contain the colon symbol.");
            }

            if (body.Password.Contains(':'))
            {
                ModelState.AddModelError("password", "Username cannot contain the colon symbol.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(ModelState);
            }

            // Create new user
            string hash = hasher.HashPassword(body.Password);
            User user = new() { Username = body.Username, PasswordHash = hash };

            await repository.Users.AddAsync(user);
            await repository.SaveChangesAsync();

            // Return 200 OK
            return new OkObjectResult(new ResponseBody { Id = user.UserId });
        }
    }
}
