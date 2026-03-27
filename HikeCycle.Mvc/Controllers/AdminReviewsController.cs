using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminReviewsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminReviewsController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var reviews = await _db.Reviews
                                .OrderBy(r => r.Id)
                                .ToListAsync();

            ViewBag.Users = await _db.Users.ToDictionaryAsync(u => u.Id, u => u.Email);
            ViewBag.Products = await _db.Products.ToDictionaryAsync(p => p.Id, p => p.Name);
            return View(reviews);
        }
    }
}