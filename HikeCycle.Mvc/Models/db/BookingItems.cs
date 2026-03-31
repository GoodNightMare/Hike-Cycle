using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("booking_items")] 
    public class BookingItem
    {
        [Key]
        public int Id { get; set; }

        [Column("booking_id")]
        public int BookingId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [StringLength(10)]
        public string? Size { get; set; }

        public int Quantity { get; set; }

        [Column("is_free")] 
        public bool IsFree { get; set; }

        [Column("price_per_day", TypeName = "decimal(10,2)")] 
        public decimal PricePerDay { get; set; }

        [Column("item_total", TypeName = "decimal(10,2)")] 
        public decimal ItemTotal { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}