using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalManagement.DomainModels
{
    public class Prescription : IBase
    {
        public int PrescriptionId { get; set; }
        [Required]
        [ForeignKey("PatientId")]

        public int PatientId { get; set; }
        [Required]
        [ForeignKey("DentistId")]

        public int DentistId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public virtual Patient Patient { get; set; }
        public virtual Dentist Dentist { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}