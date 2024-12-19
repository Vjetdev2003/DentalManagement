using DentalManagement.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DentalManagement.Web.Models
{
    public class InvoiceCreateModel
    {
        [Required]
        public int InvoiceId { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public int PrescriptionId {  get; set; }
        public string ServiceName {  get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; } = 0;
        public decimal TotalPrice
        {
            get
            {
                return SalePrice * Quantity; 
            }
        }
    }
}
