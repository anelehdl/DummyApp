namespace Core.Models.DTO
{
    public class InventoryDto
    {
        public string Id { get; set; }
        public int Sku { get; set; }  // Changed from string to int
        public string SkuDescription { get; set; }
        public string UserCode { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PreviousOrderDate { get; set; }
        public int Litres { get; set; }
        public int DaysBetweenOrders { get; set; }
        public double AverageDailyUse { get; set; }
        public string UserId { get; set; }
    }

    public class ClientInventoryStatsDto
    {
        public string ClientId { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public int TotalOrders { get; set; }
        public int TotalLitres { get; set; }
        public double AverageDailyUsage { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public List<InventoryDto> RecentOrders { get; set; } = new();
        public Dictionary<string, int> SkuBreakdown { get; set; } = new();
    }

    public class InventoryFilterDto
    {
        public string? ClientId { get; set; }
        public string? UserCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Sku { get; set; }
    }

    public class StockMetricsOverviewDto
    {
        public int TotalClients { get; set; }
        public int TotalOrders { get; set; }
        public int TotalLitres { get; set; }
        public double AverageDailyUsageAllClients { get; set; }
        public List<ClientInventoryStatsDto> TopClients { get; set; } = new();
        public Dictionary<string, int> SkuDistribution { get; set; } = new();
    }
}