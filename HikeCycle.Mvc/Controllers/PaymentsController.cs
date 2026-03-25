using Microsoft.AspNetCore.Mvc;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Models.Dto;
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

        public IActionResult Index(decimal originalTotal, decimal totalDiscount, decimal finalTotal)
        {
            var model = new PaymentViewModel { 
                OriginalTotal = originalTotal,
                TotalDiscount = totalDiscount,
                Amount = finalTotal
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
                    var validCartItems = cartItems.Where(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate)).ToList();
                    var minDate = validCartItems.Min(i => DateTime.Parse(i.StartDate));
                    var maxDate = validCartItems.Max(i => DateTime.Parse(i.EndDate));

                    var newBooking = new Booking
                    {
                        UserId = userId,
                        StartDate = minDate,
                        EndDate = maxDate,
                        TotalAmount = model.OriginalTotal,
                        DiscountAmount = model.TotalDiscount,
                        FinalAmount = model.Amount,
                        Status = "Confirmed",
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
                            var itemStartDate = DateTime.Parse(item.StartDate);
                            var itemEndDate = DateTime.Parse(item.EndDate);
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
                            if (product.Category?.ToLower() == "shoes" && !string.IsNullOrEmpty(item.Size) && !string.IsNullOrEmpty(product.Variants))
                            {
                                var variants = JsonSerializer.Deserialize<List<ProductVariantDto>>(product.Variants) ?? new List<ProductVariantDto>();
                                var variant = variants.FirstOrDefault(v => v.Size == item.Size);
                                if (variant != null)
                                {
                                    variant.Stock -= 1; // Decrease stock by 1
                                }
                                product.Variants = JsonSerializer.Serialize(variants);
                            }
                            else
                            {
                                product.Stock = (product.Stock ?? 1) - 1; // Decrease stock by 1
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