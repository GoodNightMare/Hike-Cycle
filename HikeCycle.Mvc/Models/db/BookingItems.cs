using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class BookingItem
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        public int ProductId { get; set; }

        [StringLength(10)]
        public string? Size { get; set; }

        public int Quantity { get; set; }

        public bool IsFree { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerDay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ItemTotal { get; set; }

        // Navigation Properties
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}