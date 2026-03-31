using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.ViewModels
{
    public class CartViewModel
    {
        public List<CartSessionItem> CartItems { get; set; }
        public List<Promotion> Promotions { get; set; }
        public CartCalculationResult CalculationResult { get; set; }
        public bool IsStudent { get; set; }

        public int? BookingId { get; set; }

        public List<UserVoucher> AvailableVouchers { get; set; } = new();
    }
}
