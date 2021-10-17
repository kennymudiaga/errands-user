using Errands.Users.Domain.Models;
using System.Threading.Tasks;

namespace Errands.Users.Domain.Queries
{
    public interface IAccountQuery
    {
        Task<bool> IsUsernameAvailable(string username);
        Task<bool> IsEmailAvailable(string email);
        Task<LoginResponse> GetUser(string username);
    }
}
