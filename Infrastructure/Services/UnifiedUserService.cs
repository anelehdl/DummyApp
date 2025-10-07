using Core.Models;
using Core.Models.DTO;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class UnifiedUserService
    {
        private readonly MongoDBContext _context;
        private readonly RoleService _roleService;

        public UnifiedUserService(MongoDBContext context, RoleService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        /// <summary>
        /// Get all users from both Staff and Client collections with optional filtering
        /// </summary>
        public async Task<List<UnifiedUserDto>> GetAllUsersAsync(UserFilterDto? filter = null)
        {
            var unifiedUsers = new List<UnifiedUserDto>();

            // Get Staff users if not filtered to Client only
            if (string.IsNullOrEmpty(filter?.UserType) || filter.UserType == "Staff")
            {
                var staffMembers = await _context.StaffCollection.Find(_ => true).ToListAsync();

                foreach (var staff in staffMembers)
                {
                    var role = await _roleService.GetRoleByIdAsync(staff.RoleId.ToString());

                    // Apply role filter
                    if (filter?.RoleId != null && staff.RoleId.ToString() != filter.RoleId)
                        continue;

                    // Apply search filter
                    if (!string.IsNullOrEmpty(filter?.SearchTerm))
                    {
                        var searchLower = filter.SearchTerm.ToLower();
                        if (!staff.FirstName.ToLower().Contains(searchLower) &&
                            !staff.Email.ToLower().Contains(searchLower))
                            continue;
                    }

                    unifiedUsers.Add(new UnifiedUserDto
                    {
                        Id = staff.Id.ToString(),
                        UserType = "Staff",
                        FirstName = staff.FirstName,
                        Email = staff.Email,
                        Phone = staff.Phone,
                        RoleName = role?.Name ?? "Unknown",
                        RoleId = staff.RoleId.ToString()
                    });
                }
            }

            // Get Client users if not filtered to Staff only
            if (string.IsNullOrEmpty(filter?.UserType) || filter.UserType == "Client")
            {
                var clients = await _context.ClientCollection.Find(_ => true).ToListAsync();

                foreach (var client in clients)
                {
                    var role = await _roleService.GetRoleByIdAsync(client.RoleId.ToString());

                    // Apply role filter
                    if (filter?.RoleId != null && client.RoleId.ToString() != filter.RoleId)
                        continue;

                    // Apply search filter
                    if (!string.IsNullOrEmpty(filter?.SearchTerm))
                    {
                        var searchLower = filter.SearchTerm.ToLower();
                        if (!client.UserCode.ToLower().Contains(searchLower) &&
                            !client.Username.ToLower().Contains(searchLower)) ;
                            continue;
                    }

                    unifiedUsers.Add(new UnifiedUserDto
                    {
                        Id = client.Id.ToString(),
                        UserType = "Client",
                        UserCode = client.UserCode,
                        Username = client.Username,
                        RoleName = role?.Name ?? "Unknown",
                        RoleId = client.RoleId.ToString()
                    });
                }
            }

            return unifiedUsers;
        }

        /// <summary>
        /// Get specific user by ID and type (Staff or Client)
        /// </summary>
        public async Task<UnifiedUserDto?> GetUserByIdAsync(string userId, string userType)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
                return null;

            if (userType == "Staff")
            {
                var staff = await _context.StaffCollection
                    .Find(s => s.Id == objectId)
                    .FirstOrDefaultAsync();

                if (staff == null)
                    return null;

                var role = await _roleService.GetRoleByIdAsync(staff.RoleId.ToString());

                return new UnifiedUserDto
                {
                    Id = staff.Id.ToString(),
                    UserType = "Staff",
                    FirstName = staff.FirstName,
                    Email = staff.Email,
                    Phone = staff.Phone,
                    RoleName = role?.Name ?? "Unknown",
                    RoleId = staff.RoleId.ToString()
                };
            }
            else // Client
            {
                var client = await _context.ClientCollection
                    .Find(c => c.Id == objectId)
                    .FirstOrDefaultAsync();

                if (client == null)
                    return null;

                var role = await _roleService.GetRoleByIdAsync(client.RoleId.ToString());

                return new UnifiedUserDto
                {
                    Id = client.Id.ToString(),
                    UserType = "Client",
                    UserCode = client.UserCode,
                    Username = client.Username,
                    RoleName = role?.Name ?? "Unknown",
                    RoleId = client.RoleId.ToString()
                };
            }
        }

        /// <summary>
        /// Create a new Staff user with authentication
        /// </summary>
        public async Task<string> CreateStaffUserAsync(CreateStaffDto createStaff)
        {
            // Validate role exists and is not "client"
            var role = await _roleService.GetRoleByIdAsync(createStaff.RoleId);
            if (role == null)
            {
                throw new ArgumentException("Invalid role specified");
            }

            if (role.Name.ToLower() == "client")
            {
                throw new ArgumentException("Cannot assign client role to staff user");
            }

            // Check if email already exists in Staff collection
            var existingStaff = await _context.StaffCollection
                .Find(s => s.Email == createStaff.Email)
                .FirstOrDefaultAsync();

            if (existingStaff != null)
            {
                throw new InvalidOperationException("Staff user with this email already exists");
            }

            // Create authentication record
            var (hashedPassword, salt) = HashPassword(createStaff.Password);
            var auth = new Authentication
            {
                AuthID = Guid.NewGuid().ToString(),
                Salt = salt,
                HashedPassword = hashedPassword
            };
            await _context.AuthenticationCollection.InsertOneAsync(auth);

            // Create staff record
            var staff = new Staff
            {
                FirstName = createStaff.FirstName,
                Email = createStaff.Email,
                Phone = createStaff.Phone,
                RoleId = ObjectId.Parse(createStaff.RoleId),
                AuthId = auth.Id
            };
            await _context.StaffCollection.InsertOneAsync(staff);

            return staff.Id.ToString();
        }

        /// <summary>
        /// Create a new Client user with optional authentication
        /// </summary>
        public async Task<string> CreateClientUserAsync(CreateClientDto createClient)
        {
            // Get client role automatically
            var clientRole = await _roleService.GetRoleByNameAsync("client");
            if (clientRole == null)
            {
                throw new InvalidOperationException("Client role not found in database. Please ensure 'client' role exists.");
            }

            // Check if user_code already exists
            var existingClient = await _context.ClientCollection
                .Find(c => c.UserCode == createClient.UserCode)
                .FirstOrDefaultAsync();

            if (existingClient != null)
            {
                throw new InvalidOperationException($"Client with user code '{createClient.UserCode}' already exists");
            }

            // Create auth record ONLY if password is provided
            ObjectId? authId = null;
            if (!string.IsNullOrEmpty(createClient.Password))
            {
                var (hashedPassword, salt) = HashPassword(createClient.Password);
                var auth = new Authentication
                {
                    AuthID = Guid.NewGuid().ToString(),
                    Salt = salt,
                    HashedPassword = hashedPassword
                };
                await _context.AuthenticationCollection.InsertOneAsync(auth);
                authId = auth.Id;
            }

            // Create client record
            var client = new Client
            {
                UserCode = createClient.UserCode,
                Username = createClient.Username,
                RoleId = clientRole.Id
            };
            await _context.ClientCollection.InsertOneAsync(client);

            return client.Id.ToString();
        }

        /// <summary>
        /// Delete user (works for both Staff and Client)
        /// </summary>
        public async Task<bool> DeleteUserAsync(string userId, string userType)
        {
            if (!ObjectId.TryParse(userId, out var objectId))
                return false;

            if (userType == "Staff")
            {
                var staff = await _context.StaffCollection
                    .Find(s => s.Id == objectId)
                    .FirstOrDefaultAsync();

                if (staff == null)
                    return false;

                // Delete authentication record
                await _context.AuthenticationCollection
                    .DeleteOneAsync(a => a.Id == staff.AuthId);

                // Delete staff record
                var result = await _context.StaffCollection
                    .DeleteOneAsync(s => s.Id == objectId);

                return result.DeletedCount > 0;
            }
            else // Client
            {
                var client = await _context.ClientCollection
                    .Find(c => c.Id == objectId)
                    .FirstOrDefaultAsync();

                if (client == null)
                    return false;

                // Delete client record
                var result = await _context.ClientCollection
                    .DeleteOneAsync(c => c.Id == objectId);

                return result.DeletedCount > 0;
            }
        }

        /// <summary>
        /// Hash password using SHA256 with salt
        /// </summary>
        private (string hashedPassword, string salt) HashPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                // Generate random salt
                byte[] saltBytes = new byte[32];
                rng.GetBytes(saltBytes);
                string salt = Convert.ToBase64String(saltBytes);

                // Hash password with salt
                using (var sha256 = SHA256.Create())
                {
                    var saltedPassword = password + salt;
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                    string hashed = Convert.ToBase64String(hashBytes);

                    return (hashed, salt);
                }
            }
        }
    }
}