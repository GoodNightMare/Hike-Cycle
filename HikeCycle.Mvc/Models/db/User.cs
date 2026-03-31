using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public required string Email { get; set; }

        [Column("password_hash")]
        public required string Password { get; set; }

        public string Role { get; set; } = "user";

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}