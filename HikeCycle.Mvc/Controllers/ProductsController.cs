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
        private readonly HikeCycledbContext _db;

        public ProductsController(HikeCycledbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string category, decimal? maxPrice)
        {
            var query = _db.Products.Include(p => p.ProductImages).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PricePerDay <= maxPrice.Value);
            }

            var products = await query.ToListAsync();

            var productIds = products.Select(p => p.Id).ToList();

            var reviews = await _db.Reviews
                                        .Where(r => productIds.Contains(r.ProductId))
                                        .ToListAsync();

            var reviewsByProductId = reviews.GroupBy(r => r.ProductId).ToDictionary(g => g.Key, g => g.ToList());

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

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var specs = !string.IsNullOrEmpty(product.Specs)
                        ? JsonDocument.Parse(product.Specs).RootElement : (JsonElement?)null;

            var suitableFor = !string.IsNullOrEmpty(product.SuitableFor)
                              ? JsonSerializer.Deserialize<List<string>>(product.SuitableFor, options)
                              : new List<string>();

            var variants = !string.IsNullOrEmpty(product.Variants)
                           ? JsonDocument.Parse(product.Variants).RootElement.EnumerateArray().ToList()
                           : new List<JsonElement>();

            int totalStock = (product.Category == "shoes" && variants.Any())
                             ? variants.Sum(v => v.GetProperty("stock").GetInt32())
                             : product.Stock.GetValueOrDefault();

            var reviews = await _db.Reviews.Where(r => r.ProductId == id).ToListAsync();
            var rating = reviews.Any() ? Math.Round((decimal)reviews.Average(r => r.Rating), 2) : 0;
            var reviewCount = reviews.Count;

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
            var product = await _db.Products.FindAsync(ProductId);
            if (product == null) return NotFound();

            DateTime start = DateTime.Parse(StartDate);
            DateTime end = DateTime.Parse(EndDate);

            if (end <= start)
            {
                TempData["ErrorMessage"] = "วันที่คืนต้องหลังจากวันที่เช่า";
                return RedirectToAction("Details", new { id = ProductId });
            }

            TempData["SuccessMessage"] = $"เพิ่ม {product.Name} ลงตะกร้าเรียบร้อย!";

            return RedirectToAction("Details", new { id = ProductId });
        }

        [HttpGet("api/all")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetProducts()
        {
            var products = await _db.Products.ToListAsync();
            var productImages = await _db.ProductImages.ToListAsync();

            var productImagesGrouped = productImages.GroupBy(pi => pi.ProductId).ToDictionary(g => g.Key, g => g.ToList());

            var allReviews = await _db.Reviews.ToListAsync();

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
            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productImages = await _db.ProductImages
                .Where(pi => pi.ProductId == id)
                .Select(pi => new { ImageUrl = pi.ImageUrl })
                .ToListAsync();

            var reviews = await _db.Reviews
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

        [HttpPost("api/create")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync();

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
            var existingProduct = await _db.Products.FindAsync(id);

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
                await _db.SaveChangesAsync();
                return Ok(new { message = "อัปเดตสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("api/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "ไม่พบสินค้าที่ต้องการลบ" });
            }

            try
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
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
            var reviews = await (from r in _db.Reviews
                                 join u in _db.UserProfiles on r.UserId equals u.UserId
                                 where r.ProductId == productId
                                 orderby r.CreatedAt descending
                                 select new 
                                 {
                                     Id = r.Id,
                                     UserId = r.UserId,
                                     UserName = u.FullName,
                                     Rating = r.Rating,
                                     Comment = r.Comment,
                                     CreatedAt = r.CreatedAt
                                 }).ToListAsync();

            return Ok(reviews);
        }
    }
}
