using Core.Models.DTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API Controller for unified user management (Staff and Clients)
    /// Admin-only access required for all endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UnifiedUserManagementController : ControllerBase
    {
        private readonly UnifiedUserService _unifiedUserService;
        private readonly RoleService _roleService;
        private readonly ILogger<UnifiedUserManagementController> _logger;

        public UnifiedUserManagementController(
            UnifiedUserService unifiedUserService,
            RoleService roleService,
            ILogger<UnifiedUserManagementController> logger)
        {
            _unifiedUserService = unifiedUserService;
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDto? filter)
        {
            try
            {
                _logger.LogInformation("Getting all users with filters: {@Filter}", filter);

                var users = await _unifiedUserService.GetAllUsersAsync(filter);

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { Error = "An error occurred while retrieving users", Details = ex.Message });
            }
        }

        /// <summary>
        /// Get specific staff user by ID
        /// </summary>
        /// <returns>Staff user details</returns>
        [HttpGet("users/staff/{id}")]
        public async Task<IActionResult> GetStaffUser(string id)
        {
            try
            {
                _logger.LogInformation("Getting staff user with ID: {Id}", id);

                var user = await _unifiedUserService.GetUserByIdAsync(id, "Staff");

                if (user == null)
                {
                    return NotFound(new { Message = "Staff user not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff user with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving staff user", Details = ex.Message });
            }
        }

        /// <summary>
        /// Get specific client user by ID
        /// </summary>
        /// <returns>Client user details</returns>
        [HttpGet("users/client/{id}")]
        public async Task<IActionResult> GetClientUser(string id)
        {
            try
            {
                _logger.LogInformation("Getting client user with ID: {Id}", id);

                var user = await _unifiedUserService.GetUserByIdAsync(id, "Client");

                if (user == null)
                {
                    return NotFound(new { Message = "Client user not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving client user with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving client user", Details = ex.Message });
            }
        }

        /// <summary>
        /// Create a new staff user
        /// </summary>
        /// <returns>Created user ID and success message</returns>
        [HttpPost("users/staff")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto createStaff)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(createStaff.FirstName) ||
                    string.IsNullOrEmpty(createStaff.Email) ||
                    string.IsNullOrEmpty(createStaff.Phone) ||
                    string.IsNullOrEmpty(createStaff.Password) ||
                    string.IsNullOrEmpty(createStaff.RoleId))
                {
                    return BadRequest(new { Message = "All fields (FirstName, Email, Phone, Password, RoleId) are required" });
                }

                _logger.LogInformation("Creating staff user with email: {Email}", createStaff.Email);

                var userId = await _unifiedUserService.CreateStaffUserAsync(createStaff);

                _logger.LogInformation("Staff user created successfully with ID: {UserId}", userId);

                return Ok(new
                {
                    UserId = userId,
                    Message = "Staff user created successfully",
                    UserType = "Staff"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict while creating staff user");
                return Conflict(new { Error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while creating staff user");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user");
                return StatusCode(500, new { Error = "An error occurred while creating staff user", Details = ex.Message });
            }
        }

        /// <summary>
        /// Create a new client user
        /// </summary>
        /// <returns>Created user ID and success message</returns>
        [HttpPost("users/client")]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDto createClient)
        {
            try
            {
                // Validate required fields (only userCode and username are required)
                if (string.IsNullOrEmpty(createClient.UserCode) ||
                    string.IsNullOrEmpty(createClient.Username))
                {
                    return BadRequest(new { Message = "UserCode and Username are required" });
                }

                _logger.LogInformation("Creating client user with code: {UserCode}", createClient.UserCode);

                var userId = await _unifiedUserService.CreateClientUserAsync(createClient);

                _logger.LogInformation("Client user created successfully with ID: {UserId}", userId);

                return Ok(new
                {
                    UserId = userId,
                    Message = "Client user created successfully",
                    UserType = "Client",
                    HasAuthentication = !string.IsNullOrEmpty(createClient.Password)
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict while creating client user");
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client user");
                return StatusCode(500, new { Error = "An error occurred while creating client user", Details = ex.Message });
            }
        }

        /// <summary>
        /// Delete a staff user
        /// </summary>
        /// <returns>Success message</returns>
        [HttpDelete("users/staff/{id}")]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            try
            {
                _logger.LogInformation("Deleting staff user with ID: {Id}", id);

                var result = await _unifiedUserService.DeleteUserAsync(id, "Staff");

                if (!result)
                {
                    return NotFound(new { Message = "Staff user not found" });
                }

                _logger.LogInformation("Staff user deleted successfully: {Id}", id);

                return Ok(new { Message = "Staff user deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff user with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while deleting staff user", Details = ex.Message });
            }
        }

        /// <summary>
        /// Delete a client user
        /// </summary>
        /// <returns>Success message</returns>
        [HttpDelete("users/client/{id}")]
        public async Task<IActionResult> DeleteClient(string id)
        {
            try
            {
                _logger.LogInformation("Deleting client user with ID: {Id}", id);

                var result = await _unifiedUserService.DeleteUserAsync(id, "Client");

                if (!result)
                {
                    return NotFound(new { Message = "Client user not found" });
                }

                _logger.LogInformation("Client user deleted successfully: {Id}", id);

                return Ok(new { Message = "Client user deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client user with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while deleting client user", Details = ex.Message });
            }
        }

        /// <summary>
        /// Get all roles
        /// Returns both all roles and staff-specific roles (excludes client role for staff creation)
        /// </summary>
        /// <returns>Object with AllRoles and StaffRoles lists</returns>
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                _logger.LogInformation("Getting all roles");

                var roles = await _roleService.GetAllRolesAsync();

                var allRoles = roles.Select(r => new RoleDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    Description = r.Description,
                    Permissions = r.Permissions
                }).ToList();

                // Filter out client role for staff creation
                // (staff should only be assigned admin or staff roles)
                var staffRoles = allRoles.Where(r => r.Name.ToLower() != "client").ToList();

                return Ok(new
                {
                    AllRoles = allRoles,
                    StaffRoles = staffRoles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(500, new { Error = "An error occurred while retrieving roles", Details = ex.Message });
            }
        }
    }
}