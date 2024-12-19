using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalManagement.DomainModels
{
    public class Service : IBase
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Photo { get; set; } = string.Empty ;
        public string Description { get; set; } = string.Empty;
        public ICollection<InvoiceDetails> InvoiceDetails { get; set; }
        public DateTime DateCreated { get ; set ; }
        public DateTime DateUpdated { get; set ; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}