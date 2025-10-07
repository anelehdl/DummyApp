using Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Dashboard.Controllers
{
    [Authorize(Roles = "admin")]
    public class UnifiedUserManagementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UnifiedUserManagementController> _logger;

        public UnifiedUserManagementController(
            IHttpClientFactory httpClientFactory,
            ILogger<UnifiedUserManagementController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET: /UnifiedUserManagement/Index - Main landing page
        public IActionResult Index()
        {
            return View();
        }

        // GET: /UnifiedUserManagement/ViewStaff
        public async Task<IActionResult> ViewStaff(string? searchTerm)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var queryString = "?userType=Staff";
                if (!string.IsNullOrEmpty(searchTerm))
                    queryString += $"&searchTerm={searchTerm}";

                var usersResponse = await client.GetAsync($"/api/unifiedusermanagement/users{queryString}");
                var rolesResponse = await client.GetAsync("/api/unifiedusermanagement/roles");

                if (usersResponse.IsSuccessStatusCode && rolesResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();

                    var users = JsonSerializer.Deserialize<List<UnifiedUserDto>>(usersJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var rolesData = JsonSerializer.Deserialize<Dictionary<string, List<RoleDto>>>(rolesJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ViewBag.StaffRoles = rolesData?["staffRoles"] ?? new List<RoleDto>();
                    ViewBag.CurrentSearch = searchTerm;

                    return View(users ?? new List<UnifiedUserDto>());
                }

                ViewBag.Error = "Failed to load staff users";
                return View(new List<UnifiedUserDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff users");
                ViewBag.Error = "Failed to load staff users";
                return View(new List<UnifiedUserDto>());
            }
        }

        // GET: /UnifiedUserManagement/ViewClients
        public async Task<IActionResult> ViewClients(string? searchTerm, string? isActive)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var queryString = "?userType=Client";
                if (!string.IsNullOrEmpty(searchTerm))
                    queryString += $"&searchTerm={searchTerm}";
                if (!string.IsNullOrEmpty(isActive))
                    queryString += $"&isActive={isActive}";

                var usersResponse = await client.GetAsync($"/api/unifiedusermanagement/users{queryString}");

                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UnifiedUserDto>>(usersJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ViewBag.CurrentSearch = searchTerm;
                    ViewBag.CurrentStatus = isActive;

                    return View(users ?? new List<UnifiedUserDto>());
                }

                ViewBag.Error = "Failed to load client users";
                return View(new List<UnifiedUserDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading client users");
                ViewBag.Error = "Failed to load client users";
                return View(new List<UnifiedUserDto>());
            }
        }

        // GET: /UnifiedUserManagement/ViewAll
        public async Task<IActionResult> ViewAll(string? searchTerm, string? userType)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var queryString = "";
                if (!string.IsNullOrEmpty(searchTerm))
                    queryString += $"?searchTerm={searchTerm}";
                if (!string.IsNullOrEmpty(userType))
                    queryString += (queryString == "" ? "?" : "&") + $"userType={userType}";

                var usersResponse = await client.GetAsync($"/api/unifiedusermanagement/users{queryString}");

                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersJson = await usersResponse.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UnifiedUserDto>>(usersJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ViewBag.CurrentSearch = searchTerm;
                    ViewBag.CurrentUserType = userType;

                    return View(users ?? new List<UnifiedUserDto>());
                }

                ViewBag.Error = "Failed to load users";
                return View(new List<UnifiedUserDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all users");
                ViewBag.Error = "Failed to load users";
                return View(new List<UnifiedUserDto>());
            }
        }

        // POST: /UnifiedUserManagement/CreateStaff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(CreateStaffDto model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/unifiedusermanagement/users/staff", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Staff user created successfully";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to create staff user: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user");
                TempData["Error"] = "An error occurred while creating the staff user";
            }

            return RedirectToAction(nameof(ViewStaff));
        }

        // POST: /UnifiedUserManagement/CreateClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClient(CreateClientDto model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/unifiedusermanagement/users/client", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Client user created successfully";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to create client user: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client user");
                TempData["Error"] = "An error occurred while creating the client user";
            }

            return RedirectToAction(nameof(ViewClients));
        }

        // POST: /UnifiedUserManagement/DeleteStaff/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.DeleteAsync($"/api/unifiedusermanagement/users/staff/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Staff user deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete staff user";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff user");
                TempData["Error"] = "An error occurred while deleting the staff user";
            }

            return RedirectToAction(nameof(ViewStaff));
        }

        // POST: /UnifiedUserManagement/DeleteClient/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClient(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DummyAPI");

                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.DeleteAsync($"/api/unifiedusermanagement/users/client/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Client user deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete client user";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client user");
                TempData["Error"] = "An error occurred while deleting the client user";
            }

            return RedirectToAction(nameof(ViewClients));
        }
    }
}