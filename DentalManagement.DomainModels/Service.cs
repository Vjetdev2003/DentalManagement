using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalManagement.DomainModels
{
    public class Service : IBase
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } // Một dịch vụ có thể liên quan đến nhiều hồ sơ y tế
        public virtual ICollection<Invoice> Invoices { get; set; } // Một dịch vụ có thể có nhiều hóa đơn
        public DateTime DateCreated { get ; set ; }
        public DateTime DateUpdated { get; set ; }
        public string? UserIdCreate { get; set ; }
        public string? UserIdUpdated { get ; set ; }
    }
}