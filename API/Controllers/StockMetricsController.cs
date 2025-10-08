using Core.Models.DTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,staff")]
    public class StockMetricsController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        private readonly ILogger<StockMetricsController> _logger;

        public StockMetricsController(
            InventoryService inventoryService,
            ILogger<StockMetricsController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            try
            {
                _logger.LogInformation("Getting stock metrics overview");
                var overview = await _inventoryService.GetStockMetricsOverviewAsync();
                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock metrics overview");
                return StatusCode(500, new { Error = "An error occurred while retrieving stock metrics", Details = ex.Message });
            }
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientStats(string clientId)
        {
            try
            {
                _logger.LogInformation("Getting inventory stats for client: {ClientId}", clientId);

                var stats = await _inventoryService.GetClientInventoryStatsAsync(clientId);

                if (stats == null)
                {
                    return NotFound(new { Message = "Client not found or no inventory data available" });
                }

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client inventory stats");
                return StatusCode(500, new { Error = "An error occurred while retrieving client stats", Details = ex.Message });
            }
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventory([FromQuery] InventoryFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Getting inventory with filters: {@Filter}", filter);

                var inventory = await _inventoryService.GetInventoryByFilterAsync(filter);

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory");
                return StatusCode(500, new { Error = "An error occurred while retrieving inventory", Details = ex.Message });
            }
        }
    }
}