using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminPromotionsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminPromotionsController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var promotions = await _db.Promotions
                                   .OrderBy(p => p.Id)
                                   .ToListAsync();
            return View(promotions);
        }
    }
}