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
        private readonly HikeCycledbContext _context;

        public AdminDashboardController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // 1. สถิติพื้นฐาน (เหมือนเดิม)
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.LowStock = await _context.Products.CountAsync(p => p.Stock < 5);
            ViewBag.OutOfStock = await _context.Products.CountAsync(p => p.Stock == 0);
            ViewBag.TotalReviews = await _context.Reviews.CountAsync();

            var startDate = DateTime.Now.Date.AddDays(-6);
            var thaiCulture = new CultureInfo("th-TH");

            // 🚩 ดึงข้อมูลและ Group รายวัน
            var rawData = await _context.Bookings
                .Where(b => b.CreatedAt >= startDate)
                .GroupBy(b => b.CreatedAt.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(b => b.FinalAmount)
                })
                .OrderBy(g => g.Date) // 🚩 สำคัญ: ต้องเรียงจากวันที่เก่าไปใหม่ กราฟถึงจะเดินไปข้างหน้า
                .ToListAsync();

            // สร้าง List ของ 7 วันล่าสุดจริงๆ (ป้องกันวันไหนไม่มีข้อมูลแล้วกราฟฟันหลอ)
            var labels = new List<string>();
            var revenues = new List<decimal>();
            var counts = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                labels.Add(date.ToString("dd MMM", thaiCulture));

                // ค้นหาข้อมูลจาก rawData ถ้าไม่มีให้เป็น 0
                var dayData = rawData.FirstOrDefault(x => x.Date == date);
                revenues.Add(dayData?.Revenue ?? 0);
                counts.Add(dayData?.Count ?? 0);
            }

            ViewBag.ChartLabels = labels;
            ViewBag.ChartRevenues = revenues;
            ViewBag.ChartCounts = counts;


            var topProducts = await _context.BookingItems
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

            // ... ดึงรีวิวส่งไปที่ View ...
            var reviews = await _context.Reviews.OrderBy(r => r.Id).Take(5).ToListAsync();
            return View(reviews);
        }
    }
}