using GoldenEraMovies.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenEraMovies.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);

        // Identity wrappers for separation of concerns
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<SignInResult> LoginUserAsync(string username, string password, bool rememberMe);
        Task SignInUserAsync(User user, bool isPersistent);
        Task LogoutUserAsync();
        Task<IList<AuthenticationScheme>> GetExternalLoginsAsync();

        // Admin functionality
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<IdentityResult> ChangeUserRoleAsync(int userId, string newRole);
    }
}
