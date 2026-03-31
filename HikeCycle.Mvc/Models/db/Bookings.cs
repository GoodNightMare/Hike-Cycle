using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("bookings")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column("discount_amount", TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        [Column("final_amount", TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        [Column("shipping_address")]
        public string? ShippingAddress { get; set; }

        [Column("deposit_amount", TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    }
}