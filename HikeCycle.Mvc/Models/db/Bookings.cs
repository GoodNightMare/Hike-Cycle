using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("bookings")] // ระบุชื่อตารางให้ตรง (เผื่อ EF ไปหาตารางชื่อ Bookings เติม s ตัวใหญ่)
    public class Booking
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } 

        [Column("user_id")] // 🚩 ตรงกับ user_id ใน SQL
        public int UserId { get; set; }

        [Column("start_date")] // 🚩 ตรงกับ start_date ใน SQL
        public DateTime StartDate { get; set; }
        
        [Column("end_date")] // 🚩 ตรงกับ end_date ใน SQL
        public DateTime EndDate { get; set; }

        [Column("total_amount", TypeName = "decimal(10,2)")] // 🚩 ตรงกับ total_amount
        public decimal TotalAmount { get; set; }

        [Column("discount_amount", TypeName = "decimal(10,2)")] // 🚩 ตรงกับ discount_amount
        public decimal DiscountAmount { get; set; }

        [Column("final_amount", TypeName = "decimal(10,2)")] // 🚩 ตรงกับ final_amount
        public decimal FinalAmount { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("created_at")] // 🚩 ตรงกับ created_at ใน SQL
        public DateTime? CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}