using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminDashboardController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.LowStock = await _db.Products.CountAsync(p => p.Stock < 5);
            ViewBag.OutOfStock = await _db.Products.CountAsync(p => p.Stock == 0);
            ViewBag.TotalReviews = await _db.Reviews.CountAsync();

            var startDate = DateTime.Now.Date.AddDays(-6);
            var thaiCulture = new CultureInfo("th-TH");

            var rawData = await _db.Bookings
                .Where(b => b.CreatedAt >= startDate)
                .GroupBy(b => b.CreatedAt.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(b => b.FinalAmount)
                })
                .OrderBy(g => g.Date)
                .ToListAsync();

            var labels = new List<string>();
            var revenues = new List<decimal>();
            var counts = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                labels.Add(date.ToString("dd MMM", thaiCulture));

                var dayData = rawData.FirstOrDefault(x => x.Date == date);
                revenues.Add(dayData?.Revenue ?? 0);
                counts.Add(dayData?.Count ?? 0);
            }

            ViewBag.ChartLabels = labels;
            ViewBag.ChartRevenues = revenues;
            ViewBag.ChartCounts = counts;


            var topProducts = await _db.BookingItems
        .GroupBy(bi => bi.Product.Name)
        .Select(g => new
        {
            ProductName = g.Key,
            TotalRented = g.Sum(bi => bi.Quantity)
        })
        .OrderBy(x => x.TotalRented)
        .Take(5)
        .ToListAsync();

            ViewBag.TopProductNames = topProducts.Select(x => x.ProductName).ToList();
            ViewBag.TopProductCounts = topProducts.Select(x => x.TotalRented).ToList();

            var reviews = await _db.Reviews.OrderBy(r => r.Id).Take(5).ToListAsync();
            return View(reviews);
        }
    }
}