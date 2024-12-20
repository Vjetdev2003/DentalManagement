using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class DashboardStatistics
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int BookingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CanceledAppointments { get; set; }
        public IEnumerable<ServiceStatistics> ServiceStatistics { get; set; }
        public List<MonthlyRevenue> MonthlyRevenues { get; set; }
    }
    public class ServiceStatistics
    {
        public string ServiceName { get; set; }
        public int AppointmentCount { get; set; }
    }
    public class MonthlyRevenue
    {
        public string Month { get; set; }  // Tháng (ví dụ: "January", "February")
        public decimal Revenue { get; set; }  // Doanh thu của tháng
        public int InvoiceCount {  get; set; }
    }
}
