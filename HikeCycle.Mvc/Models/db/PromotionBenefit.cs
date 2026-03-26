using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("promotion_benefits")]
    public class PromotionBenefit
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [Column("benefit_key")]
        public required string BenefitKey { get; set; }

        [Column("benefit_value")]
        public required string BenefitValue { get; set; }
    }
}