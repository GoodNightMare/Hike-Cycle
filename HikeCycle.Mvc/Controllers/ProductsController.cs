using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Models;
using System.Text.Json;
using HikeCycle.Mvc.ViewModels;

namespace HikeCycle.Mvc.Controllers
{
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly HikeCycledbContext _context;

        public ProductsController(HikeCycledbContext context)
        {
            _context = context;
        }

        // GET: /Products
        [HttpGet]
        public async Task<IActionResult> Index(string search, string category, decimal? maxPrice)
        {
            // 1. Start with the base query
            var query = _context.Products.Include(p => p.ProductImages).AsQueryable();

            // 2. Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            // 3. Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // 4. Filter by max price
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PricePerDay <= maxPrice.Value);
            }

            // 5. Execute the query to get filtered products
            var products = await query.ToListAsync();

            // 6. Get product IDs for the review query
            var productIds = products.Select(p => p.Id).ToList();

            // 7. Fetch all reviews for the filtered products in one go
            var reviews = await _context.Reviews
                                        .Where(r => productIds.Contains(r.ProductId))
                                        .ToListAsync();

            // 8. Group reviews by ProductId for efficient lookup
            var reviewsByProductId = reviews.GroupBy(r => r.ProductId).ToDictionary(g => g.Key, g => g.ToList());

            // 9. Create the view model list
            var viewModel = products.Select(p =>
            {
                var productReviews = reviewsByProductId.ContainsKey(p.Id) ? reviewsByProductId[p.Id] : new List<Reviews>();
                return new ProductIndexViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Brand = p.Brand,
                    PricePerDay = p.PricePerDay,
                    Stock = p.Stock,
                    Rating = productReviews.Any() ? (decimal)productReviews.Average(r => r.Rating) : 0,
                    ReviewCount = productReviews.Count,
                    ProductImages = p.ProductImages
                };
            }).ToList();

            return View(viewModel);
        }

        // GET: /Products/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            // --- [Logic การประมวลผลข้อมูล JSON] ---
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var specs = !string.IsNullOrEmpty(product.Specs)
                        ? JsonDocument.Parse(product.Specs).RootElement : (JsonElement?)null;

            var suitableFor = !string.IsNullOrEmpty(product.SuitableFor)
                              ? JsonSerializer.Deserialize<List<string>>(product.SuitableFor, options)
                              : new List<string>();

            var variants = !string.IsNullOrEmpty(product.Variants)
                           ? JsonDocument.Parse(product.Variants).RootElement.EnumerateArray().ToList()
                           : new List<JsonElement>();

            // --- [การคำนวณ Stock] ---
            int totalStock = (product.Category == "shoes" && variants.Any())
                             ? variants.Sum(v => v.GetProperty("stock").GetInt32())
                             : product.Stock.GetValueOrDefault();

            // --- [การคำนวณ Rating และ Review Count] ---
            var reviews = await _context.Reviews.Where(r => r.ProductId == id).ToListAsync();
            var rating = reviews.Any() ? Math.Round((decimal)reviews.Average(r => r.Rating), 2) : 0;
            var reviewCount = reviews.Count;

            // --- [การคำนวณวันที่] ---
            var tomorrow = DateTime.Now.AddDays(1);

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                Specs = specs,
                SuitableFor = suitableFor,
                Variants = variants,
                TotalStock = totalStock,
                Rating = rating,
                ReviewCount = reviewCount,
                MinStartDate = tomorrow.ToString("yyyy-MM-dd"),
                MinEndDate = tomorrow.AddDays(1).ToString("yyyy-MM-dd")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int ProductId, string StartDate, string EndDate, string PickupTime, string? Size)
        {
            // --- [Logic การประมวลผลฝั่ง Controller] ---
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null) return NotFound();

            // 1. จำลองการเช็คข้อมูล (เช่น วันที่)
            DateTime start = DateTime.Parse(StartDate);
            DateTime end = DateTime.Parse(EndDate);

            if (end <= start)
            {
                // ส่งข้อความ Error กลับไปหน้าเดิม
                TempData["ErrorMessage"] = "วันที่คืนต้องหลังจากวันที่เช่า";
                return RedirectToAction("Details", new { id = ProductId });
            }

            // 2. บันทึกลงตะกร้า (Session หรือ Database)
            // ตรงนี้คือจุดที่คุณจัดการข้อมูลตามต้องการ

            // 3. ส่งข้อความ Success กลับไปหน้าเดิม (ทำให้ดูเหมือนไม่ย้ายหน้า)
            TempData["SuccessMessage"] = $"เพิ่ม {product.Name} ลงตะกร้าเรียบร้อย!";

            // Redirect กลับมาที่หน้า Details ของสินค้าตัวเดิม
            return RedirectToAction("Details", new { id = ProductId });
        }

        [HttpGet("api/all")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var productImages = await _context.ProductImages.ToListAsync();

            var productImagesGrouped = productImages.GroupBy(pi => pi.ProductId).ToDictionary(g => g.Key, g => g.ToList());

            var allReviews = await _context.Reviews.ToListAsync();

            var productDtos = products.Select(p =>
            {
                var productReviews = allReviews.Where(r => r.ProductId == p.Id).ToList();

                return (dynamic)new 
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    Brand = p.Brand,
                    PricePerDay = p.PricePerDay,
                    Stock = p.Stock,
                    Status = p.Status,
                    Level = p.Level,
                    Rating = productReviews.Any() ? (decimal)productReviews.Average(r => r.Rating) : 0,
                    ReviewCount = productReviews.Count,
                    CreatedAt = p.CreatedAt,
                    Specs = p.Specs,
                    SuitableFor = p.SuitableFor,
                    Variants = p.Variants,

                    ProductImages = productImagesGrouped.ContainsKey(p.Id)
                            ? productImagesGrouped[p.Id].Select(pi => (dynamic)new { ImageUrl = pi.ImageUrl }).ToList()
                            : new List<dynamic>()
                };
            }).ToList();

            return Ok(productDtos);
        }

        [HttpGet("api/{id}")]
        public async Task<ActionResult<dynamic>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productImages = await _context.ProductImages
                .Where(pi => pi.ProductId == id)
                .Select(pi => new { ImageUrl = pi.ImageUrl })
                .ToListAsync();

            var reviews = await _context.Reviews
    .Where(r => r.ProductId == id)
    .ToListAsync();

            var productDto = new 
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Brand = product.Brand,
                PricePerDay = product.PricePerDay,
                Stock = product.Stock,
                Status = product.Status,
                Level = product.Level,
                Rating = reviews.Any() ? Math.Round((decimal)reviews.Average(r => r.Rating), 2) : 0,
                ReviewCount = reviews.Count,
                CreatedAt = product.CreatedAt,
                Specs = product.Specs,
                SuitableFor = product.SuitableFor,
                Variants = product.Variants,
                ProductImages = productImages
            };

            return Ok(productDto);
        }

        // POST: api/Products
        [HttpPost("api/create")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "ไม่สามารถเพิ่มสินค้าได้", error = ex.Message });
            }
        }

        [HttpPut("api/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            // 1. ดึงข้อมูล "ของจริง" จาก DB มาก่อน
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null) return NotFound();

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Category = updatedProduct.Category;
            existingProduct.Brand = updatedProduct.Brand;
            existingProduct.PricePerDay = updatedProduct.PricePerDay;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.Status = updatedProduct.Status;
            existingProduct.Level = updatedProduct.Level;
            existingProduct.Specs = updatedProduct.Specs;
            existingProduct.SuitableFor = updatedProduct.SuitableFor;
            existingProduct.Variants = updatedProduct.Variants;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "อัปเดตสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("api/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "ไม่พบสินค้าที่ต้องการลบ" });
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(new { message = "ลบสินค้าสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "ไม่สามารถลบสินค้าได้ เนื่องจากมีการอ้างอิงข้อมูลในตารางอื่น", error = ex.Message });
            }
        }

        [HttpGet("api/{productId}/reviews")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetProductReviews(int productId)
        {
            // ดึงรีวิวของสินค้านั้นๆ และอาจจะ Join กับ UserProfile เพื่อเอาชื่อมาโชว์
            var reviews = await (from r in _context.Reviews
                                 join u in _context.UserProfiles on r.UserId equals u.UserId
                                 where r.ProductId == productId
                                 orderby r.CreatedAt descending
                                 select new 
                                 {
                                     Id = r.Id,
                                     UserId = r.UserId,
                                     UserName = u.FullName, // ดึงชื่อจริงจากตาราง user_profiles มาใส่ใน DTO
                                     Rating = r.Rating,
                                     Comment = r.Comment,
                                     CreatedAt = r.CreatedAt
                                 }).ToListAsync();

            return Ok(reviews);
        }
    }
}
