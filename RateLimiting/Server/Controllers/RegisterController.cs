using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Common.UserManagement;
using Server.Models.Register;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private Repository repository;
        private IPasswordHasher hasher;

        public RegisterController(Repository repository, IPasswordHasher hasher)
        {
            this.repository = repository;
            this.hasher = hasher;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] RequestBody body)
        {
            // Validation
            bool exists = repository.Users.Any(u => u.Username == body.username);
            if (exists)
            {
                ModelState.AddModelError("username", "Username already in use");
            }

            if (body.username is {Length: < 6 or > 64})
            {
                ModelState.AddModelError("username", "Username must be between 6 and 64 character long");
            }

            if (body.password is {Length: < 8 or > 64})
            {
                ModelState.AddModelError("password", "Password must be between 8 and 64 character long");
            }

            if (body.username.Contains(':'))
            {
                ModelState.AddModelError("username", "Username cannot contain the colon symbol.");
            }

            if (body.password.Contains(':'))
            {
                ModelState.AddModelError("password", "Username cannot contain the colon symbol.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(ModelState);
            }

            // Create new user
            string hash = hasher.HashPassword(body.password);
            User user = new() {Username = body.username, PasswordHash = hash};

            await repository.Users.AddAsync(user);
            await repository.SaveChangesAsync();

            // Return 200 OK
            return new OkObjectResult(new ResponseBody {Id = user.UserId});
        }
    }
}