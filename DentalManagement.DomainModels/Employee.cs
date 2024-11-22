using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class Employee : IBase
    {
        public int EmployeeId { get; set; } // ID của nhân viên
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } // Ngày sinh
        public string Phone { get; set; } = string.Empty; // Số điện thoại
        public string Email { get; set; } = string.Empty; // Email liên lạc
        public string Address { get; set; } = string.Empty; // Địa chỉ
        public decimal Salary { get; set; } // Lương
        public DateTime DateHired { get; set; } // Ngày bắt đầu làm việc
        public string Avatar { get; set; } = string.Empty; // Ảnh đại diện nhân viên
        public string RoleName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; }
        public string? UserIdUpdated { get; set; }
    }
}
