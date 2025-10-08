using Core.Models.DTO;
using Dashboard.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardAuthService _authService;

        public DashboardController(DashboardAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Dashboard/Index (displays login page)
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Login");
        }

        // POST: /Dashboard/Login (processes login form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model.Email, model.Password);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            // Create claims for cookie authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.GivenName, result.FirstName),
                new Claim(ClaimTypes.Name, result.FirstName),
                new Claim("Token", result.Token)
            };

            // Add role claim if available
            if (!string.IsNullOrEmpty(result.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, result.Role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Dashboard/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}