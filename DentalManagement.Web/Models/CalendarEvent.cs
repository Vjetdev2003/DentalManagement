using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
	public class CalendarEvent
	{
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public string Description { get; set; }
        public int AppointmentId {  get; set; }
        public int Status { get; set; }
    }
}
