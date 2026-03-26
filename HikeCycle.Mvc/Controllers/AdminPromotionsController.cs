using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.Controllers
{
    public class AdminPromotionsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminPromotionsController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions
                                   .OrderBy(p => p.Id)
                                   .ToListAsync();
            return View(promotions);
        }
    }
}