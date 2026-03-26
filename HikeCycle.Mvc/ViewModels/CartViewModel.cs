using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Controllers;
using System.Collections.Generic;

namespace HikeCycle.Mvc.ViewModels
{
    public class CartViewModel
    {
        public List<CartSessionItem> CartItems { get; set; }
        public List<Promotion> Promotions { get; set; }
        public CartCalculationResult CalculationResult { get; set; }
        public bool IsStudent { get; set; }

        public int? BookingId { get; set; }
    }
}
