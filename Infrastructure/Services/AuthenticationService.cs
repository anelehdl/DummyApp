using Core.Models.DTO;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthenticationService
    {
        private readonly MongoDBContext _context;

        public AuthenticationService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<LoginResponseDto> LoginAsync(string email, string password)
        {
            try
            {
                // 1. Find the staff member by email
                var staff = await _context.StaffCollection
                    .Find(s => s.Email == email)
                    .FirstOrDefaultAsync();

                if (staff == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // 2. Get the authentication record
                var auth = await _context.AuthenticationCollection
                    .Find(a => a.Id == staff.AuthId)
                    .FirstOrDefaultAsync();

                if (auth == null)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Authentication record not found"
                    };
                }

                // 3. Verify the password
                bool isPasswordValid = VerifyPassword(password, auth.HashedPassword, auth.Salt);

                if (!isPasswordValid)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // 4. Get the role information
                var role = await _context.RolesCollection
                    .Find(r => r.Id == staff.RoleId)
                    .FirstOrDefaultAsync();

                // 5. Return success response
                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    UserId = staff.Id.ToString(),
                    Email = staff.Email,
                    FirstName = staff.FirstName,
                    Role = role?.Name ?? "Unknown"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = $"An error occurred during login: {ex.Message}"
                };
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = enteredPassword + storedSalt;
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                var computedHash = Convert.ToBase64String(hashBytes);

                return computedHash == storedHash;
            }
        }
    }
}
