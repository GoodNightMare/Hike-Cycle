using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("user_vouchers")]
    public class UserVoucher
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [Column("code")]
        public string Code { get; set; } = null!;

        [Column("amount")]
        public decimal Amount { get; set; } = 50;

        [Column("is_used")]
        public bool IsUsed { get; set; } = false;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("PromotionId")]
        public virtual Promotion Promotion { get; set; } = null!;
    }
}