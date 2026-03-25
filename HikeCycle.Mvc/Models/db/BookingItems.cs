using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("booking_items")] // 🚩 ระบุชื่อตารางให้ตรงกับ MySQL (มีขีดล่าง)
    public class BookingItem
    {
        [Key]
        [Column("id")] // 🚩 ระบุชื่อคอลัมน์ให้ตรงกับ SQL
        public int Id { get; set; }

        [Column("booking_id")] // 🚩 ตรงกับ booking_id ใน SQL
        public int BookingId { get; set; }

        [Column("product_id")] // 🚩 ตรงกับ product_id ใน SQL
        public int ProductId { get; set; }

        [Column("size")]
        [StringLength(10)]
        public string? Size { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("is_free")] // 🚩 ตรงกับ is_free ใน SQL
        public bool IsFree { get; set; }

        [Column("price_per_day", TypeName = "decimal(10,2)")] // 🚩 ตรงกับ price_per_day
        public decimal PricePerDay { get; set; }

        [Column("item_total", TypeName = "decimal(10,2)")] // 🚩 ตรงกับ item_total
        public decimal ItemTotal { get; set; }

        // Navigation Properties
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}