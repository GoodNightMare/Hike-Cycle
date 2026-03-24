using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Models.Dto; // Import the DTO namespace

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
                                 select new ReviewDto
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
    }
}