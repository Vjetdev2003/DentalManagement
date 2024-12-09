namespace DentalManagement.Web.Models
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int InvoiceId {  get; set; }
        public string PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime DateCreated { get; set; }
        public string Notes { get; set; }
    }
}
