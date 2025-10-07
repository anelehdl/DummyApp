using Core.Models.DTO;
using System.Text;
using System.Text.Json;

namespace Dashboard.Services
{
    public class DashboardAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DashboardAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(string email, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var loginRequest = new LoginRequestDto
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result ?? new LoginResponseDto { Success = false, Message = "Failed to parse response" };
                }
                else
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Login failed"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}