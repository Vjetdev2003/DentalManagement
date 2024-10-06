using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalManagement.DomainModels
{
    public class Invoice : IBase
    {

        public int InvoiceId { get; set; }
        [Required]
        [ForeignKey("PatientId")]

        public int PatientId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }

        public virtual Patient Patient { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}