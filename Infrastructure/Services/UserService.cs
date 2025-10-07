using Core.Models;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Services
{
    public class UserService
    {
        private readonly MongoDBContext _context;

        public UserService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<string> CreateStaffUserAsync(ApiUserData apiUser, string password)
        {
            // 1. Get or create the admin role
            var adminRole = await _context.RolesCollection
                .Find(r => r.Name == "admin")
                .FirstOrDefaultAsync();

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "admin",
                    Description = "Full access to system management",
                    Permissions = new List<string>
                {
                    "create_user",
                    "delete_user",
                    "manage_roles",
                    "view_reports"
                }
                };
                await _context.RolesCollection.InsertOneAsync(adminRole);
            }

            // 2. Create authentication record
            var (hashedPassword, salt) = HashPassword(password);
            var auth = new Authentication
            {
                AuthID = Guid.NewGuid().ToString(),
                Salt = salt,
                HashedPassword = hashedPassword
            };
            await _context.AuthenticationCollection.InsertOneAsync(auth);

            // 3. Create staff record
            var staff = new Staff
            {
                FirstName = apiUser.FirstName,
                Email = apiUser.Email,
                Phone = apiUser.Phone,
                RoleId = adminRole.Id,
                AuthId = auth.Id
            };
            await _context.StaffCollection.InsertOneAsync(staff);

            return staff.Id.ToString();
        }

        private (string hashedPassword, string salt) HashPassword(string password)
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[32];
                rng.GetBytes(saltBytes);
                string salt = Convert.ToBase64String(saltBytes);

                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var saltedPassword = password + salt;
                    var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
                    string hashed = Convert.ToBase64String(hashBytes);
                    return (hashed, salt);
                }
            }
        }
    }
}
