using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("promotion_conditions")]
    public class PromotionCondition
    {
        [Key]
        public int Id { get; set; }

        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [Column("condition_key")]
        public required string ConditionKey { get; set; }

        [Column("condition_value")]
        public required string ConditionValue { get; set; }
    }
}