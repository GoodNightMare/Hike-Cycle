namespace HikeCycle.Mvc.Models
{
    public class PaymentViewModel
    {
        public int BookingId {get; set;}
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public decimal OriginalTotal { get; set; }
        public decimal TotalDiscount { get; set; }
    }
}
