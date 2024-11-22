using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Payment : IBase
    {
        public int PaymentId {  get; set; }
        public int InvoiceId {  get; set; }
        public string PaymentMethod {  get; set; } = string.Empty;
        public decimal AmountPaid {  get; set; }
        public string Notes {  get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set ; }
        public string? UserIdCreate { get ; set; }
        public string? UserIdUpdated { get; set; }
    }
}
