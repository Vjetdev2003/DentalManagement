﻿using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;

namespace DentalManagement.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly InvoiceDetailsRepository _invoiceDetailsRepository;
        private readonly IRepository<Service> _serviceRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly DentalManagementDbContext _context;
        private readonly IRepository<Patient> _patientRepository;
        private readonly PrescriptionRepository _prescriptionRepository;
        private readonly PrescriptionDetailsRepository _prescriptionDRepository;
        private readonly string clientId = "c3efd2c7-926e-4754-8eec-dd7f36642c23"; // Replace with your actual client ID
        private readonly string apiKey = "181f2996-525f-4fc4-aeca-9767b0152d11"; // Replace with your actual API key
        private readonly string checksumKey = "e2c88633b9cb46240d1300376bac12c6077566f54d848d84066909f7e7c2f7b9";

        public PaymentController(PaymentRepository paymentRepository,IRepository<Patient> patientRepository, 
            InvoiceDetailsRepository invoiceDetailsRepository, IRepository<Service> serviceRepository, 
            IInvoiceRepository invoiceRepository, DentalManagementDbContext context, PrescriptionRepository prescriptionRepository, PrescriptionDetailsRepository prescriptionDRepository)
        {
            _paymentRepository = paymentRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _serviceRepository = serviceRepository;
            _invoiceRepository = invoiceRepository;
            _context = context;
            _patientRepository = patientRepository;
            _prescriptionRepository = prescriptionRepository;
            _prescriptionDRepository = prescriptionDRepository;
        }
        public async Task<IActionResult> Index()
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Index");
            }

            int patientId = Int32.Parse(userData.UserId);
            var unpaidInvoices = await _context.Invoices
                  .Include(i => i.InvoiceDetails)
                  .ThenInclude(d => d.Service)
                  .Where(i => i.PatientId == patientId && i.Status == 1)
                  .OrderByDescending(p=>p.DateCreated)
                  .ToListAsync();
            var invoiceViewModels = unpaidInvoices.Select(i => new InvoicePaymentViewModel
            {
                InvoiceId = i.InvoiceId,
                TotalAmount = i.InvoiceDetails.Sum(d => d.TotalPrice + d.PricePrescription) - i.Discount,
                Discount = i.Discount,
                PatientName = userData.DisplayName,
                Status = i.Status,
            }).ToList();
            ViewBag.IsPatient = userData.Roles.Contains("patient");
            return View(invoiceViewModels);
        }
        [Authorize(Roles = $"{WebUserRoles.Patient}")]
        [HttpGet, HttpPost]
        public async Task<IActionResult> AddPayment(int invoiceId)
        {
            try
            {
                // Lấy dữ liệu người dùng
                var userData = User.GetUserData();
                if (userData == null)
                {
                    return RedirectToAction("Login");
                }

                int patientId = Int32.Parse(userData.UserId);

                // Tìm hóa đơn chưa thanh toán của bệnh nhân
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceDetails)
                    .FirstOrDefaultAsync(i => i.PatientId == patientId && i.InvoiceId == invoiceId && i.Status == 1);

                if (invoice == null)
                {
                    return NotFound("Không tìm thấy hóa đơn hoặc hóa đơn đã được thanh toán.");
                }

                var invoiceDetails = await _context.InvoiceDetails.Where(i => i.InvoiceId == invoiceId).ToListAsync();
                if (invoiceDetails == null || invoiceDetails.Count == 0)
                {
                    return NotFound("Không tìm thấy chi tiết hóa đơn.");
                }

                // Tạo một đối tượng thanh toán mới
                var payment = new Payment
                {
                    InvoiceId = invoiceId,
                    ServiceId = invoiceDetails.FirstOrDefault().ServiceId,
                    ServiceName = invoiceDetails.FirstOrDefault().ServiceName,
                    PaymentMethod = Paymethod.CASH,  // Phương thức thanh toán
                    PaymentStatus = "Đang xử lý thanh toán tiền mặt",  // Trạng thái thanh toán
                    AmountPaid = invoiceDetails.Sum(i => i.TotalPrice) - invoice.Discount,  // Tổng tiền thanh toán
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Notes = "Thanh toán cho hoá đơn",
                };

                // Thêm thanh toán vào cơ sở dữ liệu
                _context.Payments.Add(payment);

                // Thay đổi trạng thái hóa đơn thành đã thanh toán
                invoice.Status = 2;

                // Lưu thay đổi vào cơ sở dữ liệu
                var checkSuccess = await _context.SaveChangesAsync() > 0;

                if (checkSuccess)
                {
                    return Json(new { success = true, messages = "Giao dịch đang được xử lý. Vui lòng vào hoá đơn để xem", redirectUrl = Url.Action("Index", "Payment") });
                }
                else
                {
                    return Json(new { success = false, messages = "Giao dịch không thành công, vui lòng thử lại sau!", redirectUrl = Url.Action("Index", "Payment") });
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình thanh toán: " + ex.Message);
            }
        }


        [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
        public async Task<IActionResult> ApprovePayment(int invoiceId)
        {
            try
            {
                // Lấy hóa đơn cần thanh toán
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceDetails) // Bao gồm các chi tiết hóa đơn
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.Status == 2);
                var patient = await _context.Patients.SingleOrDefaultAsync(p => p.PatientId == invoice.PatientId);
                var patientId = await _patientRepository.GetByIdAsync(patient.PatientId);
               
                // Kiểm tra xem có hóa đơn chưa thanh toán không
                if (invoice == null)
                {
                    return NotFound("Không thể tìm thấy hóa đơn này hoặc hóa đơn đã được thanh toán.");
                }

                // Cập nhật trạng thái thanh toán của hóa đơn
                invoice.Status = 3; // Đánh dấu hóa đơn đã thanh toán
                invoice.FinishTime = DateTime.Now;
                invoice.DateUpdated = DateTime.Now;
                var invoiceDetails = await _context.InvoiceDetails.SingleOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);

                // Cập nhật thông tin thanh toán, nếu có
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.InvoiceId == invoiceId);
                if (payment != null)
                {
                    payment.PaymentStatus = "Thanh toán tiền mặt thành công";
                }
                else
                {
                    // Nếu không tìm thấy bản ghi thanh toán, có thể cần tạo mới bản ghi Payment
                    _context.Payments.Add(new Payment
                    {
                        InvoiceId = invoiceId,
                        AmountPaid = invoice.TotalPrice,
                        ServiceId = invoiceDetails.ServiceId,
                        ServiceName = invoiceDetails.ServiceName,
                        PaymentStatus = "Thanh toán tiền mặt thành công",
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Notes = "Thanh toán cho hoá đơn",
                        UserIdCreate = patient.PatientName.ToString()
                    });
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Thông báo thành công và chuyển hướng
                TempData["SuccessMessage"] = "Thanh toán đã được xác nhận thành công.";
                return RedirectToAction("PendingPayments");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có
                Console.WriteLine($"Error in ApprovePayment: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xác nhận thanh toán.";
                return RedirectToAction("PendingPayments");
            }
        }
        [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
        public async Task<IActionResult> RejectPayment(int invoiceId)
        {
            try
            {
                // Lấy hóa đơn cần thanh toán
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceDetails) // Bao gồm các chi tiết hóa đơn
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.Status == 2);
                var patient = await _context.Patients.SingleOrDefaultAsync(p => p.PatientId == invoice.PatientId);
                // Kiểm tra xem có hóa đơn chưa thanh toán không
                if (invoice == null)
                {
                    return NotFound("Không thể tìm thấy hóa đơn này hoặc hóa đơn đã được thanh toán.");
                }

                // Cập nhật trạng thái thanh toán của hóa đơn
                invoice.Status = -1;
                invoice.DateUpdated = DateTime.Now;
                var invoiceDetails = await _context.InvoiceDetails.SingleOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);


                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.InvoiceId == invoiceId);
                if (payment != null)
                {
                    payment.PaymentStatus = "Thanh toán không thành công";
                }
                else
                {

                    _context.Payments.Add(new Payment
                    {
                        InvoiceId = invoiceId,
                        AmountPaid = invoice.TotalPrice,
                        ServiceId = invoiceDetails.ServiceId,
                        ServiceName = invoiceDetails.ServiceName,
                        PaymentStatus = "Thanh toán tiền mặt thành công",
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Notes = "Thanh toán cho hoá đơn",
                        UserIdCreate = patient.PatientName.ToString()
                    });
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Thông báo thất bại và chuyển hướng
                TempData["ErrorMessage"] = "Thanh toán đã bị từ chối.";
                return RedirectToAction("PendingPayments");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có
                Console.WriteLine($"Error in RejectPayment: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi từ chối thanh toán.";
                return RedirectToAction("PendingPayments");
            }
        }
        [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
        public async Task<IActionResult> PendingPayments()
        {
            try
            {
                // Lấy tất cả các hóa đơn chưa thanh toán
                var invoices = await _context.Invoices
                    .Include(i => i.InvoiceDetails) // Bao gồm chi tiết hóa đơn
                    .Where(i => i.Status == 2 && i.PaymentMethod == "Tiền Mặt")
                    .OrderByDescending(p=>p.DateUpdated)// Chỉ lấy hóa đơn chưa thanh toán
                    .ToListAsync();

                // Kiểm tra có hóa đơn nào chưa thanh toán không
                if (!invoices.Any())
                {
                    TempData["ErrorMessage"] = "Không có hóa đơn nào cần thanh toán.";
                    return View(new List<InvoicePaymentViewModel>());
                }

                // Chuyển các hóa đơn chưa thanh toán thành InvoicePaymentViewModel
                var pendingPayments = invoices
                    .Select(i => new InvoicePaymentViewModel
                    {
                        InvoiceId = i.InvoiceId,
                        TotalAmount = i.InvoiceDetails.Sum(d => d.Invoice.TotalPrice * d.Quantity) - i.Discount,
                        Discount = i.Discount,
                        Status = i.Status,
                        DateUpdated = i.DateUpdated,
                    })
                    .OrderByDescending(p=>p.DateUpdated)
                    .ToList();

                // Kiểm tra sau khi lọc
                if (!pendingPayments.Any())
                {
                    TempData["ErrorMessage"] = "Không có hóa đơn nào đang chờ thanh toán.";
                }

                return View(pendingPayments);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có
                Console.WriteLine($"Error in PendingPayments: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách hóa đơn.";
                return View(new List<InvoicePaymentViewModel>());
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

        public async Task<IActionResult> GenerateQRCode(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return Json(new { success = false, message = "Hóa đơn không tồn tại." });
                }

                var payment = await _context.Payments.Where(p => p.InvoiceId == invoiceId).FirstOrDefaultAsync();
                var invoiceDetails = await _context.InvoiceDetails.Where(i => i.InvoiceId == invoiceId).ToListAsync();
                //payment.PaymentStatus = "Đang xử lý";
                //invoice.Status = 2;
                //invoice.Status = -1;
                await _context.SaveChangesAsync();
               
                decimal amount = invoice.TotalPrice;


                // Chuẩn bị danh sách các item (ví dụ dịch vụ)
                var items = new List<ItemData>();
                foreach (var invoiceDetail in invoiceDetails)
                {
                    decimal serviceAmount = amount;
                    items.Add(new ItemData(invoiceDetail.ServiceName, invoiceDetail.Quantity, (int)serviceAmount) 
                        );
                }

                // Tạo mã đơn hàng duy nhất cho việc thanh toán
                int orderCode = invoiceId;

                // Cấu hình dữ liệu thanh toán PayOS
                var paymentData = new PaymentData(
                    orderCode,
                    (int)amount,
                    "Thanh toán nha khoa", // Mô tả
                    items,
                    cancelUrl: "https://localhost:44392/Payment/Index", // URL hủy
                    returnUrl: "https://localhost:44392/Payment/HandlePaymentReturn" // URL thành công
                );

                // Khởi tạo đối tượng PayOS với thông tin xác thực của bạn
                PayOS payOS = new PayOS(clientId, apiKey, checksumKey);

                // Tạo liên kết thanh toán qua API PayOS
                CreatePaymentResult createPaymentResult = await payOS.createPaymentLink(paymentData);

                if (createPaymentResult == null)
                {
                    return Json(new { success = false, message = "Không thể tạo mã QR PayOs." });
                }

                // Sử dụng qrCodeUrl nếu thuộc tính này tồn tại trong kết quả trả về từ PayOS
                string qrCodeUrl = createPaymentResult.checkoutUrl; // Dùng qrCodeUrl thay vì checkoutUrl nếu có

                if (string.IsNullOrEmpty(qrCodeUrl))
                {
                    return Json(new { success = false, message = "Không thể tạo mã QR." });
                }

                string paymentLink = createPaymentResult.paymentLinkId; // Sử dụng paymentLinkId nếu cần

                // Trả về thông tin mã QR và liên kết thanh toán dưới dạng JSON
                ViewBag.qrCodeImageUrl = qrCodeUrl;
                return Json(new { success = true, qrCodeImageUrl = qrCodeUrl, paymentLinkId = paymentLink });
            }
            catch (Exception ex)
            {
                // Log các lỗi nếu có
                Console.WriteLine($"Error generating QR code: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tạo mã QR với PayOs." });
            }
        }
        [HttpGet]
        [Route("Payment/HandlePaymentReturn")]
        public async Task<IActionResult> HandlePaymentReturn(
            string code,
            string id,
            bool cancel,
            string status,
            string orderCode)
        {
            try
            {
                // Kiểm tra mã lỗi
                if (code != "00")
                {
                    return Json(new { success = false, message = "Thanh toán không thành công." });
                }

                // Kiểm tra trạng thái thanh toán
                if (status == "CANCELLED")
                {
                    // Cập nhật trạng thái hóa đơn là hủy
                    var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == Int32.Parse(orderCode));
                    if (invoice == null)
                    {
                        return NotFound(new { success = false, message = "Hóa đơn không tồn tại." });
                    }
                    invoice.Status = -1; // Trạng thái "CANCELLED"
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Thanh toán đã bị hủy." });
                }

                // Xử lý các trạng thái thanh toán khác
                if (status == "PAID")
                {
                    // Cập nhật trạng thái hóa đơn là đã thanh toán
                    var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == Int32.Parse(orderCode));
                    if (invoice == null)
                    {
                        return NotFound(new { success = false, message = "Hóa đơn không tồn tại." });
                    }
                    
                    var pres = await _prescriptionRepository.GetByIdAsync(invoice.PrescriptionId);
                    if(pres == null)
                    {

                        // Cập nhật trạng thái hóa đơn và thông tin thanh toán
                        var invoiceDetailss = await _context.InvoiceDetails.SingleOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);
                        invoice.Status = 3; // Trạng thái "PAID"
                        invoice.PaymentMethod = Paymethod.BANKING;
                        invoice.FinishTime = DateTime.Now;
                        var paymentNoPres = new Payment
                        {
                            InvoiceId = invoice.InvoiceId,
                            PaymentMethod = Paymethod.BANKING, // Hoặc phương thức thanh toán tương ứng
                            PaymentStatus = "Thanh toán thành công với PayOS",
                            ServiceId = invoiceDetailss.ServiceId,
                            ServiceName = invoiceDetailss.ServiceName,
                            AmountPaid = invoiceDetailss.TotalPrice,
                            Notes = "Thanh toán cho hoá đơn không có đơn thuốc",
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                        };
                        var existingPaymentNoPres = await _context.Payments.FirstOrDefaultAsync(p => p.InvoiceId == invoice.InvoiceId && p.PaymentMethod == Paymethod.BANKING);
                        if (existingPaymentNoPres == null)
                        {
                            await _context.Payments.AddAsync(paymentNoPres);
                            await _context.SaveChangesAsync();
                        }
                        //return Json(new { success = true, message = "Thanh toán thành công." });
                        return Redirect("/Payment/");
                    }
                    decimal totalPres = 0;
                    if (pres.PrescriptionId > 0 && pres != null) // Chỉ xử lý đơn thuốc nếu PrescriptionId hợp lệ
                    {
                        var presD = await _prescriptionDRepository.GetByPrescriptionIdAsync(pres.PrescriptionId);
                        if (pres == null)
                            return NotFound("Đơn thuốc không tồn tại.");

                        var presDetails = await _prescriptionDRepository.GetByPrescriptionIdAsync(pres.PrescriptionId);

                        // Duyệt qua tất cả chi tiết đơn thuốc để tính tổng
                        foreach (var item in presDetails)
                        {
                            totalPres += item.SalePrice * item.Quantity;
                        }
                    }
                    var userData = User.GetUserData();
                    // Cập nhật trạng thái hóa đơn và thông tin thanh toán
                    var invoiceDetails = await _context.InvoiceDetails.SingleOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);
                    invoice.Status = 3; // Trạng thái "PAID"
                    invoice.PaymentMethod = Paymethod.BANKING;
                    invoice.FinishTime = DateTime.Now;
                    var payment = new Payment
                    {
                        InvoiceId = invoice.InvoiceId,
                        PaymentMethod = Paymethod.BANKING, // Hoặc phương thức thanh toán tương ứng
                        PaymentStatus = "Thanh toán thành công với PayOS",
                        ServiceId = invoiceDetails.ServiceId,
                        ServiceName = invoiceDetails.ServiceName,
                        AmountPaid = invoiceDetails.TotalPrice + totalPres ,
                        Notes = "Thanh toán cho hoá đơn có đơn thuốc",
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                    };
                    var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.InvoiceId == invoice.InvoiceId && p.PaymentMethod == Paymethod.BANKING);
                    if (existingPayment == null)
                    {
                        await _context.Payments.AddAsync(payment);
                        await _context.SaveChangesAsync();
                    }
                    //return Json(new { success = true, message = "Thanh toán thành công." });
                    return Redirect("/Payment/");
                }

                return Json(new { success = false, message = "Trạng thái không hợp lệ." });
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về thông báo lỗi
                Console.WriteLine($"Error processing payment return: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi xử lý thông tin thanh toán." });
            }
        }
        [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee},{WebUserRoles.Patient}")]
        public async Task<IActionResult> PaymentHistory(int patientId, int invoiceId)
        {
            try
            {
                var userData = User.GetUserData();
                patientId = Int32.Parse(userData.UserId);
                // Lấy tất cả các hóa đơn liên quan đến bệnh nhân
                var payments = await _paymentRepository.GetPaymentsByPatientIdAsync(patientId);
                var invoices = _context.Invoices.Where(i => i.PatientId == patientId).ToList();

                if (payments == null || payments.Count == 0)
                {
                    return Json(new { success = false, message = "Không có lịch sử thanh toán cho bệnh nhân này." });
                }
                var viewModel = new PaymentHistoryViewModel
                {
                    Payments = payments,
                    Invoices = invoices
                };
                // Trả về danh sách các khoản thanh toán của bệnh nhân dưới dạng JSON
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về thông báo lỗi
                Console.WriteLine($"Error fetching patient payment history: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi lấy lịch sử thanh toán." });
            }
        }

    }
}
