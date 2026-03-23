using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HikeCycle.Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionsController : ControllerBase
    {
        private readonly HikeCycledbContext _context;

        public PromotionsController(HikeCycledbContext context)
        {
            _context = context;
        }

        // GET: api/promotions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetActivePromotions()
        {
            // ดึงข้อมูลโปรโมชันที่ Active พร้อม Join ข้อมูลจากตารางลูก
            var promotions = await _context.Promotions
                .Where(p => p.Active)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Type,
                    p.Title,
                    p.Description,
                    // ดึง Conditions ที่เกี่ยวข้อง
                    Conditions = _context.PromotionConditions
                    .Where(c => c.PromotionId == p.Id)
                    .Select(c => new
                    {
                        Key = c.ConditionKey,    // ✅ เปลี่ยนจาก c.Key เป็น c.ConditionKey
                        Value = c.ConditionValue // ✅ เปลี่ยนจาก c.Value เป็น c.ConditionValue
                    })
                    .ToList(),

                                    // ดึง Benefits ที่เกี่ยวข้อง
                    Benefits = _context.PromotionBenefits
                    .Where(b => b.PromotionId == p.Id)
                    .Select(b => new
                    {
                        Key = b.BenefitKey,      // ✅ เปลี่ยนจาก b.Key เป็น b.BenefitKey
                        Value = b.BenefitValue   // ✅ เปลี่ยนจาก b.Value เป็น b.BenefitValue
                    })
                    .ToList()
                })
                .ToListAsync();

            return Ok(promotions);
        }
    }
}