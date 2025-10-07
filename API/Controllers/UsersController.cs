using Core.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var apiUserData = new ApiUserData
            {
                FirstName = request.FirstName,
                Email = request.Email,
                Phone = request.Phone
            };

            var userId = await _userService.CreateStaffUserAsync(apiUserData, request.Password);

            return Ok(new { UserId = userId, Message = "User created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public class CreateUserRequest
{
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}