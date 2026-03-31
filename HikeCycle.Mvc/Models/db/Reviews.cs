using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class Reviews
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public required int UserId { get; set; }

        [Column("product_id")]
        public required int ProductId { get; set; }

        [Column("booking_id")]
        public int BookingId { get; set; }

        public required int Rating { get; set; }

        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}