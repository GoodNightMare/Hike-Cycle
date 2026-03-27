using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminReviewsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminReviewsController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                                .OrderBy(r => r.Id)
                                .ToListAsync();

            ViewBag.Users = await _context.Users.ToDictionaryAsync(u => u.Id, u => u.Email);
            ViewBag.Products = await _context.Products.ToDictionaryAsync(p => p.Id, p => p.Name);
            return View(reviews);
        }
    }
}