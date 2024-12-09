using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class MedicalRecord : IBase
    {
        public int MedicalRecordId { get; set; }
        [Required]
        public int? PatientId { get; set; }

        [Required]
        public int? DentistId { get; set; }
        [Required]
        public int? ServiceId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;//Chẩn đoán
        public string Treatment { get; set; } = string.Empty;//Phương pháp điều trị
        public DateTime? DateOfTreatment { get; set; } // Ngày bắt đầu điều trị
        public int Status {  get; set; }
        public string DescriptionStatus
        {
            get
            {
                switch (Status)
                {
                    case Constants_MedicalRecord.COMPLETE:
                        return "Đã hoàn thành điều trị";
                    case Constants_MedicalRecord.PENDING:
                        return "Hồ sơ bệnh án đang xử lí";
                    case Constants_MedicalRecord.CANCELLED:
                        return "Hồ sơ đã được xem xét";
                    default:
                        return "Trạng thái hồ sơ bệnh án không xác định.";

                }

            }
        }
        public string Symptoms { get; set; } = string.Empty;//Triệu chứng
        public DateTime? NextAppointmentDate { get; set; } // Ngày hẹn tiếp
        public string TreatmentOutcome { get; set; } = string.Empty; //Kết quả điều trị 
        
        public virtual Service Service { get; set; }
        public virtual Dentist Dentist { get; set; }
        public virtual Patient Patient { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}
