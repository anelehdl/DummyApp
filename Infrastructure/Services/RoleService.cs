using Core.Models;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Services
{
    public class RoleService
    {
        private readonly MongoDBContext _context;

        public RoleService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.RolesCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(string roleId)
        {
            if (!ObjectId.TryParse(roleId, out var objectId))
                return null;

            return await _context.RolesCollection
                .Find(r => r.Id == objectId)
                .FirstOrDefaultAsync();
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.RolesCollection
                .Find(r => r.Name.ToLower() == roleName.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasPermissionAsync(string roleId, string permission)
        {
            var role = await GetRoleByIdAsync(roleId);
            return role?.Permissions?.Contains(permission) ?? false;
        }
    }
}