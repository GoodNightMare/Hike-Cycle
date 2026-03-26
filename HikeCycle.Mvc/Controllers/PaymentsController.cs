using Microsoft.AspNetCore.Mvc;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json; // 🚩 ต้องใช้ตัวนี้เพื่อจัดการ JSON
using System.Linq;
using System.Threading.Tasks;
using System.Globalization; // 🚩 เพิ่มบรรทัดนี้

namespace HikeCycle.Mvc.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public PaymentsController(HikeCycledbContext context)
        {
            _context = context;
        }

        public IActionResult Index(decimal originalTotal, decimal totalDiscount, decimal finalTotal, string shippingAddress)
        {
            var model = new PaymentViewModel
            {
                OriginalTotal = originalTotal,
                TotalDiscount = totalDiscount,
                Amount = finalTotal,
                ShippingAddress = shippingAddress
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account"); // User not logged in
            }
            int userId = int.Parse(userIdStr);

            var cartJson = HttpContext.Session.GetString("UserCart");
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
                    // --- ส่วนที่ต้องแก้ไข ---
                    var validCartItems = cartItems.Where(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate)).ToList();

                    if (!validCartItems.Any())
                    {
                        ModelState.AddModelError("", "ไม่พบรายการสินค้าที่ระบุวันที่จอง");
                        return View("Index", model);
                    }

                    var userProfile = await _context.UserProfiles.AsNoTracking()
                                        .FirstOrDefaultAsync(p => p.UserId == userId);

                    // 🚩 ใช้ ParseExact เพื่อให้อ่านรูปแบบ "2026-03-26" ได้ถูกต้อง
                    var minDate = validCartItems.Min(i => DateTime.ParseExact(i.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    var maxDate = validCartItems.Max(i => DateTime.ParseExact(i.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    // ----------------------

                    var newBooking = new Booking
                    {
                        UserId = userId,
                        StartDate = minDate,
                        EndDate = maxDate,
                        TotalAmount = model.OriginalTotal,
                        DiscountAmount = model.TotalDiscount,
                        FinalAmount = model.Amount,
                        Status = "Confirmed",
                        ShippingAddress = model.ShippingAddress,
                        CreatedAt = DateTime.Now
                    };
                    _context.Bookings.Add(newBooking);
                    await _context.SaveChangesAsync();

                    foreach (var item in cartItems)
                    {
                        var bookingItem = new BookingItem
                        {
                            BookingId = newBooking.Id,
                            ProductId = item.ProductId,
                            Size = item.Size,
                            Quantity = 1, // Each cart item is one quantity
                            PricePerDay = item.PricePerDay,
                            IsFree = item.IsFree
                        };

                        if (!item.IsFree)
                        {
                            var itemStartDate = DateTime.ParseExact(item.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            var itemEndDate = DateTime.ParseExact(item.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            var itemDays = (itemEndDate - itemStartDate).Days;
                            if (itemDays <= 0) itemDays = 1;
                            bookingItem.ItemTotal = item.PricePerDay * itemDays;
                        }
                        else
                        {
                            bookingItem.ItemTotal = 0;
                        }

                        _context.BookingItems.Add(bookingItem);

                        // Update Stock
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            try
                            {
                                if (product.Category?.ToLower() == "shoes" && !string.IsNullOrEmpty(product.Variants))
                                {
                                    // 🚩 ใช้ JsonDocument เพื่อความยืดหยุ่นในการอ่านค่า "size" ที่เป็นได้ทั้ง Int และ String
                                    using (JsonDocument doc = JsonDocument.Parse(product.Variants))
                                    {
                                        var updatedVariants = new List<object>();
                                        bool isFound = false;

                                        foreach (var v in doc.RootElement.EnumerateArray())
                                        {
                                            // อ่านค่าจาก JSON เดิม
                                            string vSize = v.GetProperty("size").ToString();
                                            int vStock = v.GetProperty("stock").GetInt32();

                                            // ถ้าเจอ Size ที่ตรงกัน ให้ลดสต็อก
                                            if (!isFound && vSize == item.Size)
                                            {
                                                vStock = Math.Max(0, vStock - 1);
                                                isFound = true;
                                            }

                                            // เก็บค่ากลับเข้า List โดยรักษา Data Type เดิมของ size ไว้
                                            updatedVariants.Add(new
                                            {
                                                size = v.GetProperty("size").ValueKind == JsonValueKind.Number ? (object)v.GetProperty("size").GetInt32() : vSize,
                                                stock = vStock
                                            });
                                        }
                                        // Serialize กลับเป็น JSON string ลงฐานข้อมูล
                                        product.Variants = JsonSerializer.Serialize(updatedVariants);
                                    }
                                }

                                // ลดสต็อกรวมของสินค้าทุกประเภทด้วย
                                product.Stock = Math.Max(0, (product.Stock ?? 1) - 1);
                            }
                            catch (Exception ex)
                            {
                                // ถ้า JSON พัง ให้ลดแค่สต็อกหลักและปล่อยผ่าน เพื่อให้ Transaction ไม่ล่ม
                                System.Diagnostics.Debug.WriteLine("Stock Update Error: " + ex.Message);
                                product.Stock = Math.Max(0, (product.Stock ?? 1) - 1);
                            }
                        }
                    }

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

                    HttpContext.Session.Remove("UserCart");

                    // Redirect to a success page, passing the new booking ID
                    return RedirectToAction("Success", "Bookings", new { id = newBooking.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred during the checkout process: " + ex.Message);
                    return View("Index", model);
                }
            }
        }
    }
}