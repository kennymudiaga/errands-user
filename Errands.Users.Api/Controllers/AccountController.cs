using Errands.Users.Domain.Commands;
using Errands.Users.Domain.Models;
using Errands.Users.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Errands.Users.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        [HttpGet, Produces(typeof(LoginResponse))]
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
    }
}
