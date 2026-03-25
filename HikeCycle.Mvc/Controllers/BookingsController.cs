using Microsoft.AspNetCore.Mvc;
using HikeCycle.Mvc.Models.db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HikeCycle.Mvc.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public BookingsController(HikeCycledbContext context)
        {
            _context = context;
        }

        // GET: Bookings/Success/5
        public async Task<IActionResult> Success(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
    }
}
