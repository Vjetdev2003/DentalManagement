using Microsoft.AspNetCore.Mvc;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Repository;
using DentalManagement.DomainModels;

namespace DentalManagement.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly StatisticsService _statisticsService;

        public DashboardController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public IActionResult Index()
        {
            var stats = _statisticsService.GetDashboardStatistics();
            return View(stats);
            //var model = new DashboardStatistics
            //{
            //    TotalPatients = 100,
            //    TotalAppointments = 200,
            //    TotalRevenue = 50000m,
            //    CashPayments = 120,
            //    CardPayments = 60,
            //    BankingPayments = 20,
            //    CompletedAppointments = 150,
            //    CanceledAppointments = 50
            //};
            //return View(model);
        }
    }
}
