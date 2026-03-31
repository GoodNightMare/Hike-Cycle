using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.ViewModels;
using HikeCycle.Mvc.Models.db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HikeCycle.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly HikeCycledbContext _db;
        private const string CartSessionKey = "UserCart";

        public CartController(HikeCycledbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int ProductId, string StartDate, string EndDate, string? Size)
        {
            if (DateTime.Parse(StartDate) > DateTime.Parse(EndDate))
            {
                TempData["ErrorMessage"] = "วันที่สิ้นสุดต้องไม่ก่อนวันที่เริ่มต้น";
                return RedirectToAction("Details", "Products", new { id = ProductId });
            }

            var product = await _db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == ProductId);

            if (product == null) return NotFound();

            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            var cart = string.IsNullOrEmpty(sessionData)
                ? new List<CartSessionItem>()
                : JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData) ?? new List<CartSessionItem>();

            int availableStock = product.Stock ?? 0;

            if (product.Category?.ToLower() == "shoes" && !string.IsNullOrEmpty(product.Variants) && !string.IsNullOrEmpty(Size))
            {
                using (var doc = JsonDocument.Parse(product.Variants))
                {
                    var variant = doc.RootElement.EnumerateArray()
                        .FirstOrDefault(v => v.GetProperty("size").GetRawText().Replace("\"", "") == Size);

                    if (variant.ValueKind != JsonValueKind.Undefined)
                        availableStock = variant.GetProperty("stock").GetInt32();
                    else
                        availableStock = 0;
                }
            }

            var amountInCart = cart.Count(i =>
                i.ProductId == ProductId &&
                (product.Category?.ToLower() != "shoes" || i.Size == Size)
            );

            if (amountInCart + 1 > availableStock)
            {
                TempData["ErrorMessage"] = $"ไม่สามารถเพิ่มได้ เนื่องจากสต็อกสินค้า (รวมในตะกร้า) มีเพียง {availableStock} ชิ้น";
                return RedirectToAction("Details", "Products", new { id = ProductId });
            }

            cart.Add(new CartSessionItem
            {
                ProductId = ProductId,
                ProductName = product.Name,
                ImageUrl = product.ProductImages.FirstOrDefault()?.ImageUrl,
                PricePerDay = product.PricePerDay ?? 0,
                Category = product.Category,
                Size = Size,
                StartDate = StartDate,
                EndDate = EndDate,
                Id = Guid.NewGuid().ToString()
            });

            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
            TempData["SuccessMessage"] = "เพิ่มสินค้าลงในตะกร้าแล้ว";
            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool isStudent = false)
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            var cartItems = string.IsNullOrEmpty(sessionData)
                ? new List<CartSessionItem>()
                : JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData) ?? new List<CartSessionItem>();

            cartItems.RemoveAll(i => !i.IsRemovable);

            if (cartItems.Count(i => i.Category?.ToLower() == "tent") >= 2)
            {
                var productIdsToAdd = new List<int> { 3, 5 };
                var productsToAdd = await _db.Products
                    .Include(p => p.ProductImages)
                    .Where(p => productIdsToAdd.Contains(p.Id))
                    .ToListAsync();

                foreach (var product in productsToAdd)
                {
                    if (!cartItems.Any(ci => ci.ProductId == product.Id))
                    {
                        cartItems.Add(new CartSessionItem
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            ImageUrl = product.ProductImages.FirstOrDefault()?.ImageUrl,
                            PricePerDay = 0,
                            Category = product.Category,
                            IsFree = true,
                            IsRemovable = false,
                            StartDate = "", 
                            EndDate = "",   
                            Id = Guid.NewGuid().ToString()
                        });
                    }
                }
            }

            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));

            var userIdStr = HttpContext.Session.GetString("UserId");
            List<UserVoucher> availableVouchers = new List<UserVoucher>();

            if (!string.IsNullOrEmpty(userIdStr))
            {
                int userId = int.Parse(userIdStr);

                availableVouchers = await _db.UserVouchers
                    .Where(v => v.UserId == userId && !v.IsUsed)
                    .ToListAsync();

                var profile = await _db.UserProfiles.AsNoTracking()
                                .FirstOrDefaultAsync(p => p.UserId == userId);
                ViewBag.UserAddress = profile?.Address;
            }

            var promotions = await _db.Promotions.Where(p => p.Active).ToListAsync();

            var calculationResult = CalculateCart(cartItems, promotions, isStudent);

            var viewModel = new CartViewModel
            {
                CartItems = cartItems,
                Promotions = promotions,
                CalculationResult = calculationResult,
                IsStudent = isStudent,
                AvailableVouchers = availableVouchers
            };
            
            if (!string.IsNullOrEmpty(userIdStr))
            {
                var profile = await _db.UserProfiles.AsNoTracking()
                                .FirstOrDefaultAsync(p => p.UserId == int.Parse(userIdStr));
                ViewBag.UserAddress = profile?.Address;
            }

            return View(viewModel);
        }

        private CartCalculationResult CalculateCart(List<CartSessionItem> cartItems, List<Promotion> activePromotions, bool isStudent)
        {
            decimal originalTotal = 0;
            decimal totalDiscount = 0;
            var appliedPromotions = new List<AppliedPromotion>();

            foreach (var item in cartItems)
            {
                if (item.IsFree || string.IsNullOrEmpty(item.StartDate) || string.IsNullOrEmpty(item.EndDate))
                {
                    continue; 
                }

                var start = DateTime.Parse(item.StartDate);
                var end = DateTime.Parse(item.EndDate);
                int totalDays = (end - start).Days;
                if (totalDays <= 0) totalDays = 1;

                decimal itemOriginalPrice = item.PricePerDay * totalDays;
                decimal itemDiscount = 0;

                var longTripDeal = activePromotions.FirstOrDefault(p => p.Title == "Long Trip Deal");
                if (longTripDeal != null && totalDays > 5)
                {
                    var discount = (totalDays - 5) * (item.PricePerDay / 2);
                    itemDiscount += discount;
                    if (!appliedPromotions.Any(p => p.Title == longTripDeal.Title))
                    {
                        appliedPromotions.Add(new AppliedPromotion { Title = longTripDeal.Title, Description = longTripDeal.Description });
                    }
                }

                var earlyBirdHiker = activePromotions.FirstOrDefault(p => p.Title == "Early Bird Hiker");
                if (earlyBirdHiker != null && (start - DateTime.Now).TotalDays >= 30)
                {
                    var discount = (itemOriginalPrice - itemDiscount) * 0.20m;
                    itemDiscount += discount;
                    if (!appliedPromotions.Any(p => p.Title == earlyBirdHiker.Title))
                    {
                        appliedPromotions.Add(new AppliedPromotion { Title = earlyBirdHiker.Title, Description = earlyBirdHiker.Description });
                    }
                }

                originalTotal += itemOriginalPrice;
                totalDiscount += itemDiscount;
            }

            decimal subTotal = originalTotal - totalDiscount;

            var studentExplorer = activePromotions.FirstOrDefault(p => p.Title == "Student Explorer");
            if (studentExplorer != null && isStudent)
            {
                var studentDiscount = subTotal * 0.10m;
                totalDiscount += studentDiscount;
                if (!appliedPromotions.Any(p => p.Title == studentExplorer.Title))
                {
                    appliedPromotions.Add(new AppliedPromotion { Title = studentExplorer.Title, Description = studentExplorer.Description });
                }
            }

            var moreTheMerrier = activePromotions.FirstOrDefault(p => p.Title == "The more The Merrier");
            if (moreTheMerrier != null && cartItems.Count(i => i.Category?.ToLower() == "tent") >= 2)
            {
                if (!appliedPromotions.Any(p => p.Title == moreTheMerrier.Title))
                {
                    appliedPromotions.Add(new AppliedPromotion { Title = moreTheMerrier.Title, Description = moreTheMerrier.Description });
                }
            }

            return new CartCalculationResult
            {
                OriginalTotal = originalTotal,
                TotalDiscount = totalDiscount,
                FinalTotal = originalTotal - totalDiscount,
                AppliedPromotions = appliedPromotions
            };
        }

        [HttpPost]
        public IActionResult UpdateDate(string id, string startDate, string endDate)
        {
            if (DateTime.Parse(startDate) > DateTime.Parse(endDate))
            {
                TempData["ErrorMessage"] = "วันที่สิ้นสุดต้องไม่ก่อนวันที่เริ่มต้น";
                return RedirectToAction("Index");
            }

            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Index");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData);
            var itemToUpdate = cartItems.FirstOrDefault(item => item.Id == id);

            if (itemToUpdate != null)
            {
                itemToUpdate.StartDate = startDate;
                itemToUpdate.EndDate = endDate;
                HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(string id)
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Index");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData);
            var itemToRemove = cartItems.FirstOrDefault(item => item.Id == id);

            if (itemToRemove != null && itemToRemove.IsRemovable)
            {
                cartItems.Remove(itemToRemove);
                HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(string id)
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                return RedirectToAction("Index");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartSessionItem>>(sessionData);
            var existingItem = cartItems.FirstOrDefault(item => item.Id == id);

            if (existingItem == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _db.Products.FindAsync(existingItem.ProductId);
            if (product == null) return NotFound();

            int availableStock = product.Stock ?? 0;

            if (product.Category?.ToLower() == "shoes" && !string.IsNullOrEmpty(product.Variants) && !string.IsNullOrEmpty(existingItem.Size))
            {
                using (var doc = JsonDocument.Parse(product.Variants))
                {
                    var variant = doc.RootElement.EnumerateArray()
                        .FirstOrDefault(v => v.GetProperty("size").GetRawText().Replace("\"", "") == existingItem.Size);

                    if (variant.ValueKind != JsonValueKind.Undefined)
                        availableStock = variant.GetProperty("stock").GetInt32();
                    else
                        availableStock = 0;
                }
            }

            var amountInCart = cartItems.Count(i =>
                i.ProductId == existingItem.ProductId &&
                (product.Category?.ToLower() != "shoes" || i.Size == existingItem.Size)
            );

            if (amountInCart + 1 > availableStock)
            {
                TempData["ErrorMessage"] = $"ไม่สามารถเพิ่มได้ เนื่องจากสต็อกสินค้า (รวมในตะกร้า) มีเพียง {availableStock} ชิ้น";
                return RedirectToAction("Index");
            }

            cartItems.Add(new CartSessionItem
            {
                ProductId = existingItem.ProductId,
                ProductName = existingItem.ProductName,
                ImageUrl = existingItem.ImageUrl,
                PricePerDay = existingItem.PricePerDay,
                Category = existingItem.Category,
                Size = existingItem.Size,
                StartDate = existingItem.StartDate,
                EndDate = existingItem.EndDate,
                Id = Guid.NewGuid().ToString() 
            });

            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cartItems));

            return RedirectToAction("Index");
        }
    }
}
