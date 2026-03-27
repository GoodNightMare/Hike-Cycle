using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminRoutesController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminRoutesController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var recommendedRoutes = await _context.RecommendedRoutes.ToListAsync();
            return View(recommendedRoutes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoute([FromBody] RecommendedRoute route)
        {
            if (ModelState.IsValid)
            {
                _context.RecommendedRoutes.Add(route);
                await _context.SaveChangesAsync();
                return Json(route);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoute([FromBody] RecommendedRoute route)
        {
            if (ModelState.IsValid)
            {
                // ใช้ Update จะช่วยลดปัญหาเรื่อง Tracking ของ Entity Framework ได้ดีกว่าในบางกรณี
                _context.RecommendedRoutes.Update(route);

                try
                {
                    await _context.SaveChangesAsync();
                    // 🚩 แก้ไข: ส่ง route กลับไปเพื่อให้ JavaScript อัปเดตตัวหนังสือในตารางทันที
                    return Json(route);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.RecommendedRoutes.Any(e => e.Id == route.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.RecommendedRoutes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            _context.RecommendedRoutes.Remove(route);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}