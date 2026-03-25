using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("reviews")]
    public class Reviews
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public required int UserId { get; set; }

        [Column("product_id")]
        public required int ProductId { get; set; }

        [Column("booking_id")]
        public int BookingId { get; set; }

        [Column("rating")]
        public required int Rating { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}