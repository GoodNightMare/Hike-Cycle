using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class Booking
    {
        [Key]
        public int Id { get; set; } 

        public int UserId { get; set; }

        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        // ใช้ string เพื่อให้ตรงกับ Enum ใน SQL หรือจะสร้าง Enum C# ก็ได้
        public string Status { get; set; } = "Pending";

        public DateTime? CreatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        
        // เชื่อมกับตาราง Payments (ถ้ามี)
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}