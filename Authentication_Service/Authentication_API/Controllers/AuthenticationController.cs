using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security;
using System.Security.Claims;

namespace JWT_API.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ValidateToken()
        {
            return Ok();
            //id Header
            //permission string

        }

       
        public async Task<IActionResult> Login()
        {
            return Ok();

            //Get User ?? null
            //Validate Password
            //return Token
        }

        public async Task<IActionResult> Register()
        {
            return Ok();
            //Get User ?? null
            //Hash Password
            //Publish New User Event
            //return Token
        }

        //var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;

        //if (claimIdentity == null)
        //{
        //    return new List<int>();
        //}

        //var permissionClaims = claimIdentity.FindAll("permission");

        //var permissions = permissionClaims
        //    .Select(c => Enum.TryParse<Permission>(c.Value, out var perm) ? perm : default)
        //    .Where(p => p != default)
        //    .ToList();

        //return permissions;
    }
}
