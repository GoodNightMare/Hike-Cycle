using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public ReviewsController(HikeCycledbContext context)
        {
            _context = context;
        }

        // GET: /Reviews/Index/5 (เลข 5 คือ ProductId)
        public async Task<IActionResult> Index(int id)
        {
            // 1. ดึงข้อมูลสินค้า (เพื่อเอาชื่อมาโชว์หัวข้อ)
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // 2. ดึงรีวิวของสินค้านั้นๆ และ Join กับ UserProfile เพื่อเอาชื่อมาโชว์ (ใช้ Left Join)
            var reviews = await (from r in _context.Reviews
                                 where r.ProductId == id
                                 join u in _context.UserProfiles on r.UserId equals u.UserId into userGroup
                                 from u in userGroup.DefaultIfEmpty() // Left Join
                                 orderby r.CreatedAt descending
                                 select new 
                                 {
                                     Id = r.Id,
                                     UserId = r.UserId,
                                     UserName = u != null ? u.FullName : "Anonymous", // ถ้าไม่มี user ให้ใช้ชื่อ "Anonymous"
                                     Rating = r.Rating,
                                     Comment = r.Comment,
                                     CreatedAt = r.CreatedAt
                                 }).ToListAsync();

            ViewBag.Product = product;
            return View(reviews);
        }

        // GET: Reviews/Create?productId=1&bookingId=8
        public async Task<IActionResult> Create(int productId, int bookingId)
        {
            // ดึง Product พร้อมกับดึง List ของ ProductImages มาด้วย (Eager Loading)
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return NotFound();

            ViewBag.Product = product;
            ViewBag.ProductId = productId;
            ViewBag.BookingId = bookingId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reviews review)
        {
            // 1. ดึง UserId จากคนที่ Login อยู่จริง
            string userIdStr = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdStr))
            {
                // แปลงจาก string "1" เป็น int 1
                review.UserId = int.Parse(userIdStr);
            }
            else
            {
                // กรณี Test ถ้ายังไม่ได้ทำระบบ Login ให้ใช้ 1 ไปก่อนได้ครับ
                review.UserId = 1;
            }

            review.CreatedAt = DateTime.Now;

            // 2. [Optional] เช็คว่าเคยรีวิวสินค้าตัวนี้ใน Booking นี้ไปหรือยัง
            var existingReview = await _context.Reviews
                .AnyAsync(r => r.BookingId == review.BookingId && r.ProductId == review.ProductId);

            if (existingReview)
            {
                ModelState.AddModelError("", "คุณได้ทำการรีวิวสินค้านี้สำหรับการจองนี้ไปเรียบร้อยแล้ว");
            }

            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // เมื่อรีวิวเสร็จ ส่งกลับไปหน้า Profile
                return RedirectToAction("Profile", "Account");
            }

            // ถ้าไม่ผ่าน Validation ให้กลับไปหน้าเดิมพร้อมโชว์ Error
            var product = await _context.Products.FindAsync(review.ProductId);
            ViewBag.Product = product;
            return View(review);
        }
    }
}