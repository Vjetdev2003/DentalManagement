using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Dentist : IBase
    {

        public int DentistId { get; set; }
        public string DentistName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateTime WorkSchedule { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } // Một bác sĩ có thể có nhiều hồ sơ y tế
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }

    }
}
