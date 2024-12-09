using Microsoft.Build.Framework;

namespace DentalManagement.Web.Models
{
    public class PrescriptionCreateModel
    {
        [Required]
        public int PatientId { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
