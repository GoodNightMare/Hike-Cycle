namespace HikeCycle.Mvc.ViewModels
{
    public class AppliedPromotion
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CartCalculationResult
    {
        public decimal OriginalTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalTotal { get; set; }
        public List<AppliedPromotion> AppliedPromotions { get; set; } = new();
    }
}