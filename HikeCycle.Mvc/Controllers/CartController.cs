using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace HikeCycle.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly HikeCycledbContext _context;
        private const string CartSessionKey = "UserCart";

        public CartController(HikeCycledbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> Add(int ProductId, string StartDate, string EndDate, string? Size)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages) // ดึงรูปภาพมาด้วย
                .FirstOrDefaultAsync(p => p.Id == ProductId);

            if (product == null) return NotFound();

            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            var cart = string.IsNullOrEmpty(sessionData)
                ? new List<CartSessionItem>()
                : JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData);

            cart!.Add(new CartSessionItem
            {
                ProductId = ProductId,
                ProductName = product.Name,
                // ดึงรูปแรก ถ้าไม่มีให้ใช้รูป Default
                ImageUrl = product.ProductImages.FirstOrDefault()?.ImageUrl ,
                PricePerDay = product.PricePerDay ?? 0,
                // เก็บหมวดหมู่ไว้เช็คเงื่อนไขการแสดง Size
                Category = product.Category,
                Size = Size,
                StartDate = StartDate,
                EndDate = EndDate
            });

            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            var cart = string.IsNullOrEmpty(sessionData)
                ? new List<CartSessionItem>()
                : JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData);

            return View(cart); // ส่ง List<CartSessionItem> ไปที่ View ตรงๆ
        }
    }
}