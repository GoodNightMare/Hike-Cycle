namespace HikeCycle.Mvc.ViewModels
{
    public class CartSessionItem
    {
        public string Id { get; set; } = "";
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? ImageUrl { get; set; }
        public decimal PricePerDay { get; set; }
        public string? Size { get; set; }
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string? Category { get; set; } 
        public bool IsFree { get; set; } = false;
        public bool IsRemovable { get; set; } = true;

    }
}