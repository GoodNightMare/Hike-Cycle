using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class Return
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.Now;

        // ใช้ Enum เพื่อให้โค้ดอ่านง่าย (Good, Dirty, Damaged, Lost)
        public ReturnCondition Condition { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ExtraFee { get; set; }

        public bool IsExtraFeePaid { get; set; }

        public string? Note { get; set; }

        // Navigation Property
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;
    }

    public enum ReturnCondition
    {
        Good = 0,
        Dirty = 1,
        Damaged = 2,
        Lost = 3
    }
}