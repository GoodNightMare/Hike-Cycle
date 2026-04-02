using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("user_profiles")]
    public class UserProfile
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("full_name")]
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
        [Column("is_expert")]
        public bool IsExpert { get; set; } = false;
    }
}