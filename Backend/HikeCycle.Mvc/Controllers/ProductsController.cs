using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Models;
using HikeCycle.Mvc.Models.Dto;

namespace HikeCycle.Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly HikeCycledbContext _context;

        public ProductsController(HikeCycledbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            var productImages = await _context.ProductImages.ToListAsync();

            var productImagesGrouped = productImages.GroupBy(pi => pi.ProductId).ToDictionary(g => g.Key, g => g.ToList());

            var allReviews = await _context.Reviews.ToListAsync();

            var productDtos = products.Select(p =>
            {
                var productReviews = allReviews.Where(r => r.ProductId == p.Id).ToList();

                return new ProductDto
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
                            ? productImagesGrouped[p.Id].Select(pi => new ProductImageDto { ImageUrl = pi.ImageUrl }).ToList()
                            : new List<ProductImageDto>()
                };
            }).ToList();

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productImages = await _context.ProductImages
                .Where(pi => pi.ProductId == id)
                .Select(pi => new ProductImageDto { ImageUrl = pi.ImageUrl })
                .ToListAsync();

            var reviews = await _context.Reviews
    .Where(r => r.ProductId == id)
    .ToListAsync();

            var productDto = new ProductDto
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
        [HttpPost]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            // 1. ดึงข้อมูล "ของจริง" จาก DB มาก่อน
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null) return NotFound();

            // 2. อัปเดตเฉพาะ Field ที่เรายอมให้แก้จากหน้าบ้าน
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

            // ⚠️ ไม่ต้องแตะต้อง existingProduct.Rating หรือ CreatedAt 
            // ค่าเดิมใน DB จะยังคงอยู่เหมือนเดิม ไม่กลายเป็น NULL

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
        [HttpDelete("{id}")]
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

        [HttpGet("{productId}/reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetProductReviews(int productId)
        {
            // ดึงรีวิวของสินค้านั้นๆ และอาจจะ Join กับ UserProfile เพื่อเอาชื่อมาโชว์
            var reviews = await (from r in _context.Reviews
                                 join u in _context.UserProfiles on r.UserId equals u.UserId
                                 where r.ProductId == productId
                                 orderby r.CreatedAt descending
                                 select new ReviewDto
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
