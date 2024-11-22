using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class AppointmentDetailModel
    {
       public List<AppointmentViewModel> Details;
       public Appointment Appointments{get; set;}
        public Service Services{get; set;}
        public Dentist Dentists{get; set;}
    }
}
    