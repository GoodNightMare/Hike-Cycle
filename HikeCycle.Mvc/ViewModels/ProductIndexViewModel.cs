using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.ViewModels
{
    public class ProductIndexViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal? PricePerDay { get; set; }
        public int? Stock { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
