using System;
using System.Linq;
using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;

namespace DentalManagement.Web.Repository
{
    public class StatisticsService
    {
        private readonly DentalManagementDbContext _context;

        public StatisticsService(DentalManagementDbContext context)
        {
            _context = context;
        }

        public DashboardStatistics GetDashboardStatistics()
        {
            var stats = new DashboardStatistics
            {
                TotalPatients = _context.Patients.Count(),
                TotalAppointments = _context.Appointments.Count(),
                TotalRevenue = _context.Invoices.Sum(i => i.TotalPrice),

                CashPayments = _context.Payments.Count(i => i.PaymentMethod == "Tiền Mặt"),
                BankingPayments = _context.Payments.Count(i => i.PaymentMethod == "Chuyển Khoản"),
                CompletedAppointments = _context.Appointments.Count(a => a.Status == 4),
                CanceledAppointments = _context.Appointments.Count(a => a.Status == -1 || a.Status == -2),

                // Thống kê các loại dịch vụ
                ServiceStatistics = _context.Appointments
           .GroupBy(a => a.ServiceID) // Nhóm theo ServiceId
           .Select(group => new ServiceStatistics
           {
               ServiceName = _context.Services.FirstOrDefault(s => s.ServiceId == group.Key).ServiceName, // Lấy tên dịch vụ từ bảng Services
               AppointmentCount = group.Count() // Đếm số cuộc hẹn cho dịch vụ này
           }).ToList() // Chuyển thành danh sách
            };

            return stats;
        }
    }
}
