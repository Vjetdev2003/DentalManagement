using System;
using System.Linq;
using System.Web.WebPages;
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

        public DashboardStatistics GetDashboardStatistics(DateTime? startDate,DateTime? endDate)
        {
            // Lấy tất cả các dịch vụ một lần và tạo từ điển để tra cứu
            var serviceDictionary = _context.Services.ToDictionary(s => s.ServiceId, s => s.ServiceName);
            //if (startDate.HasValue)
            //    query = query.Where(i => i.DateCreated >= startDate.Value);

            //if (endDate.HasValue)
            //    query = query.Where(i => i.DateCreated <= endDate.Value);
            var stats = new DashboardStatistics
            {
                // Thống kê doanh thu theo tháng
                MonthlyRevenues = _context.Invoices
                .Where(i=>i.Status ==3)
                    .GroupBy(i => new { i.DateCreated.Year, i.DateCreated.Month }) // Nhóm theo năm và tháng
                    .Select(group => new
                    {
                        Year = group.Key.Year,
                        Month = group.Key.Month,
                        Revenue = group.Sum(i => i.TotalPrice) // Tổng doanh thu cho tháng
                    })
                    .ToList() // Chuyển sang danh sách (client-side evaluation)
                    .Select(group => new MonthlyRevenue
                    {
                        // Chuyển đổi định dạng tháng/năm sau khi dữ liệu được tải
                        Month = $"{group.Month}/{group.Year}", // Tháng/Năm
                        Revenue = group.Revenue
                    })
                    .OrderBy(m => m.Month) // Sắp xếp theo tháng
                    .ToList(),

                // Các thống kê khác
                BookingAppointments = _context.Appointments.Count(a=>a.Status==1),
                TotalInvoices = _context.Invoices.Count(),
                TotalAppointments = _context.Appointments.Count(),
                TotalRevenue = _context.Invoices.Sum(i => i.TotalPrice),
                CompletedAppointments = _context.Appointments.Count(a => a.Status == 4),
                CanceledAppointments = _context.Appointments.Count(a => a.Status == -1 || a.Status == -2),

                // Thống kê các loại dịch vụ
                ServiceStatistics = _context.Appointments
                    .GroupBy(a => a.ServiceID) // Nhóm theo ServiceId
                    .Select(group => new ServiceStatistics
                    {
                        ServiceName = serviceDictionary.ContainsKey(group.Key)
                            ? serviceDictionary[group.Key]
                            : "Unknown", // Lấy tên dịch vụ từ từ điển
                        AppointmentCount = group.Count() // Đếm số cuộc hẹn cho dịch vụ này
                    })
                    .ToList() // Chuyển thành danh sách
            };

            return stats;
        }

    }
}
