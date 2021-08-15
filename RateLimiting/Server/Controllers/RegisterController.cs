using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Server.Common.Randomness;
using Server.Common.UserManagement;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private IUserManager userManager;

        public RegisterController(IUserManager userManager) => this.userManager = userManager;

        private class ResponseObject
        {
            public int Id { get; set; }
        }

        public class CreateUserBody
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateUserBody body)
        {
            int? id = userManager.RegisterUser(body.username, body.password);
            if (id != null)
            {
                return new JsonResult(new ResponseObject {Id = id.Value});
            }
            else
            {
                return new BadRequestResult(); // TODO: Message
            }
        }
    }
}