using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendedRoutesController : ControllerBase
    {
        private readonly HikeCycledbContext _db;

        public RecommendedRoutesController(HikeCycledbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecommendedRoute>>> GetRoutes()
        {
            try
            {
                var routes = await _db.RecommendedRoutes
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Id)
                    .ToListAsync();

                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "เกิดข้อผิดพลาดในการดึงข้อมูลเส้นทาง", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecommendedRoute>> GetRoute(int id)
        {
            var route = await _db.RecommendedRoutes.FindAsync(id);

            if (route == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลเส้นทางนี้" });
            }

            return Ok(route);
        }
    }
}