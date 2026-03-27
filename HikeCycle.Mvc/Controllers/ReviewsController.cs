using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public ReviewsController(HikeCycledbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            var reviews = await (from r in _db.Reviews
                                 where r.ProductId == id
                                 join u in _db.UserProfiles on r.UserId equals u.UserId into userGroup
                                 from u in userGroup.DefaultIfEmpty() 
                                 orderby r.CreatedAt descending
                                 select new 
                                 {
                                     Id = r.Id,
                                     UserId = r.UserId,
                                     UserName = u != null ? u.FullName : "ไม่ระบุ", 
                                     Rating = r.Rating,
                                     Comment = r.Comment,
                                     CreatedAt = r.CreatedAt
                                 }).ToListAsync();

            ViewBag.Product = product;
            return View(reviews);
        }

        public async Task<IActionResult> Create(int productId, int bookingId)
        {
            var product = await _db.Products
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
            string userIdStr = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdStr))
            {
                review.UserId = int.Parse(userIdStr);
            }
            else
            {
                review.UserId = 1;
            }

            review.CreatedAt = DateTime.Now;

            var existingReview = await _db.Reviews
                .AnyAsync(r => r.BookingId == review.BookingId && r.ProductId == review.ProductId);

            if (existingReview)
            {
                ModelState.AddModelError("", "คุณได้ทำการรีวิวสินค้านี้สำหรับการจองนี้ไปเรียบร้อยแล้ว");
            }

            if (ModelState.IsValid)
            {
                _db.Reviews.Add(review);
                await _db.SaveChangesAsync();

                return RedirectToAction("Profile", "Account");
            }

            var product = await _db.Products.FindAsync(review.ProductId);
            ViewBag.Product = product;
            return View(review);
        }
    }
}