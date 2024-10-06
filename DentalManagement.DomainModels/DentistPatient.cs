using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class DentistPatient : IBase
    {
  
        public int DentistID { get; set; }
        public int PatientID { get; set; }

        public virtual Dentist Dentist { get; set; }
        public virtual Patient Patient { get; set; }
        public DateTime DateCreated { get  ; set  ; }
        public DateTime DateUpdated { get  ; set  ; }
        public string? UserIdCreate { get; set; } = string.Empty;
        public string? UserIdUpdated { get; set; } = string.Empty;
    }
}
