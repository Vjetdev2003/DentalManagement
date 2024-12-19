using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace DentalManagement.DomainModels
{
    public class Appointment : IBase
    {
        public int AppointmentId { get; set; }
        [Required]
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public int DentistId { get; set; }
        public string DentistName {  get; set; } = string.Empty;
        [Required]
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? StartTime { get; set; } 
        public TimeSpan? EndTime { get; set; }
        public string Notes {  get; set; } = string.Empty;
        public virtual Service Service { get; set; }
        public virtual Dentist Dentist { get; set; }
        public virtual Patient Patient { get; set; }
        public DateTime DateCreated { get ; set ; }
        public DateTime DateUpdated { get  ; set  ; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; } 
        public DateTime? FinishedTime { get; set; }
        public int Status {  get; set; } 
        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case Constants.APPOINTMENT_INIT:
                        return "Lịch hẹn đã đặt. Đang chờ xác nhận.";
                    case Constants.APPOINTMENT_CONFIRMED:
                        return "Lịch hẹn đã xác nhận";
                    case Constants.APPOINTMENT_IN_PROGRESS:
                        return "Lịch hẹn đang diễn ra.";
                    case Constants.APPOINTMENT_FINISHED:
                        return "Lịch hẹn đã hoàn tất.";
                    case Constants.APPOINTMENT_CANCELLED:
                        return "Lịch hẹn đã hủy.";
                    case Constants.APPOINTMENT_NO_SHOW:
                        return "Bệnh nhân không đến khám.";
                    default:
                        return "Trạng thái không xác định.";
                }
            }
        }
    }
    public class AppointmentStatus
    {
        public int Status { get; set; }
        public string Description { get; set; }
    }
}
