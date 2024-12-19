using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class MedicalRecordDetailsModel
    {
        public MedicalRecord MedicalRecord { get; set; }
        public Dentist Dentist { get; set; }
        public Patient Patient { get; set; }
    }
}
