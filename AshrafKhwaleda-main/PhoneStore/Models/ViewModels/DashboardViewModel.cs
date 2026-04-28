using System.Collections.Generic;

namespace PhoneStore.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCompanies { get; set; }
        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalCategories { get; set; }
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }

        public List<RecentOrderItem> RecentOrders { get; set; } = new();
        public List<TopProductItem> TopProducts { get; set; } = new();

        public List<string> OrderStatusLabels { get; set; } = new();
        public List<int> OrderStatusData { get; set; } = new();

        public List<string> CompanyLabels { get; set; } = new();
        public List<int> CompanyProductCounts { get; set; } = new();

        public List<string> RevenueLabels { get; set; } = new();
        public List<decimal> RevenueData { get; set; } = new();
    }

    public class RecentOrderItem
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int ItemsCount { get; set; }
    }

    public class TopProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        public decimal Price { get; set; }
    }
}
