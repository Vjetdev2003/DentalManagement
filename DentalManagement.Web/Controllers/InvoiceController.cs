using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DentalManagement.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Dentist},{WebUserRoles.Employee}")]
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

                var data = await _dentalManagementDbContext.Invoices.ToListAsync();
                if (input.Status != 0)
                {
                    data = data.Where(e => e.Status == input.Status).ToList();


                }
                if (!string.IsNullOrEmpty(input.DateRange))
                {
                    data = data.Where(e => e.DateCreated <= input.ToTime && e.DateCreated >= input.FromTime).ToList();
                }
                if (!string.IsNullOrEmpty(input.SearchValue))
                {
                    data = data.Where(e => e.PatientName.ToUpper().Contains(input.SearchValue.ToUpper())).ToList();
                }
                int rowCount = data.Count();

                data = data.Skip((input.Page - 1) * input.PageSize)
                    .OrderByDescending(e => e.DateCreated)
                   .Take(input.PageSize).ToList();




                var model = new InvoiceSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue ?? "",
                    RowCount = rowCount,
                    Invoices = data
                };
                ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
                return View(model);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc hiển thị thông báo lỗi
                Console.WriteLine($"Error while searching appointments: {ex.Message}");

                // Bạn có thể trả về một trang lỗi hoặc xử lý lỗi tùy ý
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
            ViewBag.PatientList =   SelectListHelper.GetPatients(_patientRepository);
            ViewBag.Prescription =   await SelectListHelper.GetPrescriptions(_prescriptionRepository);

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
                    .ThenInclude(idetail => idetail.Service) // Nếu bạn cần thông tin dịch vụ
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
                    .Where(p => p.PatientId == invoiceDetails.PatientId) // Assuming there's a link to InvoiceId or PatientId
                    .SelectMany(p => p.PrescriptionDetails) // Assuming PrescriptionDetails is a collection of medicine details
                    .Select(pd => new PrescriptionCreateModel
                    {
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
                var pres = await _prescriptionRepository.GetByIdAsync(prescriptionId);
                var presDetails = await _prescriptionDRepository.GetByPrescriptionIdAsync(pres.PrescriptionId);

                decimal totalPres = 0;

                // Duyệt qua tất cả chi tiết đơn thuốc để tính tổng
                foreach (var item in presDetails)  // presDetails là danh sách chi tiết đơn thuốc
                {
                    totalPres += item.SalePrice * item.Quantity;
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
                    PrescriptionId = prescriptionId, // Optional if prescription is null
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

        //[HttpGet]
        //public async Task<IActionResult> GetLatestPrescriptionByPatientId(int patientId)
        //{
        //    var latestPrescription = await _dentalManagementDbContext.Prescriptions
        //        .Where(p => p.PatientId == patientId)
        //        .OrderByDescending(p => p.DateCreated) // Sắp xếp theo ngày tạo mới nhất
        //        .Select(p => new { p.PrescriptionId, p.DateCreated })
        //        .FirstOrDefaultAsync();
             
        //    if (latestPrescription == null)
        //    {
        //        return Json(new { success = false, message = "Không tìm thấy đơn thuốc cho bệnh nhân này." });
        //    }

        //    return Json(new { success = true, data = latestPrescription });
        //}



    }
}
