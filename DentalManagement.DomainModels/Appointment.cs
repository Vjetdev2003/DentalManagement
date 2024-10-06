using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Appointment : IBase
    {
        public int AppointmentId { get; set; }
        [Required]
        [ForeignKey("PatientId")]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey("DentistId")]
        public int DentistId { get; set; }
        [Required]
        [ForeignKey("ServiceID")]
        public int ServiceID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        [Required]
        [ForeignKey("EmployeeId")]
        public int EmployeeId { get; set; } // Khóa ngoại đến Employee
        public virtual Employee Employee { get; set; }

        public virtual Service Service { get; set; }
        public virtual Dentist Dentist { get; set; }
        public virtual Patient Patient { get; set; }
        public DateTime DateCreated { get ; set ; }
        public DateTime DateUpdated { get  ; set  ; }
        public string? UserIdCreate { get; set; } = string.Empty;
        public string? UserIdUpdated { get; set; } = string.Empty;
    }
}
