using Microsoft.AspNetCore.Mvc;
using HikeCycle.Mvc.Models.db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HikeCycle.Mvc.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public BookingsController(HikeCycledbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Success(int? id, decimal? voucherDiscount)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _db.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            ViewBag.VoucherDiscount = voucherDiscount ?? 0;

            return View(booking);
        }
    }
}
