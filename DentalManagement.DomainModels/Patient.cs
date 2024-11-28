using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Patient : IBase
    {

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime PatientDOB { get; set; }
        public bool Gender { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleName {  get; set; } = string.Empty;  
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } 
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } // Một bệnh nhân có thể có nhiều hồ sơ y tế
        public virtual ICollection<Invoice> Invoices { get; set; } // Một bệnh nhân có thể có nhiều hóa đơn
        public DateTime DateCreated { get ; set; }
        public DateTime DateUpdated { get ; set ; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}
