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
            var query = _db.Products
                        .Include(p => p.ProductImages)
                        .Where(p => p.Status == "Active")
                        .AsQueryable();

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

            var specs = !string.IsNullOrEmpty(product.Specs)
                        ? JsonDocument.Parse(product.Specs).RootElement : (JsonElement?)null;

            var suitableFor = !string.IsNullOrEmpty(product.SuitableFor)
                              ? JsonSerializer.Deserialize<List<string>>(product.SuitableFor)
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
    }
}
