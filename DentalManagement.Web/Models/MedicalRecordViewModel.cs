using System.Web.Mvc;

namespace DentalManagement.Web.Models
{
    public class MedicalRecordViewModel
    {
        public List<SelectListItem> PatientList { get; set; }
        public List<SelectListItem> DentistList { get; set; }
        public List<SelectListItem> ServiceList { get; set; }
    }
}
