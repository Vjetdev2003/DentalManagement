namespace DentalManagement.Web.Models
{
    public class PaymentViewModel
    {
        public int InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }= string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
