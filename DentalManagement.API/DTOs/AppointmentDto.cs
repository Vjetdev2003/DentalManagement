using System.ComponentModel.DataAnnotations;

namespace DentalManagement.API.DTOs
{
    public class AppointmentDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string Service { get; set; }

        public string Notes { get; set; }
    }
}
