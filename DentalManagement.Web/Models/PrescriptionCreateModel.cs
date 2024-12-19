using Microsoft.Build.Framework;

namespace DentalManagement.Web.Models
{
    public class PrescriptionCreateModel
    {
        public int PrescriptionId { get; set; } 
        [Required]
        public int PatientId { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice{get;set; }
    }
}
