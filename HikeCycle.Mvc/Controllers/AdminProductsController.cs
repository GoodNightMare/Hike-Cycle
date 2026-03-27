using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using Microsoft.AspNetCore.Authorization;

namespace HikeCycle.Mvc.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class AdminProductsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public AdminProductsController(HikeCycledbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // ดึงสินค้าพร้อมรูปภาพ และเรียงตาม ID ล่าสุด
            var products = await _context.Products
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
        var products = await _context.Products.Include(p => p.ProductImages).ToListAsync();
        return View("Index", products);
    }

    if (model.Id == 0)
    {
        // ✨ กรณีเพิ่มสินค้าใหม่
        model.CreatedAt = DateTime.Now;
        _context.Products.Add(model);
    }
    else
    {
        // ✏️ กรณีแก้ไขสินค้าเดิม
        _context.Products.Update(model);
        // ป้องกันไม่ให้ CreatedAt กลายเป็นค่าว่าง/ค่าเริ่มต้น
        _context.Entry(model).Property(x => x.CreatedAt).IsModified = false;
    }

    await _context.SaveChangesAsync(); // บันทึกเพื่อให้ได้ model.Id มาใช้กับรูปภาพ

    // 📸 จัดการอัปโหลดรูปภาพ (ถ้ามีการเลือกไฟล์)
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

                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = model.Id,
                    ImageUrl = "/uploads/products/" + fileName
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    TempData["Success"] = model.Id == 0 ? "เพิ่มสินค้าสำเร็จ!" : "อัปเดตข้อมูลสำเร็จ!";
    return RedirectToAction(nameof(Index));
}

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}