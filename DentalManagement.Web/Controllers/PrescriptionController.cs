using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace DentalManagement.Web.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly DentalManagementDbContext _context;
        private readonly IRepository<Medicine> _medicineRepo;
        private readonly IRepository<Dentist> _dentistRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly PrescriptionRepository _prescriptionRepo;
        private readonly MedicalRecordRepository _medicalRecordRepo;
        private const int PAGE_SIZE = 10;
        private string SEARCH_PRE = "prescription_search";
        private const string PRESCRIPTION_MEDICINE = "prescription_medicine";
        public PrescriptionController(DentalManagementDbContext context,IRepository<Medicine> medicineRepo,
            IRepository<Dentist> dentistRepo, IRepository<Patient> patientRepo,PrescriptionRepository prescriptionRepo, MedicalRecordRepository medicalRecordRepo)
        {
            _context = context;
            _medicineRepo = medicineRepo;
            _dentistRepo = dentistRepo;
            _patientRepo = patientRepo;
            _prescriptionRepo = prescriptionRepo;
            _medicalRecordRepo = medicalRecordRepo;
        }
        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_PRE);

            // Cập nhật các giá trị cho input
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            try
            {
                int rowCount = await _context.Prescriptions
           .Where(e => string.IsNullOrEmpty(input.SearchValue))
           .CountAsync();
                var data = await _context.Prescriptions
                    .Include(p=>p.Patient)
                    .Include(p=>p.Dentist)
                    .Where(e => string.IsNullOrEmpty(input.SearchValue))
                    .Skip((input.Page - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .ToListAsync();
                var model = new PrescriptionSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue ?? "",
                    RowCount = rowCount,
                    Prescriptions = data
                };
                ApplicationContext.SetSessionData(SEARCH_PRE, input);
                return View(model);
            }
            catch (Exception ex) { 
                return View(ex);
            }
        }
        public async Task<IActionResult> Create()
        {
            try
            {
                var input = ApplicationContext.GetSessionData<MedicineSearchInput>(SEARCH_PRE);
                ViewBag.PatientList = SelectListHelper.GetPatients(_patientRepo);
                if (input == null)
                {
                    input = new MedicineSearchInput()
                    {
                        Page = 1,
                        PageSize = PAGE_SIZE,
                        SearchValue = "",
                    };
                }
                var prescriptionModel = new PrescriptionCreateModel()
                {
                    Diagnosis = "",       // Default empty, you can update with prefilled data if available
                    Dosage = "",    // Default empty
                    Duration = "",
                    Frequency = "",
                    Notes = ""
                };

                // Combine the MedicineSearchInput and PrescriptionCreateModel
                // You may need to use a more complex model depending on your view requirements
                var combinedModel = new PrescriptionViewModel
                {
                    MedicineSearchInput = input,
                    PrescriptionCreateModel = prescriptionModel
                };

                return View(combinedModel);
            }
            catch (Exception ex) { 
                return View(ex.Message);
            }
        }
        public IEnumerable<Medicine> listMedicine(out int rowCount, string searchValue = "")
        {
            var data = _medicineRepo.ListAlll(searchValue ?? "").Result;
            rowCount = data.Count();
            return data;
        }
        public async Task<IActionResult> SearchMedicine(PaginationSearchInput input)
        {
            /*if (string.IsNullOrWhiteSpace(input.SearchValue))
            {
                // Trường hợp không tìm kiếm, hiển thị toàn bộ dịch vụ mặc định
                input.Page = 1; // Đặt trang mặc định
                input.PageSize = 5; // Số lượng dịch vụ mặc định
            }*/
            var rowCount = 1;
            listMedicine(out rowCount, input.SearchValue);
            var data = await _medicineRepo.GetAllAsync(input.Page, input.PageSize, input.SearchValue);

            var model = new MedicineSearchResult
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_PRE, input);
            return View(model);
        }
        public List<PrescriptionMedicineViewModel> GetMedicine()
        {
            var prescription_medicine = ApplicationContext.GetSessionData<List<PrescriptionMedicineViewModel>>(PRESCRIPTION_MEDICINE);
            if (prescription_medicine == null)
            {
                prescription_medicine = new List<PrescriptionMedicineViewModel>();
                ApplicationContext.SetSessionData(PRESCRIPTION_MEDICINE, prescription_medicine);
            }
            return prescription_medicine;
        }
        public IActionResult ShowPrescription()
        {
            var model = GetMedicine();
            return View(model);
        }

        public List<int> listSV = new List<int>();
        public async Task<IActionResult> AddToPrescription(PrescriptionMedicineViewModel data)
        {

            if (data.MedicinePrice <=0 ||data.Quantity <= 0)
                return Json(" số lượng và giá không hợp lệ ");
            var prescription_medicine = GetMedicine();
            var existsMedicine = prescription_medicine.FirstOrDefault(p => p.MedicineId == data.MedicineId);
            if (existsMedicine == null)
            {
                prescription_medicine.Add(data);
            }
            else
            {
                existsMedicine.Quantity += data.Quantity;
            }
            listSV.Add(data.MedicineId);
            ApplicationContext.SetSessionData(PRESCRIPTION_MEDICINE, prescription_medicine);
            return Json("");
        }


        public IActionResult RemoveFromPrescription(int id = 0)
        {
            var prescription_medicine = GetMedicine();
            int index = prescription_medicine.FindIndex(m => m.MedicineId == id);
            if (index >= 0)
                prescription_medicine.RemoveAt(index);
            ApplicationContext.SetSessionData(PRESCRIPTION_MEDICINE, prescription_medicine);
            return Json("");
        }
        public IActionResult ClearPrescription()
        {
            var prescription_medicine = GetMedicine();
            prescription_medicine.Clear();
            ApplicationContext.SetSessionData(PRESCRIPTION_MEDICINE, prescription_medicine);
            return Json("");
        }
        public async Task<IActionResult> Details(int id)
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
                var pres = await _prescriptionRepo.GetByIdAsync(id);
                var dentist = await _context.Dentists.SingleOrDefaultAsync(d=>d.DentistId == pres.DentistId);
                var patient = await _context.Patients.SingleOrDefaultAsync(d=>d.PatientId == pres.PatientId);
                var presDetails = await _context.Prescriptions
                       .Include(i => i.Patient)
                       .Include(i => i.Dentist)
                       .Include(i => i.PrescriptionDetails)
                       .ThenInclude(idetail => idetail.Medicine) // Nếu bạn cần thông tin dịch vụ
                       .FirstOrDefaultAsync(i => i.PrescriptionId == id);
                if (presDetails == null)
                {
                    return RedirectToAction("Index");
                }
                if (pres == null)
                {
                    return RedirectToAction("Index");
                }

                // Lấy danh sách chi tiết hóa đơn từ repository

                var presDetailViewModel = presDetails.PrescriptionDetails?.Select(d => new PrescriptionMedicineViewModel
                {
                    MedicineName = d.MedicineName,
                    Quantity = d.Quantity,
                    MedicinePrice = d.MedicinePrice,
                    TotalMedicine= d.TotalMedicine,

                }).ToList();
                // Khởi tạo model để truyền dữ liệu cho view
                var model = new PrescriptionDetailModel
                {
                    Prescriptions = presDetails,
                    Details = presDetailViewModel,
                    Dentists = dentist,
                    Patients = patient
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
        public async Task<IActionResult> Init(int patientId, IEnumerable<PrescriptionMedicineViewModel> details, PrescriptionViewModel model)
        {
            try
            {
                // Log thông tin đầu vào
                Console.WriteLine("Begin Init - Start");
                Console.WriteLine($"PatientId: {patientId}");

                // Kiểm tra giỏ hàng (treatment voucher)
                var prescription_medicine = GetMedicine();
                Console.WriteLine($"Treatment Voucher Count: {prescription_medicine.Count}, Contents: {string.Join(", ", prescription_medicine.Select(t => t.MedicineId))}");
                if (prescription_medicine.Count == 0)
                    return BadRequest("Chưa có danh sách thuốc, không thể lập đơn thuốc.");

                if (patientId <= 0)
                    return BadRequest("Vui lòng nhập đầy đủ thông tin.");

                // Lấy thông tin Dentist và Patient
                int dentistId = Convert.ToInt32(User.GetUserData()?.UserId);
                var dentist = await _dentistRepo.GetByIdAsync(dentistId);
                var patient = await _patientRepo.GetByIdAsync(patientId);

                if (dentist == null || patient == null)
                    return NotFound("Nhân viên hoặc bệnh nhân không tồn tại.");

                // Xác nhận Patient tồn tại trong DB
                var existingPatient = _context.Patients.Find(patientId);
                if (existingPatient == null)
                    throw new Exception($"Patient với ID = {patientId} không tồn tại.");

                // Tạo một Prescription mới
                var prescription = new Prescription
                {
                    DentistId = dentist.DentistId,
                    PatientId = patient.PatientId,
                    Diagnosis = model.Diagnosis,
                    Frequency = model.Frequency,
                    Dosage = model.Dosage,
                    Duration = model.Duration,
                    Notes = model.Notes,
                    DateCreated = DateTime.Now,
                    UserIdCreate = User.Identity?.Name,
                    DateUpdated = DateTime.Now
                };

                // Bắt đầu transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Thêm Prescription
                    await _context.Prescriptions.AddAsync(prescription);
                    await _context.SaveChangesAsync(); // Lưu ngay để lấy ID

                    Console.WriteLine($"Added Prescription - ID: {prescription.PrescriptionId}");

                    // Lấy ID Prescription
                    int prescriptionId = prescription.PrescriptionId;

                    // Thêm PrescriptionDetails
                    foreach (var item in prescription_medicine)
                    {
                        var prescriptionMedicine = new PrescriptionDetails
                        {
                            PrescriptionId = prescriptionId,
                            MedicineId = item.MedicineId,
                            MedicineName = item.MedicineName,
                            MedicinePrice = item.MedicinePrice,
                            Quantity = item.Quantity,
                        };

                        await _context.PrescriptionDetails.AddAsync(prescriptionMedicine);
                    }

                    // Lưu tất cả thay đổi
                    await _context.SaveChangesAsync();

                    // Xóa giỏ hàng sau khi lập đơn thuốc
                    ClearPrescription();

                    // Commit transaction
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction committed successfully.");

                    return Json($"Details/{prescriptionId}");
                }
                catch (Exception ex)
                {
                    // Rollback nếu xảy ra lỗi
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Transaction rolled back. Error: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Init: {ex.ToString()}");
                return BadRequest(ex.Message);
            }
        }

    }
}
