using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class InvoiceDetailModel 
    {
        public List<InvoiceDetailViewModel> Details { get; set; }
        public Invoice Invoices { get; set; }
        public Prescription Prescriptions { get; set; }
        public List<PrescriptionCreateModel> PrescriptionDetails { get; set; }
        public List<PaymentViewModel> Payments {  get; set; }
    }
}
