using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminProductsController : Controller
    {
        private readonly HikeCycledbContext _db;

        public AdminProductsController(HikeCycledbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                                         .Include(p => p.ProductImages)
                                         .OrderBy(p => p.Id)
                                         .ToListAsync();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProduct(Product model, IFormFileCollection? images)
        {
            if (!ModelState.IsValid)
            {
                var products = await _db.Products.Include(p => p.ProductImages).ToListAsync();
                return View("Index", products);
            }

            if (model.Id == 0)
            {
                model.CreatedAt = DateTime.Now;
                _db.Products.Add(model);
            }
            else
            {
                _db.Products.Update(model);
                _db.Entry(model).Property(x => x.CreatedAt).IsModified = false;
            }

            await _db.SaveChangesAsync();

            if (images != null && images.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _db.ProductImages.Add(new ProductImage
                        {
                            ProductId = model.Id,
                            ImageUrl = "/uploads/products/" + fileName
                        });
                    }
                }
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = model.Id == 0 ? "เพิ่มสินค้าสำเร็จ!" : "อัปเดตข้อมูลสำเร็จ!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}