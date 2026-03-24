using HikeCycle.Mvc.Models.db;
using HikeCycle.Mvc.Controllers;
using System.Collections.Generic;

namespace HikeCycle.Mvc.Models
{
    public class CartViewModel
    {
        public List<CartSessionItem> CartItems { get; set; }
        public List<Promotion> Promotions { get; set; }
        public CartCalculationResult CalculationResult { get; set; }
        public bool IsStudent { get; set; }
    }
}
