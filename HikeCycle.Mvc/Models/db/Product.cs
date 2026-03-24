using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("brand")]
        public string? Brand { get; set; }

        [Column("price_per_day")]
        public decimal? PricePerDay { get; set; }

        [Column("stock")]
        public int? Stock { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("level")]
        public string? Level { get; set; }



        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("specs")]
        public string? Specs { get; set; }

        [Column("suitable_for")]
        public string? SuitableFor { get; set; }

        [Column("variants")]
        public string? Variants { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
