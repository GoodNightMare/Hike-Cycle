using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("promotions")]
    public class Promotion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("type")]
        public required string Type { get; set; } // 'Discount', 'FreeDay', 'Cashback'

        [Column("title")]
        public required string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
