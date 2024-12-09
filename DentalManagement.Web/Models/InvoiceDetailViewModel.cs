namespace DentalManagement.Web.Models
{
    public class InvoiceDetailViewModel
    {
        public int InvoiceId { get; set; }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus {  get; set; }

    }
}
