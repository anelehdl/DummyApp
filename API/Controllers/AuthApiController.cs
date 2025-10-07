using Core.Interfaces;
using Core.Models.DTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public AuthApiController(AuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required" });
        }

        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }
}