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
        private readonly HikeCycledbContext _db;

        public PromotionsController(HikeCycledbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetActivePromotions()
        {
            var promotions = await _db.Promotions
                .Where(p => p.Active)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Type,
                    p.Title,
                    p.Description,
                    Conditions = _db.PromotionConditions
                    .Where(c => c.PromotionId == p.Id)
                    .Select(c => new
                    {
                        Key = c.ConditionKey,    
                        Value = c.ConditionValue 
                    })
                    .ToList(),

                    Benefits = _db.PromotionBenefits
                    .Where(b => b.PromotionId == p.Id)
                    .Select(b => new
                    {
                        Key = b.BenefitKey,      
                        Value = b.BenefitValue   
                    })
                    .ToList()
                })
                .ToListAsync();

            return Ok(promotions);
        }
    }
}