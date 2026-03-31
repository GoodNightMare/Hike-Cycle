using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminBookingsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminBookingsController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var bookings = await _db.Bookings
                .Include(b => b.User)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Product)
                .OrderBy(b => b.Id)
                .ToListAsync();

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, ReturnCondition? condition, decimal? extraFee, string? note)
        {
            var booking = await _db.Bookings
        .Include(b => b.User)
        .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return NotFound();

            if (status == "Completed")
            {
                booking.Status = "Completed";

                var returnEntry = new Return
                {
                    BookingId = id,
                    ReturnDate = DateTime.Now,
                    Condition = condition ?? ReturnCondition.Good,
                    ExtraFee = extraFee ?? 0,
                    IsExtraFeePaid = (extraFee ?? 0) <= 0,
                    Note = note
                };
                _db.Returns.Add(returnEntry);
                if (condition == ReturnCondition.Good)
                {
                    var voucher = new UserVoucher
                    {
                        UserId = booking.UserId,
                        PromotionId = 4,
                        Code = "CLEAN-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                        Amount = 50,
                        IsUsed = false,
                        CreatedAt = DateTime.Now
                    };
                    _db.UserVouchers.Add(voucher);

                    TempData["Success"] = $"คืนอุปกรณ์สำเร็จ! ระบบแจก Voucher 50 บาทให้คุณ {booking.User.Email} แล้ว";

                }
            }
            else if (status == "Cancelled")
            {
                booking.Status = "Cancelled";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}