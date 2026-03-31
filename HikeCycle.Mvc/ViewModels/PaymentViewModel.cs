namespace HikeCycle.Mvc.ViewModels
{
    public class PaymentViewModel
    {
        public int BookingId {get; set;}
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public decimal OriginalTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public string ShippingAddress { get; set; }

        public string? VoucherCode { get; set; }
        public decimal VoucherDiscount { get; set; }
        public int DepositTotal { get; set; }
    }
}
