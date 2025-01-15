using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using DocumentFormat.OpenXml.InkML;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DentalManagement.Web.Controllers
{
   
    public class InvoiceController : Controller
    {
        private readonly DentalManagementDbContext _dentalManagementDbContext;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<Dentist> _dentistRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IInvoiceDetails _invoiceDetailsRepository;
        private readonly IPayment _paymentRepository;
        private readonly PrescriptionRepository _prescriptionRepository;
        private readonly PrescriptionDetailsRepository _prescriptionDRepository;
        private const string APPOINTMENT_SEARCH = "appointment_search";
        private const string SEARCH_CONDITION = "invoice_search";
        private const string SERVICE_SEARCH_CONDITION = "service_search";
        private const string TREATMENT_VOUCHER = "treatment_voucher";
        int PAGE_SIZE = 9;
        int APPOINMENT_PAGE_SIZE = 9;

        public InvoiceController(DentalManagementDbContext dentalManagementDbContext, IInvoiceRepository invoiceRepository,
            IRepository<Patient> patientRepository, IRepository<Service> serviceRepository, IRepository<Dentist> dentistRepository,
            IRepository<Employee> employeeRepository, IAppointmentRepository appointmentRepository, IInvoiceDetails invoiceDetailsRepository
            , IPayment paymentRepository, PrescriptionRepository prescriptionRepository, PrescriptionDetailsRepository prescriptionDRepository)
        {
            _dentalManagementDbContext = dentalManagementDbContext;
            _invoiceRepository = invoiceRepository;
            _patientRepository = patientRepository;
            _serviceRepository = serviceRepository;
            _dentistRepository = dentistRepository;
            _employeeRepository = employeeRepository;
            _appointmentRepository = appointmentRepository;
            _invoiceDetailsRepository = invoiceDetailsRepository;
            _paymentRepository = paymentRepository;
            _prescriptionDRepository = prescriptionDRepository;
            _prescriptionRepository = prescriptionRepository;

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var input = ApplicationContext.GetSessionData<InvoiceSearchInput>(SEARCH_CONDITION);

                // Cập nhật các giá trị cho input
                if (input == null)
                {
                    input = new InvoiceSearchInput()
                    {
                        Page = 1,
                        PageSize = PAGE_SIZE,
                        SearchValue = ""
                    };
                }

                return View(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View();
            }
        }
        public async Task<IActionResult> Search(InvoiceSearchInput input)
        {
            try
            {
                // Start with the base query
                var query = _dentalManagementDbContext.Invoices.AsQueryable();

                // Apply status filter if specified
                if (input.Status != 0)
                {
                    query = query.Where(e => e.Status == input.Status);
                }

                // Apply date range filter if specified
                if (!string.IsNullOrEmpty(input.DateRange))
                {
                    query = query.Where(e => e.DateCreated <= input.ToTime && e.DateCreated >= input.FromTime);
                }

                // Apply search value filter if specified
                if (!string.IsNullOrEmpty(input.SearchValue))
                {
                    var searchValueUpper = input.SearchValue.ToUpper();
                    query = query.Where(e => e.PatientName.ToUpper().Contains(searchValueUpper));
                }

                // Get the total row count before pagination
                int rowCount = await query.CountAsync();

                // Apply pagination
                var data = await query
                    .OrderByDescending(e => e.DateCreated) // Apply ordering before pagination
                    .Skip((input.Page - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .ToListAsync();

                // Prepare the result model
                var model = new InvoiceSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue ?? "",
                    RowCount = rowCount,
                    Invoices = data
                };

                // Save search condition in session (if needed)
                ApplicationContext.SetSessionData(SEARCH_CONDITION, input);

                // Return the view with the result model
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while searching invoices: {ex.Message}");

                // Redirect to an error page or handle error appropriately
                return RedirectToAction("Error", new { message = "There was an issue processing your request." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetServicePrice(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);

            if (service != null)
            {
                return Json(new { price = service.Price });
            }
            return Json(new { price = 0 });
        }



        public async Task<IActionResult> Create()
        {
            var input = ApplicationContext.GetSessionData<ServiceSearchInput>(SERVICE_SEARCH_CONDITION);

            // Sử dụng phương thức bất đồng bộ để lấy danh sách bệnh nhân và đơn thuốc
            ViewBag.PatientList = SelectListHelper.GetPatients(_patientRepository);
            ViewBag.Prescription = await SelectListHelper.GetPrescriptions(_prescriptionRepository);

            // Nếu `input` không tồn tại, khởi tạo giá trị mặc định
            if (input == null)
            {
                input = new ServiceSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                };
            }

            return View(input);
        }

        public IEnumerable<Service> listService(out int rowCount, string searchValue = "")
        {
            var data = _serviceRepository.ListAlll(searchValue ?? "").Result;
            rowCount = data.Count();
            return data;
        }
        public async Task<IActionResult> SearchService(ServiceSearchInput input)
        {
            /*if (string.IsNullOrWhiteSpace(input.SearchValue))
            {
                // Trường hợp không tìm kiếm, hiển thị toàn bộ dịch vụ mặc định
                input.Page = 1; // Đặt trang mặc định
                input.PageSize = 5; // Số lượng dịch vụ mặc định
            }*/
            var rowCount = 1;
            listService(out rowCount, input.SearchValue);
            var data = await _serviceRepository.GetAllAsync(input.Page, input.PageSize, input.SearchValue);

            var model = new ServiceSearchResult
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SERVICE_SEARCH_CONDITION, input);
            return View(model);
        }
        public List<InvoiceCreateModel> GetTreatmentVoucher()
        {
            var treatmentVoucher = ApplicationContext.GetSessionData<List<InvoiceCreateModel>>(TREATMENT_VOUCHER);
            if (treatmentVoucher == null)
            {
                treatmentVoucher = new List<InvoiceCreateModel>();
                ApplicationContext.SetSessionData(TREATMENT_VOUCHER, treatmentVoucher);
            }
            return treatmentVoucher;
        }
        public IActionResult ShowTreatmentVoucher()
        {
            var model = GetTreatmentVoucher();
            return View(model);
        }

        public List<int> listSV = new List<int>();
        public async Task<IActionResult> AddToTreatmentVoucher(InvoiceCreateModel data)
        {

            if (data.SalePrice <= 0 && data.Quantity <= 0)
                return Json("phí dịch vụ và số lượng không hợp lệ ");
            var treatmentVoucher = GetTreatmentVoucher();
            var existsService = treatmentVoucher.FirstOrDefault(m => m.ServiceId == data.ServiceId);
            if (existsService == null)
            {
                treatmentVoucher.Add(data);
            }
            else
            {
                existsService.Quantity += data.Quantity;
            }
            listSV.Add(data.ServiceId);
            ApplicationContext.SetSessionData(TREATMENT_VOUCHER, treatmentVoucher);
            return Json("");
        }


        public IActionResult RemoveFromVoucher(int id = 0)
        {
            var treatmentVoucher = GetTreatmentVoucher();
            int index = treatmentVoucher.FindIndex(m => m.ServiceId == id);
            if (index >= 0)
                treatmentVoucher.RemoveAt(index);
            ApplicationContext.SetSessionData(TREATMENT_VOUCHER, treatmentVoucher);
            return Json("");
        }
        public IActionResult ClearVoucher()
        {
            var treatmentVoucher = GetTreatmentVoucher();
            treatmentVoucher.Clear();
            ApplicationContext.SetSessionData(TREATMENT_VOUCHER, treatmentVoucher);
            return Json("");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetByIdAsync(id);
            var patient = invoice.PatientId;
            var patients = await _patientRepository.GetByIdAsync(patient);

            if (invoice == null)
            {
                return NotFound();
            }

            // Convert the Invoice model to InvoiceViewModel
            var model = new InvoiceViewModel
            {
                InvoiceId = invoice.InvoiceId,
                PatientId = invoice.PatientId,
                PatientName = patients.PatientName,
                PatientAddress = invoice.Patient.Address, // Add this
                TotalPrice = invoice.TotalPrice,
                EmployeeId = invoice.EmployeeId,
                PaymentMethod = invoice.PaymentMethod,
                Status = invoice.Status,
            };

            return View(model);
        }
        public async Task<IActionResult> Details(int id = 0)
        {
            try
            {
                // Khởi tạo các biến điều khiển trạng thái trên giao diện
                ViewBag.IsFinish = false;
                ViewBag.IsDelete = false;
                ViewBag.IsEmployee = false;
                ViewBag.IsUnPaid = false;

                // Lấy dữ liệu người dùng hiện tại
                var userData = User.GetUserData();

                // Lấy thông tin hóa đơn từ repository
                var invoice = await _invoiceRepository.GetByIdAsync(id);

                var invoiceDetails = await _dentalManagementDbContext.Invoices
                    .Include(i => i.Patient)
                    .Include(i => i.Employee)
                    .Include(i => i.InvoiceDetails)
                    .ThenInclude(idetail => idetail.Service)
                    // Nếu bạn cần thông tin dịch vụ
                    .FirstOrDefaultAsync(i => i.InvoiceId == id);

                if (invoiceDetails == null)
                {
                    return RedirectToAction("Index");
                }
                if (invoice == null)
                {
                    return RedirectToAction("Index");
                }

                // Kiểm tra nếu người dùng hiện tại là nhân viên liên quan đến hóa đơn
                if (userData.UserId.Equals(invoice.EmployeeId.ToString()))
                {
                    ViewBag.IsEmployee = true;
                }

                // Cập nhật các biến ViewBag dựa trên trạng thái của hóa đơn
                switch (invoiceDetails.Status)
                {
                    case Constants_Invoice.INVOICE_UNPAID:
                        ViewBag.IsDelete = true;
                        ViewBag.IsUnPaid = true;
                        break;
                    case Constants_Invoice.INVOICE_PROCESSING:
                        ViewBag.IsUnPaid = true;
                        break;
                    case Constants_Invoice.INVOICE_PAID:
                        ViewBag.IsFinish = true;
                        break;
                    case Constants_Invoice.INVOICE_CANCELLED:
                    case Constants_Invoice.INVOICE_FAILED:
                        ViewBag.IsFinish = true;
                        ViewBag.IsDelete = true;
                        break;
                }

                // Lấy danh sách chi tiết hóa đơn từ repository
                var invoiceDetailViewModels = invoiceDetails.InvoiceDetails?.Select(d => new InvoiceDetailViewModel
                {
                    ServiceName = d.ServiceName,
                    Quantity = d.Quantity,
                    SalePrice = d.SalePrice,
                    TotalPrice = d.TotalPrice,

                }).ToList();

                // Lấy thông tin thanh toán từ repository
                var payments = await _paymentRepository.GetPaymentsByInvoiceIdAsync(invoice.InvoiceId);
                var paymentViewModels = payments.Select(p => new PaymentViewModel
                {
                    PaymentId = p.PaymentId,
                    InvoiceId = p.InvoiceId,
                    PaymentStatus = p.PaymentStatus,
                    AmountPaid = p.AmountPaid,
                    DateCreated = p.DateCreated,
                    Notes = p.Notes
                }).ToList();

                // Lấy danh sách thông tin đơn thuốc (PrescriptionDetails)
                var prescriptionDetails = await _dentalManagementDbContext.Prescriptions
                    .Where(p => p.PrescriptionId == invoice.PrescriptionId) // Assuming there's a link to InvoiceId or PatientId
                    .SelectMany(p => p.PrescriptionDetails)
                    // .FirstOrDefaultAsync(p => p.PrescriptionId == invoice.PrescriptionId)
                    // Assuming PrescriptionDetails is a collection of medicine details
                    .Select(pd => new PrescriptionCreateModel
                    {
                        PrescriptionId = pd.PrescriptionId,
                        MedicineName = pd.MedicineName,
                        Quantity = pd.Quantity,
                        SalePrice = pd.SalePrice,
                        TotalPrice = pd.TotalMedicine
                    }).ToListAsync();

                // Khởi tạo model để truyền dữ liệu cho view
                var model = new InvoiceDetailModel
                {
                    Invoices = invoiceDetails,
                    Details = invoiceDetailViewModels,
                    Payments = paymentViewModels,
                    PrescriptionDetails = prescriptionDetails  // Populate prescription details here
                };

                // Truyền thông báo (nếu có) từ TempData
                if (TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"];
                }

                // Trả về view với model
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }




        public async Task<IActionResult> ConfirmPayment(int id = 0)
        {
            bool result = await _invoiceRepository.UnPaid(id);
            if (!result)
                TempData["Message"] = "Không thể xác nhận hoá đơn này ";
            return RedirectToAction("Details", new { id });
        }
        public async Task<IActionResult> Finish(int id = 0)
        {
            bool result = await _invoiceRepository.PaidInvoice(id);
            if (!result)
                TempData["Message"] = "Không thể thanh toán hoá đơn này ";
            return RedirectToAction("Index", new { id });
        }
        public async Task<IActionResult> Cancel(int id = 0)
        {
            bool result = await _invoiceRepository.CancelInvoice(id);
            if (!result)
                TempData["Message"] = "";
            return RedirectToAction("Details", new { id });
        }
        public async Task<IActionResult> Failed(int id = 0)
        {
            bool result = await _invoiceRepository.FailedInvoice(id);
            if (!result)
                TempData["Message"] = "";
            return RedirectToAction("Details", new { id });
        }
        public async Task<IActionResult> Delete(int id = 0)
        {
            try
            {
                // Step 1: Get the invoice to be deleted
                var invoice = await _dentalManagementDbContext.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    TempData["Message"] = "Hoá đơn không tồn tại.";
                    return RedirectToAction("Index");
                }

                // Step 2: Get the related payments
                var payments = await _dentalManagementDbContext.Payments
                    .Where(p => p.InvoiceId == id)
                    .ToListAsync();

                // Step 3: Delete related payments
                _dentalManagementDbContext.Payments.RemoveRange(payments);

                // Step 4: Delete the invoice
                _dentalManagementDbContext.Invoices.Remove(invoice);
                await _dentalManagementDbContext.SaveChangesAsync();

                // Step 5: Return to Index after successful deletion
                TempData["Message"] = "Hoá đơn đã được xoá thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the delete operation
                TempData["Message"] = "Có lỗi xảy ra khi xoá hoá đơn: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        public async Task<IActionResult> Init(int patientId, IEnumerable<InvoiceCreateModel> details, int prescriptionId)
        {
            try
            {
                // Lấy thông tin các dịch vụ đã chọn từ giỏ hàng (treatment voucher)
                var treatmentVoucher = GetTreatmentVoucher();
                Console.WriteLine($"Treatment Voucher Count: {treatmentVoucher.Count}, Contents: {string.Join(", ", treatmentVoucher.Select(t => t.ServiceId))}");

                if (treatmentVoucher.Count == 0)
                    return BadRequest("Giỏ hàng trống, không thể lập đơn hàng.");

                if (patientId <= 0)
                    return BadRequest("Vui lòng nhập đầy đủ thông tin.");

                int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);
                var employee = await _employeeRepository.GetByIdAsync(employeeID);
                var patient = await _patientRepository.GetByIdAsync(patientId);

                if (employee == null || patient == null)
                    return NotFound("Nhân viên hoặc bệnh nhân không tồn tại.");

                decimal totalPres = 0;
                if (prescriptionId > 0) // Chỉ xử lý đơn thuốc nếu PrescriptionId hợp lệ
                {
                    var pres = await _prescriptionRepository.GetByIdAsync(prescriptionId);
                    if (pres == null)
                        return NotFound("Đơn thuốc không tồn tại.");

                    var presDetails = await _prescriptionDRepository.GetByPrescriptionIdAsync(pres.PrescriptionId);

                    // Duyệt qua tất cả chi tiết đơn thuốc để tính tổng
                    foreach (var item in presDetails)
                    {
                        totalPres += item.SalePrice * item.Quantity;
                    }
                }
                decimal totalAmount = treatmentVoucher.Sum(d => d.Quantity * d.SalePrice);


                decimal totalPaidAmount = totalAmount + totalPres;

                // Calculate discount  
                decimal discountPercentage = 0;
                if (totalPaidAmount >= 50000000)
                {
                    discountPercentage = 0.2m;
                }
                else if (totalPaidAmount >= 10000000)
                {
                    discountPercentage = 0.1m;
                }

                var discountAmount = totalPaidAmount * discountPercentage;
                var finalTotalAmount = totalPaidAmount - discountAmount;

                var invoice = new Invoice
                {
                    EmployeeId = employee.EmployeeId,
                    PatientId = patient.PatientId,
                    PrescriptionId = prescriptionId > 0 ? prescriptionId : 0, // Optional if prescription is null
                    PatientName = patient.PatientName,
                    EmployeeName = employee.FullName,
                    PatientAddress = patient.Address,
                    TotalPrice = finalTotalAmount,
                    Status = Constants_Invoice.INVOICE_UNPAID,
                    PaymentMethod = Paymethod.CASH,
                    DateCreated = DateTime.Now,
                    Discount = discountAmount
                };

                await _dentalManagementDbContext.Invoices.AddAsync(invoice);
                await _dentalManagementDbContext.SaveChangesAsync();

                int invoiceId = invoice.InvoiceId;

                foreach (var item in treatmentVoucher)
                {
                    var invoiceDetail = new InvoiceDetails
                    {
                        InvoiceId = invoiceId,
                        ServiceId = item.ServiceId,
                        ServiceName = item.ServiceName,
                        Quantity = item.Quantity,
                        PricePrescription = totalPres,
                        SalePrice = item.SalePrice,
                        PrescriptionId = item.PrescriptionId  // If prescription exists, set it
                    };

                    await _dentalManagementDbContext.InvoiceDetails.AddAsync(invoiceDetail);
                }

                await _dentalManagementDbContext.SaveChangesAsync();

                ClearVoucher();

                return Json($"Details/{invoiceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
        [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Patient},{WebUserRoles.Employee}")]
        public async Task<IActionResult> PrintInvoice(int id)
        {
            var invoices = await _invoiceRepository.GetByIdAsync(id);
            var invoiceDetails= await _dentalManagementDbContext.InvoiceDetails.SingleOrDefaultAsync(i=>i.InvoiceId == invoices.InvoiceId);
            invoiceDetails.PrescriptionId = invoices.PrescriptionId;
            if (invoiceDetails.PrescriptionId != 0)
            {
                var invoice = await _dentalManagementDbContext.Invoices
                .Include(i => i.Patient)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Service)
                .Include(i => i.Prescription)
                    .ThenInclude(p => p.PrescriptionDetails)
                        .ThenInclude(pd => pd.Medicine)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

                if (invoice == null)
                {
                    return NotFound("Hóa đơn không tồn tại.");
                }

                // Tổng tiền dịch vụ
                decimal totalService = invoice.InvoiceDetails.Sum(d => d.TotalPrice);

                // Tổng tiền thuốc nếu có đơn thuốc
                decimal totalMedicinePrice = 0;
                if (invoice.Prescription != null)
                {
                    totalMedicinePrice = invoice.Prescription.PrescriptionDetails.Sum(pd => pd.TotalMedicine);
                }

                // Tạo tài liệu PDF
                var document = new iTextSharp.text.Document(PageSize.A4);
                using (var ms = new MemoryStream())
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Nhúng font Times New Roman
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\times.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var font = new Font(baseFont, 12);
                    var boldFont = new Font(baseFont, 12, Font.BOLD);

                    // Thông tin phòng khám
                    document.Add(new Paragraph("PHÒNG KHÁM NHA KHOA VietClinic", boldFont));
                    document.Add(new Paragraph("Địa chỉ: 26 Lương Văn Can, An Cựu, TP-Huế", font));
                    document.Add(new Paragraph("Số điện thoại: 0879345947", font));
                    document.Add(new Paragraph("Email: nhakhoa.vietclinic@gmail.com", font));
                    document.Add(new Paragraph("\n"));

                    // Tiêu đề hóa đơn
                    document.Add(new Paragraph("HÓA ĐƠN THANH TOÁN", boldFont));
                    document.Add(new Paragraph("=================================", font));

                    // Thông tin hóa đơn
                    document.Add(new Paragraph($"Mã hóa đơn: {invoice.InvoiceId}", font));
                    document.Add(new Paragraph($"Ngày lập: {invoice.DateCreated:dd/MM/yyyy}", font));
                    document.Add(new Paragraph($"Người lập: {invoice.EmployeeName}", font));

                    // Thông tin bệnh nhân
                    document.Add(new Paragraph($"Bệnh nhân: {invoice.Patient.PatientName}", font));
                    document.Add(new Paragraph($"Địa chỉ: {invoice.Patient.Address}", font));
                    document.Add(new Paragraph($"Số điện thoại: {invoice.Patient.Phone}", font));
                    document.Add(new Paragraph("\n"));

                    // Thông tin dịch vụ
                    document.Add(new Paragraph("DANH SÁCH DỊCH VỤ", boldFont));
                    document.Add(new Paragraph("\n"));
                    var serviceTable = new PdfPTable(4) { WidthPercentage = 100 };
                    serviceTable.SetWidths(new float[] { 4, 2, 2, 2 });

                    serviceTable.AddCell(new PdfPCell(new Phrase("Tên dịch vụ", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    serviceTable.AddCell(new PdfPCell(new Phrase("Số lượng", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    serviceTable.AddCell(new PdfPCell(new Phrase("Đơn giá", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    serviceTable.AddCell(new PdfPCell(new Phrase("Thành tiền", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        serviceTable.AddCell(new PdfPCell(new Phrase(detail.Service.ServiceName, font)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        serviceTable.AddCell(new PdfPCell(new Phrase(detail.Quantity.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        serviceTable.AddCell(new PdfPCell(new Phrase(detail.SalePrice.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                        serviceTable.AddCell(new PdfPCell(new Phrase(detail.TotalPrice.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    document.Add(serviceTable);
                    document.Add(new Paragraph("\n"));

                    // Thông tin đơn thuốc nếu có
                    if (invoice.Prescription != null)
                    {
                        document.Add(new Paragraph("CHI TIẾT ĐƠN THUỐC", boldFont));
                        document.Add(new Paragraph("\n"));

                        var prescriptionTable = new PdfPTable(4) { WidthPercentage = 100 };
                        prescriptionTable.SetWidths(new float[] { 4, 2, 2, 2 });

                        prescriptionTable.AddCell(new PdfPCell(new Phrase("Tên thuốc", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        prescriptionTable.AddCell(new PdfPCell(new Phrase("Số lượng", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        prescriptionTable.AddCell(new PdfPCell(new Phrase("Đơn giá", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        prescriptionTable.AddCell(new PdfPCell(new Phrase("Thành tiền", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                        foreach (var detail in invoice.Prescription.PrescriptionDetails)
                        {
                            prescriptionTable.AddCell(new PdfPCell(new Phrase(detail.Medicine.MedicineName, font)) { HorizontalAlignment = Element.ALIGN_LEFT });
                            prescriptionTable.AddCell(new PdfPCell(new Phrase(detail.Quantity.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                            prescriptionTable.AddCell(new PdfPCell(new Phrase(detail.SalePrice.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                            prescriptionTable.AddCell(new PdfPCell(new Phrase(detail.TotalMedicine.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                        }

                        document.Add(prescriptionTable);
                        document.Add(new Paragraph("\n"));
                    }

                    // Tổng kết hóa đơn
                    // decimal totalPayment = totalService + totalMedicinePrice;
                    document.Add(new Paragraph("TỔNG KẾT", boldFont));
                    document.Add(new Paragraph($"Tổng tiền dịch vụ: {totalService:C}", font));
                    document.Add(new Paragraph($"Tổng tiền thuốc: {totalMedicinePrice:C}", font));
                    document.Add(new Paragraph($"Tổng thanh toán: {invoice.TotalPrice:C}", font));
                    document.Add(new Paragraph($"Hình thức thanh toán: {invoice.PaymentMethod}", font));

                    // Lời cảm ơn
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!", font));
                    document.Add(new Paragraph("Mọi thắc mắc xin vui lòng liên hệ với phòng khám.", font));

                    // Đóng tài liệu
                    document.Close();

                    // Trả về file PDF
                    var fileBytes = ms.ToArray();
                    return File(fileBytes, "application/pdf", $"HoaDon_{invoice.InvoiceId}_{invoice.Patient.PatientName}.pdf");
                }
            }
            else if (invoiceDetails.PrescriptionId == 0)
            {
                var invoice = await _dentalManagementDbContext.Invoices
      .Include(i => i.Patient)
      .Include(i => i.InvoiceDetails)
          .ThenInclude(d => d.Service)
      .FirstOrDefaultAsync(i => i.InvoiceId == id);

                if (invoice == null)
                {
                    return NotFound("Hóa đơn không tồn tại.");
                }

                // Tạo tài liệu PDF
                var document = new iTextSharp.text.Document(PageSize.A4);
                using (var ms = new MemoryStream())
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Nhúng font Times New Roman
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\times.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var font = new Font(baseFont, 12);
                    var boldFont = new Font(baseFont, 12, Font.BOLD);

                    // Thông tin phòng khám
                    document.Add(new Paragraph("PHÒNG KHÁM NHA KHOA VietClinic", boldFont));
                    document.Add(new Paragraph("Địa chỉ: 26 Lương Văn Can, An Cựu, TP-Huế", font));
                    document.Add(new Paragraph("Số điện thoại: 0879345947", font));
                    document.Add(new Paragraph("Email: nhakhoa.vietclinic@gmail.com", font));
                    document.Add(new Paragraph("\n"));

                    // Tiêu đề hóa đơn
                    document.Add(new Paragraph("HÓA ĐƠN THANH TOÁN", boldFont));
                    document.Add(new Paragraph("=================================", font));

                    // Thông tin hóa đơn
                    document.Add(new Paragraph($"Mã Hóa Đơn: {invoice.InvoiceId}", font));
                    document.Add(new Paragraph($"Ngày Lập: {invoice.DateCreated:dd/MM/yyyy}", font));
                    document.Add(new Paragraph($"Bệnh Nhân: {invoice.Patient.PatientName}", font));
                    document.Add(new Paragraph($"Tổng Tiền: {invoice.TotalPrice:C}", font));
                    document.Add(new Paragraph($"Hình thức thanh toán: {invoice.PaymentMethod}", font));
                    document.Add(new Paragraph("\n"));

                    // Thêm bảng chi tiết hóa đơn
                    var table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 3, 2, 2, 2 });

                    table.AddCell(new PdfPCell(new Phrase("Dịch Vụ", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Số Lượng", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Đơn Giá", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Thành Tiền", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    // Duyệt qua danh sách chi tiết hóa đơn và thêm vào bảng
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        table.AddCell(new PdfPCell(new Phrase(detail.Service.ServiceName, font)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        table.AddCell(new PdfPCell(new Phrase(detail.Quantity.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(detail.SalePrice.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(detail.TotalPrice.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    document.Add(table);

                    // Lời cảm ơn
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!", font));
                    document.Add(new Paragraph("Mọi thắc mắc xin vui lòng liên hệ với phòng khám.", font));

                    // Đóng tài liệu
                    document.Close();

                    // Trả về file PDF
                    var fileBytes = ms.ToArray();
                    return File(fileBytes, "application/pdf", $"HoaDon_{invoice.InvoiceId}_{invoice.Patient.PatientName}.pdf");
                }
        }
            else
            {
                return BadRequest("Không thể In hóa đơn.");
            }
        }

    }

}
