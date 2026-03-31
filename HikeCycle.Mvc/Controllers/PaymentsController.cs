using Microsoft.AspNetCore.Mvc;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace HikeCycle.Mvc.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public PaymentsController(HikeCycledbContext db)
        {
            _db = db;
        }

        public IActionResult Index(decimal originalTotal, decimal totalDiscount, decimal finalTotal, string shippingAddress, string? voucherCode, decimal? voucherDiscount)
        {
            decimal vDiscount = voucherDiscount ?? 0;
            decimal combinedDiscount = totalDiscount + vDiscount;

            decimal actualAmount = finalTotal;
            if(actualAmount < 0) actualAmount = 0;

            var model = new PaymentViewModel
            {
                OriginalTotal = originalTotal,
                TotalDiscount = combinedDiscount,
                Amount = actualAmount,
                ShippingAddress = shippingAddress,
                VoucherCode = voucherCode,
                VoucherDiscount = vDiscount
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account"); 
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

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var validCartItems = cartItems.Where(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate)).ToList();

                    if (!validCartItems.Any())
                    {
                        ModelState.AddModelError("", "ไม่พบรายการสินค้าที่ระบุวันที่จอง");
                        return View("Index", model);
                    }

                    var userProfile = await _db.UserProfiles.AsNoTracking()
                                        .FirstOrDefaultAsync(p => p.UserId == userId);

                    var minDate = validCartItems.Min(i => DateTime.ParseExact(i.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    var maxDate = validCartItems.Max(i => DateTime.ParseExact(i.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));

                    decimal finalAmount = model.Amount;
                    if (finalAmount < 0) finalAmount = 0; 

                    var newBooking = new Booking
                    {
                        UserId = userId,
                        StartDate = minDate,
                        EndDate = maxDate,
                        TotalAmount = model.OriginalTotal,
                        DiscountAmount = model.TotalDiscount,
                        FinalAmount = finalAmount,
                        Status = "Confirmed",
                        ShippingAddress = model.ShippingAddress,
                        CreatedAt = DateTime.Now
                    };
                    _db.Bookings.Add(newBooking);
                    await _db.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(model.VoucherCode))
                    {
                        var voucher = await _db.UserVouchers
                            .FirstOrDefaultAsync(v => v.Code == model.VoucherCode && v.UserId == userId && !v.IsUsed);

                        if (voucher != null)
                        {
                            voucher.IsUsed = true; 
                            _db.UserVouchers.Update(voucher);
                        }
                    }

                    foreach (var item in cartItems)
                    {
                        var bookingItem = new BookingItem
                        {
                            BookingId = newBooking.Id,
                            ProductId = item.ProductId,
                            Size = item.Size,
                            Quantity = 1, 
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

                        _db.BookingItems.Add(bookingItem);

                        var product = await _db.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            try
                            {
                                if (product.Category?.ToLower() == "shoes" && !string.IsNullOrEmpty(product.Variants))
                                {
                                    using (JsonDocument doc = JsonDocument.Parse(product.Variants))
                                    {
                                        var updatedVariants = new List<object>();
                                        bool isFound = false;

                                        foreach (var v in doc.RootElement.EnumerateArray())
                                        {
                                            string vSize = v.GetProperty("size").ToString();
                                            int vStock = v.GetProperty("stock").GetInt32();

                                            if (!isFound && vSize == item.Size)
                                            {
                                                vStock = Math.Max(0, vStock - 1);
                                                isFound = true;
                                            }

                                            updatedVariants.Add(new
                                            {
                                                size = v.GetProperty("size").ValueKind == JsonValueKind.Number ? (object)v.GetProperty("size").GetInt32() : vSize,
                                                stock = vStock
                                            });
                                        }
                                        product.Variants = JsonSerializer.Serialize(updatedVariants);
                                    }
                                }

                                product.Stock = Math.Max(0, (product.Stock ?? 1) - 1);
                            }
                            catch (Exception ex)
                            {
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
                    _db.Payments.Add(payment);

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    HttpContext.Session.Remove("UserCart");

                    return RedirectToAction("Success", "Bookings", new { id = newBooking.Id, voucherDiscount = model.VoucherDiscount });
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