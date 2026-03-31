using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("promotions")]
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        public required string Type { get; set; } 

        public required string Title { get; set; }

        public string? Description { get; set; }

        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
