using Core.Models;

namespace Core.Interfaces
{
    public interface IAuthenticationService
    {
        // User authentication
        Task<Authentication?> ValidateUserAsync(string username, string password);
        Task<Authentication?> GetUserByIdAsync(string userId);
        Task<Authentication?> GetUserByUsernameAsync(string username);

        // User management
        Task<bool> CreateUserAsync(Authentication user);
        Task<bool> UpdateUserAsync(string userId, Authentication user);
        Task<bool> DeleteUserAsync(string userId);
    }
}