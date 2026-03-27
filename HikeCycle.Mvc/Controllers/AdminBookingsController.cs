using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminBookingsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminBookingsController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User) // ดึงข้อมูลผู้ใช้งาน
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Product) // ดึงข้อมูลสินค้าในแต่ละรายการจอง
                .OrderBy(b => b.Id) // เอาการจองล่าสุดขึ้นก่อน
                .ToListAsync();

            return View(bookings);
        }

        // เพิ่ม Action สำหรับอัปเดตสถานะการจองแบบง่าย
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, ReturnCondition? condition, decimal? extraFee, string? note)
        {
            var booking = await _context.Bookings
        .Include(b => b.User) 
        .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return NotFound();

            if (status == "Completed")
            {
                // 1. อัปเดตสถานะ Booking
                booking.Status = "Completed";

                // 2. สร้างข้อมูลการคืน (Return)
                var returnEntry = new Return
                {
                    BookingId = id,
                    ReturnDate = DateTime.Now,
                    Condition = condition ?? ReturnCondition.Good,
                    ExtraFee = extraFee ?? 0,
                    IsExtraFeePaid = (extraFee ?? 0) <= 0, // ถ้าไม่มีค่าปรับ ให้ถือว่าจ่ายแล้ว
                    Note = note
                };
                _context.Returns.Add(returnEntry);
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
                    _context.UserVouchers.Add(voucher);

                    // แจ้ง Admin บนหน้าจอ
                    TempData["Success"] = $"คืนอุปกรณ์สำเร็จ! ระบบแจก Voucher 50 บาทให้คุณ {booking.User.Email} แล้ว";

                }
            }
            else if (status == "Cancelled")
            {
                booking.Status = "Cancelled";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}