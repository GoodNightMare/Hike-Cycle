using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [Column("password_hash")]
        public required string Password { get; set; }

        [Column("role")]
        public string Role { get; set; } = "user";

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}