using DentalManagement.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DentalManagement.Web.Models
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân.")]
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn nha sĩ.")]
        public int DentistId { get; set; }
        public string DentistName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn dịch vụ.")]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày hẹn.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hẹn")]
        public DateTime AppointmentDate { get; set; }
        public string Phone {  get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public int Status { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; } = string.Empty;
        public DateTime? FinishedTime { get; set; }
        

        // Lists for dropdowns
        public IEnumerable<SelectListItem> PatientList { get; set; }
        public IEnumerable<SelectListItem> DentistList { get; set; }
        public IEnumerable<SelectListItem> ServiceList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string StatusDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; } = string.Empty;
        public string? UserIdUpdated { get; set; } = string.Empty;
    }

}