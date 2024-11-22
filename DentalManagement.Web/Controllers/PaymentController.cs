using Microsoft.AspNetCore.Mvc;
using DentalManagement.DomainModels;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Repository;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;
using DentalManagement.Web.Models;

namespace DentalManagement.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly InvoiceDetailsRepository _invoiceDetailsRepository;
        private readonly IRepository<Service> _serviceRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly DentalManagementDbContext _context;

        public PaymentController(PaymentRepository paymentRepository,InvoiceDetailsRepository invoiceDetailsRepository,IRepository<Service>serviceRepository, IInvoiceRepository invoiceRepository,DentalManagementDbContext context)
        {
            _paymentRepository = paymentRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _serviceRepository = serviceRepository;
            _invoiceRepository = invoiceRepository;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Login");
            }

            if (!userData.Roles.Contains(WebUserRoles.Patient))
            {
                return Unauthorized("Chỉ bệnh nhân mới được phép thực hiện thao tác này.");
            }

            int patientId = Int32.Parse(userData.UserId);

            var unpaidServices = await _context.InvoiceDetails
                    .Include(d => d.Service)
                    .Where(d => d.Invoice.PatientId == patientId && d.ServiceStatus != "Đã thanh toán")
                    .ToListAsync();

            // Map to ViewModel
            var servicePaymentViewModels = unpaidServices.Select(d => new ServicePaymentViewModel
            {
                ServiceId = d.ServiceId,
                ServiceName = d.Service?.ServiceName,
                Price = d.Service?.Price ?? 0,
                PaymentStatus = d.PaymentStatus
            }).ToList();

            // Pass the ViewModel to the View
            return View(servicePaymentViewModels);
        }


        public async Task<IActionResult> AddPayment(int serviceId,int invoiceId)
        {
            try
            {
                // Get the current user
                var userData = User.GetUserData();
                if (userData == null)
                {
                    return RedirectToAction("Login");
                }

                if (!userData.Roles.Contains(WebUserRoles.Patient))
                {
                    return Unauthorized("Chỉ bệnh nhân mới được phép thực hiện thao tác này.");
                }

                int patientId = Int32.Parse(userData.UserId);

                // Find the unpaid service in the patient's invoice
                var invoiceDetail = await _context.InvoiceDetails
                    .Include(d => d.Service)
                    .Include(d => d.Invoice)
                    .FirstOrDefaultAsync(d =>
                        d.ServiceId == serviceId &&
                        d.Invoice.PatientId == patientId &&
                        d.ServiceStatus != "Đã thanh toán");

                if (invoiceDetail == null)
                {
                    return NotFound("Dịch vụ không tồn tại hoặc đã được thanh toán.");
                }
                var invoice = invoiceDetail.Invoice;
                invoice.Status = 2;
                invoiceDetail.ServiceStatus = "Đã thanh toán";

                // Save changes to the database
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thanh toán dịch vụ thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest("Đã xảy ra lỗi trong quá trình thanh toán: " + ex.Message);
            }
        }


        // POST: Add a Payment
        [HttpPost]
        public async Task<IActionResult> AddPayment(Payment model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set audit information
                    var currentUser = User.GetUserData();
                    model.UserIdCreate = currentUser?.UserId;
                    model.DateCreated = DateTime.Now;

                    // Add payment to database
                    await _paymentRepository.AddPaymentAsync(model);

                    TempData["Message"] = "Payment added successfully.";
                    return RedirectToAction("Details", "Invoice", new { id = model.InvoiceId });
                }

                TempData["Error"] = "Invalid payment details. Please check your input.";
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding payment: {ex.Message}");
                TempData["Error"] = "An error occurred while adding the payment.";
                return RedirectToAction("Details", "Invoice", new { id = model.InvoiceId });
            }
        }
       


        // GET: View Payments by Invoice ID
        [HttpGet]
        public async Task<IActionResult> GetPaymentsByInvoiceId(int invoiceId)
        {
            try
            {
                var payments = await _paymentRepository.GetPaymentsByInvoiceIdAsync(invoiceId);

                if (payments == null || payments.Count() == 0)
                {
                    TempData["Message"] = "No payments found for this invoice.";
                }

                return View(payments); // Pass payments to the view
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving payments: {ex.Message}");
                TempData["Error"] = "An error occurred while retrieving payments.";
                return RedirectToAction("Details", "Invoice", new { id = invoiceId });
            }
        }
    }
}
