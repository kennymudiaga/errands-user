using Errands.Users.Domain.Models;
using System.Threading.Tasks;

namespace Errands.Users.Domain.Commands
{
    public interface IAccountCommand
    {
        Task<LoginResponse> Login(LoginRequest model);
        Task<LoginResponse> SignUp(SignUpRequest model);
        Task<string> ChangePassword(string userId, PasswordChangeRequest model);
    }
}
