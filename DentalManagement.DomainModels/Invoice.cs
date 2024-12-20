using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Web.Mvc;

namespace DentalManagement.DomainModels
{
    public class Invoice : IBase
    {
        [Key]
        public int InvoiceId { get; set; }
        [Required]

        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int PrescriptionId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PatientAddress { get ; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; } 
        public decimal Discount {  get; set; }
        
        public string StatusDecription
        {
            get
            {
                switch (Status)
                {
                    case Constants_Invoice.INVOICE_UNPAID:
                        return "Đang chờ thanh toán";
                    case Constants_Invoice.INVOICE_PROCESSING:
                        return "Hoá đơn đang xử lí";
                    case Constants_Invoice.INVOICE_PAID:
                        return "Hoá đơn đã thanh toán ";
                    case Constants_Invoice.INVOICE_FAILED:
                        return "Thanh toán thất bại";
                    case Constants_Invoice.INVOICE_CANCELLED:
                        return "Hoá đơn bị huỷ";
                    default:
                        return "Trạng thái không xác định.";

                }

            }
        }
        public virtual Prescription Prescription { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
        public DateTime? FinishTime {  get; set; }
        public ICollection<InvoiceDetails> InvoiceDetails { get; set; }
      //  public  ICollection<PrescriptionDetails> PrescriptionDetails { get; set; }

    }
    public class InvoiceStatus
    {
        public int Status { get; set; }
        public string Description { get; set; }
    }
}