namespace CSE3200.Web.Areas.Admin.Models
{
    public class PaymentResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid DonationId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}
