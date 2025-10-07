using Core.Models;

namespace Core.Interfaces
{
    public interface IUserService
    {
        // Get operations
        Task<Staff?> GetUserByIdAsync(string userId);
        Task<Staff?> GetUserByUsernameAsync(string username);
        Task<Staff?> GetUserByEmailAsync(string email);
        Task<IEnumerable<Staff>> GetAllUsersAsync();
        Task<IEnumerable<Staff>> GetUsersByRoleAsync(string roleId);

        // Create/Update/Delete operations
        Task<bool> CreateUserAsync(Staff user);
        Task<bool> UpdateUserAsync(string userId, Staff user);
        Task<bool> DeleteUserAsync(string userId);
    }
}