using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    [Table("user_profiles")]
    public class UserProfile
    {
        [Key]
        [Column("user_id")]
        // เนื่องจากเป็น PK ที่มาจากตาราง users (FK) ปกติจะไม่มี DatabaseGenerated 
        // เพราะค่าจะถูกส่งมาจากตารางหลัก
        public int UserId { get; set; }

        [Column("full_name")]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Column("phone")]
        [MaxLength(10)]
        public string? Phone { get; set; }

        [Column("address")]
        [MaxLength(100)]
        public string? Address { get; set; }
    }
}