using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            // Set default values for startDate and endDate if not provided
            startDate ??= DateTime.Now.AddMonths(-1); // Default to one month ago
            endDate ??= DateTime.Now; // Default to current date

            // Validate that startDate is not later than endDate
            if (startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "Start date cannot be later than end date.");
                return View(); // Return the view with an error message
            }

            // Get the statistics from the service
            var stats = _statisticsService.GetDashboardStatistics(startDate.Value, endDate.Value);

            // Pass the statistics to the view
            return View(stats);
        }
    }
}
