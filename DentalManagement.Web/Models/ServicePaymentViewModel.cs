namespace DentalManagement.Web.Models
{
    public class ServicePaymentViewModel
    {
        public int InvoiceId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Quantity {  get; set; }
        public decimal Price { get; set; }
        public string PaymentStatus { get; set; }
    }
}
