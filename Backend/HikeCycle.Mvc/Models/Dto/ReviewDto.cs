using System;

namespace HikeCycle.Mvc.Models.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; } // สำหรับโชว์ชื่อคนรีวิวในหน้า React
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}