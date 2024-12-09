using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
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
        private const string APPOINTMENT_SEARCH = "appointment_search";
        private const string SEARCH_CONDITION = "invoice_search";
        private const string SERVICE_SEARCH_CONDITION = "service_search";
        private const string TREATMENT_VOUCHER = "treatment_voucher";
        int PAGE_SIZE = 9;
        int APPOINMENT_PAGE_SIZE = 9;

        public InvoiceController(DentalManagementDbContext dentalManagementDbContext, IInvoiceRepository invoiceRepository,
            IRepository<Patient> patientRepository, IRepository<Service> serviceRepository, IRepository<Dentist> dentistRepository,
            IRepository<Employee> employeeRepository, IAppointmentRepository appointmentRepository, IInvoiceDetails invoiceDetailsRepository
            , IPayment paymentRepository)
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
            ViewBag.PatientList = SelectListHelper.GetPatients(_patientRepository);
            if (input == null)
            {
                input = new ServiceSearchInput()
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
        

        //[HttpPost]
        //public async Task<IActionResult> Save(InvoiceViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Reload patient and service lists for the view
        //        model.PatientList = SelectListHelper.GetPatients(_patientRepository);
        //        model.ServiceList = SelectListHelper.GetServices(_serviceRepository);
        //        model.DentistList = SelectListHelper.GetDentists(_dentistRepository);
        //        return View("Edit", model); // Return the view with validation errors
        //    }
        //    var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        //    // Check if the invoice already exists
        //    Invoice invoice = model.InvoiceId == 0
        //        ? new Invoice()
        //        : await _invoiceRepository.GetByIdAsync(model.InvoiceId);

        //    if (invoice == null)
        //    {
        //        return NotFound();
        //    }
        //    var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        //    // Fetch patient and service data to ensure they exist
        //    var patient = await _patientRepository.GetByIdAsync(model.PatientId);
        //   // var invoiceD = await _invoiceDetailsRepository.GetByIdAsync(invoice.Service.ServiceId);
        //  //  var service = await _serviceRepository.GetByIdAsync(invoiceD.InvoiceId);
        //    if (patient == null || service == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Có thông tin không tồn tại.");
        //        return View(model);
        //    }
        //    var userData = User.GetUserData();
        //    var employeeId = int.Parse(userData.UserId);
        //    var employee = await _employeeRepository.GetByIdAsync(employeeId);


        //    // Update the invoice properties
        //    invoice.PatientId = model.PatientId;
        //    invoice.PatientName = patient.PatientName; // Set the patient's name
        //    invoice.PatientAddress = patient.Address;
        //    invoice.PaymentMethod = model.PaymentMethod ?? Paymethod.CASH;
        //    invoice.TotalPrice = model.TotalPrice;
        //    invoice.UserIdCreate = userData.UserId;
        //    invoice.EmployeeId = int.Parse(userData.UserId);
        //    invoice.EmployeeName = employee.FullName;
        //    invoice.Status = Constants_Invoice.INVOICE_UNPAID;
        //    invoice.Discount = model.Discount;
        //    invoice.DateUpdated = DateTime.Now;
        //    invoice.UserIdUpdated = User?.Identity?.Name;


        //    // If it's a new invoice, set the created date and creator
        //    if (model.InvoiceId == 0)
        //    {
        //        invoice.DateCreated = DateTime.Now;
        //        invoice.UserIdCreate = User?.Identity?.Name;
        //        await _invoiceRepository.AddAsync(invoice); // Add new invoice
        //    }
        //    else
        //    {
        //        invoice.DateUpdated = DateTime.Now;
        //        invoice.UserIdUpdated = User?.Identity?.Name;
        //        await _invoiceRepository.UpdateAsync(invoice); // Update existing invoice
        //    }

        //    await _dentalManagementDbContext.SaveChangesAsync(); // Save changes to the database
        //    TempData["Message"] = "Hóa đơn đã được cập nhật thành công!";

        //    return RedirectToAction("Index");
        //}
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
                // Khởi tạo model để truyền dữ liệu cho view
                var model = new InvoiceDetailModel
                {
                    Invoices = invoiceDetails,
                    Details = invoiceDetailViewModels,
                    Payments = paymentViewModels,
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

        public async Task<IActionResult> Init(int patientId, IEnumerable<InvoiceCreateModel> details)
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

                // Tính tổng số tiền từ giỏ hàng (dựa trên `details`)
                var totalAmount = treatmentVoucher.Sum(d => d.Quantity * d.SalePrice);

                // Tính tổng số tiền đã thanh toán cho các dịch vụ trong giỏ hàng
                decimal totalPaidAmount = 0;
                foreach (var item in treatmentVoucher)
                {
                    totalPaidAmount += item.SalePrice * item.Quantity;
                }

                // Kiểm tra tổng số tiền thanh toán đã thực hiện
                decimal discountPercentage = 0;

                // Nếu tổng tiền thanh toán >= 50 triệu, giảm 20%
                if (totalPaidAmount >= 50000000)
                {
                    discountPercentage = 0.2m; // Giảm 20%
                }
                // Nếu tổng tiền thanh toán >= 10 triệu nhưng < 50 triệu, giảm 10%
                else if (totalPaidAmount >= 10000000)
                {
                    discountPercentage = 0.1m; // Giảm 10%
                }

                // Tính số tiền giảm giá và số tiền cuối cùng sau khi giảm
                var discountAmount = totalAmount * discountPercentage;
                var finalTotalAmount = totalAmount - discountAmount;

                // Tạo hóa đơn mới
                var invoice = new Invoice
                {
                    EmployeeId = employee.EmployeeId,
                    PatientId = patient.PatientId,
                    PatientName = patient.PatientName,
                    EmployeeName = employee.FullName,
                    PatientAddress = patient.Address,
                    TotalPrice = finalTotalAmount,  // Sử dụng số tiền sau giảm giá
                    Status = Constants_Invoice.INVOICE_UNPAID,
                    PaymentMethod = Paymethod.CASH,  // Phương thức thanh toán mặc định
                    DateCreated = DateTime.Now,
                    Discount = discountAmount  // Lưu giá trị giảm giá vào hóa đơn
                };

                // Lưu hóa đơn vào database
                await _dentalManagementDbContext.Invoices.AddAsync(invoice);
                await _dentalManagementDbContext.SaveChangesAsync();

                // Lấy ID của hóa đơn mới tạo
                int invoiceId = invoice.InvoiceId;

                // Thêm chi tiết hóa đơn vào bảng InvoiceDetails
                foreach (var item in treatmentVoucher)
                {
                    var invoiceDetail = new InvoiceDetails
                    {
                        InvoiceId = invoiceId,
                        ServiceId = item.ServiceId,
                        ServiceName = item.ServiceName,
                        Quantity = item.Quantity,
                        SalePrice = item.SalePrice,
                    };

                    await _dentalManagementDbContext.InvoiceDetails.AddAsync(invoiceDetail);
                    //await _dentalManagementDbContext.Payments.AddAsync(payment);
                }

                await _dentalManagementDbContext.SaveChangesAsync();

                // Xóa giỏ hàng sau khi lập hóa đơn
                ClearVoucher();

                // Trả về view để hiển thị thông tin hóa đơn và chi tiết hóa đơn
                return Json($"Details/{invoiceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        
    }
}
