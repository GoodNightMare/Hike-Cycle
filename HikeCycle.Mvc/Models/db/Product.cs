using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Brand { get; set; }

        [Column("price_per_day")]
        public decimal? PricePerDay { get; set; }

        public int? Stock { get; set; }

        public string? Status { get; set; }

        public string? Level { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        public string? Specs { get; set; }

        [Column("suitable_for")]
        public string? SuitableFor { get; set; }

        public string? Variants { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
