using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("product_images")]
    public class ProductImage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("product_id")]
        public required int ProductId { get; set; }

        [Column("image_url")]
        public required string ImageUrl { get; set; }

        public virtual Product? Product { get; set; }
    }
}
