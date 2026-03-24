using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // เก็บเป็น Enum เพื่อให้เขียน C# ง่าย (เช่น if(p.Method == PaymentMethod.Bank))
        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

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