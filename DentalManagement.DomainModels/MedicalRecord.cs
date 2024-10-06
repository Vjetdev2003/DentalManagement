using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentalManagement.DomainModels
{
    public class MedicalRecord : IBase
    {

        public int MedicalRecordId { get; set; }
        [Required]
        [ForeignKey("PatientId")]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey("DentistId")]
        public int DentistId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        [Required]
        [ForeignKey("ServiceId")]
        public int ServiceId { get; set; }
        public string Prescription { get; set; } = string.Empty;

        public virtual Service Service { get; set; }
        public virtual Dentist Dentist { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual ICollection<Service> Services { get; set; } // Một hồ sơ có thể có nhiều dịch vụ
        public virtual ICollection<Prescription> Prescriptions { get; set; } // Một hồ sơ có thể có nhiều đơn thuốc
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; } = string.Empty;
        public string? UserIdUpdated { get; set; } = string.Empty;
    }
}