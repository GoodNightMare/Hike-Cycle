namespace HikeCycle.Models
{
    public class HomeViewModel
    {
        public List<string> Banners { get; set; } = new List<string>();
        public List<RecommendedRoute> Routes { get; set; } = new List<RecommendedRoute>();
    }
}