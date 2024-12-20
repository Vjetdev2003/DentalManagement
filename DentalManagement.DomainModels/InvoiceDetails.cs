using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    
    public class InvoiceDetails
    {
        public int InvoiceId {  get; set; }
        public int ServiceId { get; set; }
        public int PrescriptionId { get; set; }
        public string ServiceName {  get; set; }
        public int Quantity {  get; set; }
        public decimal SalePrice { get; set; } = 0;
        public decimal PricePrescription { get; set; } = 0;
        public decimal TotalPrice
        {
            get
            {
                return Quantity * SalePrice ;
            }
        }
        //public string PaymentStatus { get; set; } = string.Empty;
        public virtual Invoice Invoice { get; set; }
       // public virtual Prescription Prescription { get; set; }
        public virtual Service Service { get; set; }
    }
}
