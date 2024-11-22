using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Medicine : IBase
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string DosageInstructions { get; set; } = string.Empty;
        public int StockQuantity { get; set; } = 0;
        public string Usage { get; set; } = string.Empty;
        public decimal Price { get; set; } = decimal.Zero;
        public string Photo {  get; set; } = string.Empty;

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}
