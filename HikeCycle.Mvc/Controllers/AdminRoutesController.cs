using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminRoutesController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminRoutesController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var recommendedRoutes = await _db.RecommendedRoutes.ToListAsync();
            return View(recommendedRoutes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] RecommendedRoute route)
        {
            if (ModelState.IsValid)
            {
                _db.RecommendedRoutes.Add(route);
                await _db.SaveChangesAsync();
                return Json(route);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoute([FromBody] RecommendedRoute route)
        {
            if (ModelState.IsValid)
            {
                _db.RecommendedRoutes.Update(route);

                try
                {
                    await _db.SaveChangesAsync();
                    return Json(route);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.RecommendedRoutes.Any(e => e.Id == route.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _db.RecommendedRoutes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            _db.RecommendedRoutes.Remove(route);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}