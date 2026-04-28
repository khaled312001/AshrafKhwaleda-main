using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Filters;
using PhoneStore.Models;
using PhoneStore.Models.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PhoneStore.Extensions;

namespace PhoneStore.Controllers
{
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class AdminController : Controller
    {
        private readonly StoreDbContext _context;

        public AdminController(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var sevenDaysAgo = today.AddDays(-6);

            var nonCancelledOrders = _context.Orders.AsNoTracking().Where(o => o.Status != OrderStatus.Cancelled);

            var viewModel = new DashboardViewModel
            {
                TotalProducts = await _context.Products.AsNoTracking().CountAsync(),
                TotalOrders = await _context.Orders.AsNoTracking().CountAsync(),
                TotalCompanies = await _context.Companies.AsNoTracking().CountAsync(),
                TotalCategories = await _context.Categories.AsNoTracking().CountAsync(),
                TotalRevenue = await nonCancelledOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                PendingOrders = await _context.Orders.AsNoTracking().CountAsync(o => o.Status == OrderStatus.Pending),
                CompletedOrders = await _context.Orders.AsNoTracking().CountAsync(o => o.Status == OrderStatus.Completed),
                TodayOrders = await _context.Orders.AsNoTracking().CountAsync(o => o.OrderDate >= today),
                TodayRevenue = await _context.Orders.AsNoTracking()
                    .Where(o => o.OrderDate >= today && o.Status != OrderStatus.Cancelled)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
            };

            var nonCancelledCount = await nonCancelledOrders.CountAsync();
            viewModel.AverageOrderValue = nonCancelledCount > 0
                ? viewModel.TotalRevenue / nonCancelledCount
                : 0;

            var ordersByStatus = await _context.Orders.AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in ordersByStatus)
            {
                viewModel.OrderStatusLabels.Add(item.Status.GetDisplayName());
                viewModel.OrderStatusData.Add(item.Count);
            }

            var productsByCompany = await _context.Companies.AsNoTracking()
                .Select(c => new { c.Name, ProductCount = c.Products.Count })
                .OrderByDescending(x => x.ProductCount)
                .Take(10)
                .ToListAsync();

            foreach (var item in productsByCompany)
            {
                viewModel.CompanyLabels.Add(item.Name);
                viewModel.CompanyProductCounts.Add(item.ProductCount);
            }

            var recentOrdersRaw = await _context.Orders.AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new RecentOrderItem
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate,
                    ItemsCount = o.OrderDetails.Count
                })
                .ToListAsync();
            viewModel.RecentOrders = recentOrdersRaw;

            var topProducts = await _context.OrderDetails.AsNoTracking()
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var topProductIds = topProducts.Select(t => t.ProductId).ToList();
            var topProductsData = await _context.Products.AsNoTracking()
                .Where(p => topProductIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.ImageUrl,
                    p.Price,
                    CompanyName = p.Company != null ? p.Company.Name : ""
                })
                .ToListAsync();

            foreach (var t in topProducts)
            {
                var p = topProductsData.FirstOrDefault(x => x.Id == t.ProductId);
                if (p != null)
                {
                    viewModel.TopProducts.Add(new TopProductItem
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ImageUrl = p.ImageUrl,
                        CompanyName = p.CompanyName,
                        Price = p.Price,
                        OrdersCount = t.Count
                    });
                }
            }

            var arabicCulture = new CultureInfo("ar-JO");
            var revenueByDay = await _context.Orders.AsNoTracking()
                .Where(o => o.OrderDate >= sevenDaysAgo && o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Day = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToListAsync();

            for (var d = sevenDaysAgo; d <= today; d = d.AddDays(1))
            {
                var match = revenueByDay.FirstOrDefault(r => r.Day == d);
                viewModel.RevenueLabels.Add(d.ToString("dd MMM", arabicCulture));
                viewModel.RevenueData.Add(match?.Total ?? 0);
            }

            return View(viewModel);
        }
    }
}
