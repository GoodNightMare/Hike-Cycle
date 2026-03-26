using System.Text.Json;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public JsonElement? Specs { get; set; }
        public List<string> SuitableFor { get; set; } = new();
        public List<JsonElement> Variants { get; set; } = new();
        public int TotalStock { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public string MinStartDate { get; set; } = null!;
        public string MinEndDate { get; set; } = null!;
    }
}