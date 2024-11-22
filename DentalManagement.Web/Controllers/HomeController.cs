using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.DTOs;
using DentalManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DentalManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DentalManagementDbContext _context;

        public HomeController(ILogger<HomeController> logger, DentalManagementDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "D? li?u không h?p l?" });
                }

                var appointment = new Appointment
                {
                    PatientName = model.Name,
                    Phone = model.Phone,
                    Email = model.Email,
                    AppointmentDate = DateTime.Parse(model.Date),
                    ServiceName = model.ServiceName,
                    Notes = model.Notes,
                    Status = Constants.APPOINTMENT_IN_PROGRESS,
                    DateCreated = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "??t l?ch h?n thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi t?o l?ch h?n");
                return Json(new { success = false, message = "Có l?i x?y ra khi ??t l?ch h?n" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
