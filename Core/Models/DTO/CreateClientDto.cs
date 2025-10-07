namespace Core.Models.DTO
{
    public class CreateClientDto
    {
        public string UserCode { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; } // Optional - only if client needs login
    }
}
