using System;
using System.ComponentModel.DataAnnotations;
namespace DentalManagement.Web.DTOs
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
            public string ServiceName { get; set; }

            public string Notes { get; set; }
        }
    }
