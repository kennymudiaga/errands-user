using Errands.Users.Domain.Commands;
using Errands.Users.Domain.Models;
using Errands.Users.Domain.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Errands.Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class AccountController : ControllerBase
    {
        private readonly IAccountQuery accountQuery;
        private readonly IAccountCommand accountCommand;
        public AccountController(IAccountQuery accountQuery, IAccountCommand accountCommand)
        {
            this.accountQuery = accountQuery;
            this.accountCommand = accountCommand;
        }

        [HttpGet("IsEmailAvailable"), Produces(typeof(bool))]
        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            var isAvailable = await accountQuery.IsEmailAvailable(email);
            return Ok(isAvailable);
        }

        [HttpGet("IsUsernameAvailable"), Produces(typeof(bool))]
        public async Task<IActionResult> IsUsernameAvailable(string username)
        {
            var isAvailable = await accountQuery.IsUsernameAvailable(username);
            return Ok(isAvailable);
        }
        [HttpGet, Authorize, Produces(typeof(LoginResponse))]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await accountQuery.GetUser(username);
            return Ok(user);
        }

        [HttpPost, Produces(typeof(LoginResponse))]
        public async Task<IActionResult> Register(SignUpRequest request)
        {
            var login = await accountCommand.SignUp(request);
            return Ok(login);
        }

        [HttpPost("Login"), Produces(typeof(LoginResponse))]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var login = await accountCommand.Login(request);
            return Ok(login);
        }

        [HttpPut("Password"), Authorize, Produces(typeof(string))]
        public async Task<IActionResult> ChangePassword(PasswordChangeRequest request)
        {
            var message = await accountCommand.ChangePassword(User.FindFirst(ClaimTypes.Sid)?.Value, request);
            return Ok(message);
        }
    }
}
