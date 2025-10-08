using Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Dashboard.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class StockMetricsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StockMetricsController> _logger;

        public StockMetricsController(
            IHttpClientFactory httpClientFactory,
            ILogger<StockMetricsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
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

                var response = await client.GetAsync("/api/stockmetrics/overview");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var overview = JsonSerializer.Deserialize<StockMetricsOverviewDto>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(overview);
                }

                ViewBag.Error = "Failed to load stock metrics";
                return View(new StockMetricsOverviewDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock metrics");
                ViewBag.Error = "An error occurred while loading stock metrics";
                return View(new StockMetricsOverviewDto());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ClientDetails(string clientId)
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

                var response = await client.GetAsync($"/api/stockmetrics/client/{clientId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var stats = JsonSerializer.Deserialize<ClientInventoryStatsDto>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(stats);
                }

                ViewBag.Error = "Failed to load client details";
                return View(new ClientInventoryStatsDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading client details");
                ViewBag.Error = "An error occurred while loading client details";
                return View(new ClientInventoryStatsDto());
            }
        }
    }
}