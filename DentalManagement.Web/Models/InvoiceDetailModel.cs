using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class InvoiceDetailModel 
    {
        public List<InvoiceDetails> InvoiceDetails { get; set; }
        public List<InvoiceDetailViewModel> Details { get; set; }
        public Service Services { get; set; }
        public Invoice Invoices { get; set; }
       public Patient Patients { get; set; }
        public List<Payment> Payments {  get; set; }
        public InvoiceDetails InvoiceDetailss { get; set; }
    }
}
