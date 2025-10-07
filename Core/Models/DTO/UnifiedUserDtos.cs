namespace Core.Models.DTO
{
    public class UnifiedUserDto
    {
        public string Id { get; set; }
        public string UserType { get; set; } // "Staff" or "Client"

        // Staff-specific fields
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Client-specific fields
        public string? UserCode { get; set; }
        public string? Username { get; set; }

        // Common fields
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }

    public class UserFilterDto
    {
        public string? UserType { get; set; } // "Staff" or "Client"
        public string? RoleId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}