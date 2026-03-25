using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikeCycle.Mvc.Models.db
{
    public class EquipmentStatus
    {
        [Key]
        public int Id { get; set; }

        // เชื่อมกับ Product.Id (ซึ่งตอนนี้เป็น int ปกติแล้ว)
        public int ProductId { get; set; }

        // ใช้ Enum เพื่อให้เขียน Logic ใน C# ได้ง่ายและปลอดภัย
        public EquipmentCondition Status { get; set; } = EquipmentCondition.Available;

        public string? Note { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Property
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }

    public enum EquipmentCondition
    {
        Available,    // 0
        Rented,       // 1
        Maintenance,  // 2
        Damaged,      // 3
        Lost          // 4
    }
}