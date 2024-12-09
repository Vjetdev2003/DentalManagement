using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class AppointmentCreateModel
    {
        public int ServiceId { get; set; }
        public int PatientId {  get; set; }
        public int? DentistId {  get; set; }
        public string DentistName {  get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Notes {  get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string TimeSlot { get; set; } =string.Empty;

        public List<Service> Services { get; set; }
    }
}
