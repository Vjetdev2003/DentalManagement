using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class PaymentHistoryViewModel
    {
        public int PatientId { get; set; }
        public List<Payment> Payments { get; set; }
        public List<Invoice> Invoices { get; set; }
    }
}
