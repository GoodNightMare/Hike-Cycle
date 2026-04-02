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
        public async Task<IActionResult> Index(bool isStudent = false, int? selectedPromotionId = null, string? selectedVoucherCode = null)
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

            var allActivePromotions = await _db.Promotions.Where(p => p.Active).ToListAsync();
            var applicablePromotions = new List<Promotion>();

            var longTripPromo = allActivePromotions.FirstOrDefault(p => p.Title == "Long Trip Deal");
            if (longTripPromo != null && cartItems.Any(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate) && !string.IsNullOrEmpty(i.EndDate) && (DateTime.Parse(i.EndDate) - DateTime.Parse(i.StartDate)).Days > 5))
            {
                applicablePromotions.Add(longTripPromo);
            }

            var earlyBirdPromo = allActivePromotions.FirstOrDefault(p => p.Title == "Early Bird Hiker");
            if (earlyBirdPromo != null && cartItems.Any(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate) && (DateTime.Parse(i.StartDate) - DateTime.Now).TotalDays >= 30))
            {
                applicablePromotions.Add(earlyBirdPromo);
            }

            var merrierPromo = allActivePromotions.FirstOrDefault(p => p.Title == "The more The Merrier");
            if (merrierPromo != null && cartItems.Count(i => !i.IsFree && i.Category?.ToLower() == "tent") >= 2)
            {
                applicablePromotions.Add(merrierPromo);
            }
            
            if (isStudent)
            {
                var studentPromo = allActivePromotions.FirstOrDefault(p => p.Title == "Student Explorer");
                if (studentPromo != null)
                {
                    applicablePromotions.Add(studentPromo);
                }
            }

            var calculationResult = await CalculateCart(cartItems, allActivePromotions, isStudent, selectedPromotionId, selectedVoucherCode);

            var viewModel = new CartViewModel
            {
                CartItems = cartItems,
                Promotions = applicablePromotions,
                CalculationResult = calculationResult,
                IsStudent = isStudent,
                AvailableVouchers = availableVouchers,
                SelectedPromotionId = selectedPromotionId,
                SelectedVoucherCode = selectedVoucherCode
            };

            if (!string.IsNullOrEmpty(userIdStr))
            {
                var profile = await _db.UserProfiles.AsNoTracking()
                                .FirstOrDefaultAsync(p => p.UserId == int.Parse(userIdStr));
                ViewBag.UserAddress = profile?.Address;
            }

            return View(viewModel);
        }

        private async Task<CartCalculationResult> CalculateCart(List<CartSessionItem> cartItems, List<Promotion> activePromotions, bool isStudent, int? selectedPromotionId, string? selectedVoucherCode)
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
                originalTotal += itemOriginalPrice;
            }

            if (selectedPromotionId.HasValue)
            {
                var selectedPromotion = activePromotions.FirstOrDefault(p => p.Id == selectedPromotionId.Value);
                if (selectedPromotion != null)
                {
                    decimal promotionDiscount = 0;
                    switch (selectedPromotion.Title)
                    {
                        case "Long Trip Deal":
                            foreach (var item in cartItems.Where(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate) && !string.IsNullOrEmpty(i.EndDate)))
                            {
                                var start = DateTime.Parse(item.StartDate);
                                var end = DateTime.Parse(item.EndDate);
                                int totalDays = (end - start).Days;
                                if (totalDays > 5)
                                {
                                    promotionDiscount += (totalDays - 5) * (item.PricePerDay / 2);
                                }
                            }
                            break;
                        case "Early Bird Hiker":
                            decimal earlyBirdEligibleTotal = 0;
                            foreach (var item in cartItems.Where(i => !i.IsFree && !string.IsNullOrEmpty(i.StartDate) && !string.IsNullOrEmpty(i.EndDate)))
                            {
                                var start = DateTime.Parse(item.StartDate);
                                if ((start - DateTime.Now).TotalDays >= 30)
                                {
                                    var end = DateTime.Parse(item.EndDate);
                                    int totalDays = (end - start).Days;
                                    if (totalDays <= 0) totalDays = 1;
                                    earlyBirdEligibleTotal += item.PricePerDay * totalDays;
                                }
                            }
                            promotionDiscount = earlyBirdEligibleTotal * 0.20m;
                            break;
                        case "Student Explorer":
                            if (isStudent)
                            {
                                promotionDiscount = originalTotal * 0.10m;
                            }
                            break;
                        case "The more The Merrier":
                            if (cartItems.Count(i => i.Category?.ToLower() == "tent") >= 2)
                            {
                                // No direct discount, benefit is free items added elsewhere.
                                // Just add to applied promotions list.
                            }
                            break;
                    }

                    if (promotionDiscount > 0)
                    {
                        totalDiscount += promotionDiscount;
                        appliedPromotions.Add(new AppliedPromotion { Title = selectedPromotion.Title, Description = selectedPromotion.Description });
                    }
                    
                    if (selectedPromotion.Title == "The more The Merrier" && cartItems.Count(i => i.Category?.ToLower() == "tent") >= 2)
                    {
                        appliedPromotions.Add(new AppliedPromotion { Title = selectedPromotion.Title, Description = selectedPromotion.Description });
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(selectedVoucherCode))
            {
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    int userId = int.Parse(userIdStr);
                    var voucher = await _db.UserVouchers
                        .FirstOrDefaultAsync(v => v.UserId == userId && v.Code == selectedVoucherCode && !v.IsUsed);

                    if (voucher != null)
                    {
                        totalDiscount += voucher.Amount;
                        appliedPromotions.Add(new AppliedPromotion { Title = "Voucher", Description = $"ส่วนลด {voucher.Amount} บาท" });
                    }
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
