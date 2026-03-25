using Microsoft.AspNetCore.Mvc;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using Microsoft.EntityFrameworkCore;
using System.Text.Json; // 🚩 ต้องใช้ตัวนี้เพื่อจัดการ JSON
using System.Linq;
using System.Threading.Tasks;

namespace HikeCycle.Mvc.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public PaymentsController(HikeCycledbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ดึงค่า String จาก Session ตรงๆ
            var cartJson = HttpContext.Session.GetString("UserCart");
            var cartItems = string.IsNullOrEmpty(cartJson) 
                ? new List<CartSessionItem>() 
                : JsonSerializer.Deserialize<List<CartSessionItem>>(cartJson);

            decimal originalTotal = 0;
    if (cartItems != null)
    {
        foreach (var item in cartItems)
        {
            if (item.IsFree) continue;
            var start = DateTime.Parse(item.StartDate);
            var end = DateTime.Parse(item.EndDate);
            int days = (end - start).Days;
            if (days <= 0) days = 1;
            originalTotal += item.PricePerDay * days;
        }
    }

    // สมมติว่าดึงค่าส่วนลดมา (หรือถ้าคุณเก็บก้อน CalculationResult ไว้ใน Session ก็ดึงมาใช้ได้เลย)
    // ในที่นี้ผมจะคำนวณแบบง่ายๆ หรือถ้าคุณมี Service คำนวณให้เรียกใช้ตรงนี้ครับ
    decimal finalTotal = originalTotal; 
    // ตัวอย่าง: ถ้าเป็นนักศึกษาลด 10% (ตาม Logic โปรเจกต์คุณ)
    // finalTotal = originalTotal * 0.9m; 

    var model = new PaymentViewModel { 
        Amount = finalTotal // 🚩 ส่งค่าที่คำนวณสุทธิแล้วไป
    };
    return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            // 1. ดึงข้อมูลจากตะกร้าแบบ Manual
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return RedirectToAction("Index", "Cart");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartSessionItem>>(cartJson);

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 2. สร้าง Booking
                    var newBooking = new Booking
                    {
                        UserId = 1, // สมมติ User ID
                        StartDate = DateTime.Parse(cartItems.First().StartDate),
                        EndDate = DateTime.Parse(cartItems.First().EndDate),
                        TotalAmount = model.Amount,
                        FinalAmount = model.Amount,
                        Status = "Confirmed",
                        CreatedAt = DateTime.Now
                    };
                    _context.Bookings.Add(newBooking);
                    await _context.SaveChangesAsync(); 

                    // 3. สร้าง BookingItems
                    foreach (var item in cartItems)
                    {
                        var bookingItem = new BookingItem
                        {
                            BookingId = newBooking.Id,
                            ProductId = item.ProductId, 
                            Size = item.Size,
                            Quantity = 1,
                            PricePerDay = item.PricePerDay,
                            ItemTotal = item.PricePerDay * (newBooking.EndDate - newBooking.StartDate).Days,
                            IsFree = item.IsFree
                        };
                        _context.BookingItems.Add(bookingItem);
                    }

                    // 4. สร้าง Payment
                    var payment = new Payment
                    {
                        BookingId = newBooking.Id,
                        Amount = model.Amount,
                        Method = model.Method == "Bank" ? PaymentMethod.Bank : PaymentMethod.PromptPay,
                        Status = PaymentStatus.Paid,
                        CreatedAt = DateTime.Now
                    };
                    _context.Payments.Add(payment);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync(); 

                    // ล้าง Session
                    HttpContext.Session.Remove("Cart");

                    return RedirectToAction("Success", new { id = newBooking.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "เกิดข้อผิดพลาด: " + ex.Message);
                    return View("Index", model);
                }
            }
        }
    }
}