using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("payments")] // 🚩 ระบุชื่อตารางให้เป็นตัวเล็กตามใน MySQL
    public class Payment
    {
        [Key]
        [Column("id")] // 🚩 ระบุชื่อคอลัมน์ให้ตรงกับ SQL
        public int Id { get; set; }

        [Column("booking_id")] // 🚩 ตรงกับ booking_id ใน SQL
        public int BookingId { get; set; }

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("method")]
        public PaymentMethod Method { get; set; }

        [Column("status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Column("created_at")] // 🚩 ตรงกับ created_at ใน SQL
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;
    }

    public enum PaymentMethod
    {
        Bank = 0,
        PromptPay = 1
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2
    }
}