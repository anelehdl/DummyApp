namespace Core.Models.DTO
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
