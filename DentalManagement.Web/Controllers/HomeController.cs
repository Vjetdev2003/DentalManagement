using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DentalManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DentalManagementDbContext _context;
        private readonly IRepository<Service> _serviceRepo;
        private readonly IRepository<Dentist> _dentistRepo;

        public HomeController(ILogger<HomeController> logger, DentalManagementDbContext context, IRepository<Service> serviceRepo, IRepository<Dentist> dentistRepo)
        {
            _logger = logger;
            _context = context;
            _serviceRepo = serviceRepo;
            _dentistRepo = dentistRepo;
        }

        public async Task<IActionResult> Index()
        {
            var userData = User.GetUserData();
            var services = await _context.Services.ToListAsync();
            ViewBag.ServiceList = SelectListHelper.GetServices(_serviceRepo);
           // ViewBag.DentistList = SelectListHelper.GetDentists(_dentistRepo);
         //   ViewBag.IsPatient = userData.Roles.Contains("patient");
            var model = new AppointmentCreateModel
            {
                Services = services,

            };
            return View(model);
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
