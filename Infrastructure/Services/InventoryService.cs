using Core.Models;
using Core.Models.DTO;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Services
{
    public class InventoryService
    {
        private readonly MongoDBContext _context;

        public InventoryService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<StockMetricsOverviewDto> GetStockMetricsOverviewAsync()
        {
            var allInventory = await _context.InventoryCollection
                .Find(_ => true)
                .ToListAsync();

            var allClients = await _context.ClientCollection
                .Find(_ => true)
                .ToListAsync();

            var clientStats = new Dictionary<string, ClientInventoryStatsDto>();

            // Group inventory by user_id
            var inventoryByClient = allInventory.GroupBy(i => i.UserId.ToString());

            foreach (var group in inventoryByClient)
            {
                var clientId = group.Key;
                var client = allClients.FirstOrDefault(c => c.Id.ToString() == clientId);

                if (client == null) continue;

                var orders = group.OrderByDescending(i => i.OrderDate).ToList();
                var lastOrder = orders.FirstOrDefault();

                var stats = new ClientInventoryStatsDto
                {
                    ClientId = clientId,
                    UserCode = client.UserCode,
                    Username = client.Username,
                    TotalOrders = orders.Count,
                    TotalLitres = orders.Sum(o => o.Litres),
                    AverageDailyUsage = orders.Average(o => o.AverageDailyUse),
                    LastOrderDate = lastOrder?.OrderDate,
                    DaysSinceLastOrder = lastOrder != null
                        ? (DateTime.UtcNow - lastOrder.OrderDate).Days
                        : 0,
                    RecentOrders = orders.Take(5).Select(o => new InventoryDto
                    {
                        Id = o.Id.ToString(),
                        Sku = o.Sku,
                        SkuDescription = o.SkuDescription,
                        UserCode = o.UserCode,
                        OrderDate = o.OrderDate,
                        PreviousOrderDate = o.PreviousOrderDate,
                        Litres = o.Litres,
                        DaysBetweenOrders = o.DaysBetweenOrders,
                        AverageDailyUse = o.AverageDailyUse,
                        UserId = o.UserId.ToString()
                    }).ToList(),
                    SkuBreakdown = orders.GroupBy(o => o.SkuDescription)
                        .ToDictionary(g => g.Key, g => g.Sum(o => o.Litres))
                };

                clientStats[clientId] = stats;
            }

            var overview = new StockMetricsOverviewDto
            {
                TotalClients = clientStats.Count,
                TotalOrders = allInventory.Count,
                TotalLitres = allInventory.Sum(i => i.Litres),
                AverageDailyUsageAllClients = allInventory.Any()
                    ? allInventory.Average(i => i.AverageDailyUse)
                    : 0,
                TopClients = clientStats.Values
                    .OrderByDescending(c => c.TotalLitres)
                    .Take(10)
                    .ToList(),
                SkuDistribution = allInventory
                    .GroupBy(i => i.SkuDescription)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.Litres))
            };

            return overview;
        }

        public async Task<ClientInventoryStatsDto?> GetClientInventoryStatsAsync(string clientId)
        {
            if (!ObjectId.TryParse(clientId, out var objectId))
                return null;

            var client = await _context.ClientCollection
                .Find(c => c.Id == objectId)
                .FirstOrDefaultAsync();

            if (client == null)
                return null;

            var inventory = await _context.InventoryCollection
                .Find(i => i.UserId == objectId)
                .SortByDescending(i => i.OrderDate)
                .ToListAsync();

            if (!inventory.Any())
            {
                return new ClientInventoryStatsDto
                {
                    ClientId = clientId,
                    UserCode = client.UserCode,
                    Username = client.Username,
                    TotalOrders = 0,
                    TotalLitres = 0,
                    AverageDailyUsage = 0,
                    LastOrderDate = null,
                    DaysSinceLastOrder = 0
                };
            }

            var lastOrder = inventory.First();

            return new ClientInventoryStatsDto
            {
                ClientId = clientId,
                UserCode = client.UserCode,
                Username = client.Username,
                TotalOrders = inventory.Count,
                TotalLitres = inventory.Sum(i => i.Litres),
                AverageDailyUsage = inventory.Average(i => i.AverageDailyUse),
                LastOrderDate = lastOrder.OrderDate,
                DaysSinceLastOrder = (DateTime.UtcNow - lastOrder.OrderDate).Days,
                RecentOrders = inventory.Take(10).Select(i => new InventoryDto
                {
                    Id = i.Id.ToString(),
                    Sku = i.Sku,
                    SkuDescription = i.SkuDescription,
                    UserCode = i.UserCode,
                    OrderDate = i.OrderDate,
                    PreviousOrderDate = i.PreviousOrderDate,
                    Litres = i.Litres,
                    DaysBetweenOrders = i.DaysBetweenOrders,
                    AverageDailyUse = i.AverageDailyUse,
                    UserId = i.UserId.ToString()
                }).ToList(),
                SkuBreakdown = inventory
                    .GroupBy(i => i.SkuDescription)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.Litres))
            };
        }

        public async Task<List<InventoryDto>> GetInventoryByFilterAsync(InventoryFilterDto filter)
        {
            var filterBuilder = Builders<Inventory>.Filter;
            var filters = new List<FilterDefinition<Inventory>>();

            if (!string.IsNullOrEmpty(filter.ClientId) && ObjectId.TryParse(filter.ClientId, out var clientObjectId))
            {
                filters.Add(filterBuilder.Eq(i => i.UserId, clientObjectId));
            }

            if (!string.IsNullOrEmpty(filter.UserCode))
            {
                filters.Add(filterBuilder.Eq(i => i.UserCode, filter.UserCode));
            }

            if (filter.StartDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(i => i.OrderDate, filter.StartDate.Value));
            }

            if (filter.EndDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(i => i.OrderDate, filter.EndDate.Value));
            }

            if (!string.IsNullOrEmpty(filter.Sku))
            {
                filters.Add(filterBuilder.Eq(i => i.Sku, filter.Sku));
            }

            var combinedFilter = filters.Any()
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            var inventory = await _context.InventoryCollection
                .Find(combinedFilter)
                .SortByDescending(i => i.OrderDate)
                .ToListAsync();

            return inventory.Select(i => new InventoryDto
            {
                Id = i.Id.ToString(),
                Sku = i.Sku,
                SkuDescription = i.SkuDescription,
                UserCode = i.UserCode,
                OrderDate = i.OrderDate,
                PreviousOrderDate = i.PreviousOrderDate,
                Litres = i.Litres,
                DaysBetweenOrders = i.DaysBetweenOrders,
                AverageDailyUse = i.AverageDailyUse,
                UserId = i.UserId.ToString()
            }).ToList();
        }
    }
}