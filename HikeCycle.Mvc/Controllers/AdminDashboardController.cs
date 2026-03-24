using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Models.Dto;

namespace HikeCycle.Mvc.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminDashboardController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // 👮 เช็คสิทธิ์ Admin ก่อน (ด่านตรวจ)
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "admin" && role != "staff") return RedirectToAction("Login", "Account");

            // ดึงสถิติต่างๆ มาโชว์
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.LowStock = await _context.Products.CountAsync(p => p.Stock < 5);
            ViewBag.OutOfStock = await _context.Products.CountAsync(p => p.Stock == 0);
            ViewBag.TotalReviews = await _context.Reviews.CountAsync();
    

            return View();
        }
    }
}