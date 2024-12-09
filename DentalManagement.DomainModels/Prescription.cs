using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalManagement.DomainModels
{
    public class Prescription : IBase
    {
        public int PrescriptionId { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DentistId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;//chẩn đoán
        public string Dosage { get; set; } = string.Empty;//Liều lượng
        public string Frequency { get; set; } = string.Empty;//tần suất
        public string Duration { get; set; } = string.Empty;//Trong vòng
        public string Notes { get; set; } = string.Empty;

        public virtual Patient Patient { get; set; }
        public virtual Dentist Dentist { get; set; }
        public virtual ICollection<PrescriptionDetails> PrescriptionDetails { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}