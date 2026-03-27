using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class Return
    {
        [Key]
        public int Id { get; set; }

        [Column("booking_id")]
        public int BookingId { get; set; }
        
        [Column("return_date")]
        public DateTime ReturnDate { get; set; } = DateTime.Now;

        public ReturnCondition Condition { get; set; }

        [Column("extra_fee", TypeName = "decimal(10,2)")]
        public decimal ExtraFee { get; set; }

        [Column("is_extra_fee_paid")]
        public bool IsExtraFeePaid { get; set; }

        public string? Note { get; set; }

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