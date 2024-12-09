using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class AppointmentHistoryViewModel
    {
        public int PatientId { get; set; }  
        public List<Appointment>Appointments { get; set; }
    }
}
