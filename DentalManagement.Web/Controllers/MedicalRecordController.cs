using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.Presentation;
using DentalManagement.Web.Interfaces;

namespace DentalManagement.Web.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly DentalManagementDbContext _context;
        private readonly MedicalRecordRepository _repository;
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Dentist> _dentistRepository;
        private const string SEARCH_RECORD = "medicalrecord_search";
        private const int PAGE_SIZE = 10;

        public MedicalRecordController(DentalManagementDbContext context, MedicalRecordRepository repository, 
            IRepository<Service> serviceRepository, IRepository<Patient> patientRepository, IRepository<Dentist> dentistRepository)
        {
            _context = context;
            _repository = repository;
            _serviceRepository = serviceRepository;
            _patientRepository = patientRepository;
            _dentistRepository = dentistRepository;
        }

        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<MedicalRecordInput>(SEARCH_RECORD);
            if (input == null)
            {
                input = new MedicalRecordInput()
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
                var rowCount = await _context.MedicalRecords
                    .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.Patient.PatientName.Contains(input.SearchValue) || e.Dentist.DentistName.Contains(input.SearchValue))
                    .CountAsync();

                var data = await _context.MedicalRecords
                    .Include(e => e.Patient) // Include các navigation properties nếu cần
                    .Include(e => e.Service)
                    .Include(e=>e.Dentist)
                    .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.Patient.PatientName.ToUpper().Contains(input.SearchValue.ToUpper()) || e.Dentist.DentistName.ToUpper().Contains(input.SearchValue.ToUpper()))
                    .Skip((input.Page - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .ToListAsync();

                var model = new MedicalRecordSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue ?? "",
                    RowCount = rowCount,
                    MedicalRecords = data
                };

                ApplicationContext.SetSessionData(SEARCH_RECORD, input);

                return View(model);
            }catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung hồ sơ y tế";
           
           var service = SelectListHelper.GetServices(_serviceRepository);
           var patient = SelectListHelper.GetPatients(_patientRepository);
            var dentist = SelectListHelper.GetDentists(_dentistRepository);
            ViewBag.ServiceList = service;
            ViewBag.DentistList = dentist;
            ViewBag.PatientList = patient;
            var model = new MedicalRecord();
            return View("Edit",model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa thông tin hồ sơ y tế";
            var record = await _repository.GetByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            return View("Edit", record);
        }

        [HttpPost]
        public async Task<IActionResult> Save(MedicalRecord data, IFormFile? uploadPhoto)
        {
            try
            {
                if (string.IsNullOrEmpty(data.DescriptionStatus))
                    ModelState.AddModelError(nameof(data.DescriptionStatus), "Vui lòng nhập mô tả trạng thái");
                if (string.IsNullOrEmpty(data.Treatment))
                    ModelState.AddModelError(nameof(data.Treatment), "Vui lòng nhập phương pháp điều trị");
                if (string.IsNullOrEmpty(data.Symptoms))
                    ModelState.AddModelError(nameof(data.Symptoms), "Vui lòng nhập mô tả triệu chứng");
                if (string.IsNullOrEmpty(data.Diagnosis))
                    ModelState.AddModelError(nameof(data.Diagnosis), "Vui lòng nhập chẩn đoán");


                var userData = User.GetUserData();
                // Xử lý thời gian tạo và cập nhật
                if (data.MedicalRecordId == 0)
                {
                    data.DateCreated = DateTime.Now;
                    data.UserIdCreate = userData.Roles.ToString();
                }
                data.Status = 1;
                data.DateUpdated = DateTime.Now;
                data.UserIdUpdated = User.Identity?.Name;
                data.TreatmentOutcome = " Chưa cập nhật";
                if (data.MedicalRecordId == 0)
                {
                    await _repository.AddAsync(data);
                }
                else
                {
                    await _repository.UpdateAsync(data);

                }

                return RedirectToAction("Index");
            }
            catch (Exception ex) { 
                return Json(ex.Message);
            }
        }
        public async Task<IActionResult>Details(int id = 0)
        {
            var userData = User.GetUserData();
            ViewBag.IsFinish = false;
            ViewBag.IsDelete = false;
            ViewBag.Dentist = false;
            ViewBag.IsUpdate = false;
            ViewBag.IsUnProcess = false;
            // Lấy thông tin hóa đơn từ repository
            var record = await _repository.GetByIdAsync(id);
            var dentist = await _context.Dentists.SingleOrDefaultAsync(d => d.DentistId == record.DentistId);
            var patient = await _context.Patients.SingleOrDefaultAsync(d => d.PatientId == record.PatientId);
            var service = await _context.Services.SingleOrDefaultAsync(d => d.ServiceId == record.ServiceId);

            var medicalRecord = await _context.MedicalRecords
             .Include(m => m.Patient)
             .Include(m => m.Dentist)
             .Include(m => m.Service)
             .FirstOrDefaultAsync(m => m.MedicalRecordId == id);
            if (userData.UserId.Equals(record.DentistId.ToString()))
            {
                ViewBag.IsDentist = true;
            }
            if (medicalRecord == null)
            {
                return RedirectToAction("Index");
            }
            if (record == null)
            {
                return RedirectToAction("Index");
            }
            switch (medicalRecord.Status)
            {
                case Constants_MedicalRecord.UNPROCESS:
                    ViewBag.IsDelete = true;
                    ViewBag.IsUnProcess = true;
                    break;
                case Constants_MedicalRecord.PENDING:
                    ViewBag.IsUpdate = true;
                    break;
                case Constants_MedicalRecord.COMPLETE:
                    ViewBag.IsFinish = true;
                    break;
                case Constants_MedicalRecord.CANCELLED:
                case Constants_MedicalRecord.FAILED:
                    ViewBag.IsFinish = true;
                    ViewBag.IsDelete = true;
                    break;
            }

            // Lấy danh sách chi tiết hóa đơn từ repository

            var model = new MedicalRecordDetailsModel
            {
                MedicalRecord = medicalRecord,
                Dentist = dentist,
                Patient = patient,
                Service = service,
            };
        

            // Truyền thông báo (nếu có) từ TempData
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            // Trả về view với model
            return View(model);
        }
        public async Task<IActionResult> Confirm(int id = 0)
        {
            bool result = await _repository.UnProcess(id);
            if (!result)
                TempData["Message"] = "Không thể xác nhận xử lý hồ sơ này ";
            return RedirectToAction("Details", new { id });
        }
        public async Task<IActionResult> Finish(int id = 0)
        {
            bool result = await _repository.FinishMedicalRecord(id);
            if (!result)
                TempData["Message"] = "Không thể hoàn tất hồ sơ  này ";
            return RedirectToAction("Index", new { id });
        }
        [HttpGet] 
        public  IActionResult Report(int id=0)
        {
            var model = _context.MedicalRecords.FirstOrDefaultAsync(o => o.MedicalRecordId == id);
            return View(model.Result);
        }
        [HttpPost]
        public async Task<IActionResult> SaveReport(int id, string treatmentOutcome, DateTime? nextAppointmentDate)
        {
            var record = _context.MedicalRecords.SingleOrDefaultAsync(r => r.MedicalRecordId == id).Result;
            // Kiểm tra nếu không nhập kết quả điều trị
            if (string.IsNullOrWhiteSpace(treatmentOutcome))
                return Json(new { success = false, message = "Vui lòng nhập kết quả điều trị" });

            // Kiểm tra nếu không chọn ngày hẹn tiếp theo
            if (!nextAppointmentDate.HasValue && nextAppointmentDate < record.DateOfTreatment)
                return Json(new { success = false, message = "Vui lòng chọn ngày hẹn tiếp theo" });
            
            // Lưu thông tin báo cáo vào cơ sở dữ liệu  
            bool result = await _repository.SaveTreatmentReport(id, treatmentOutcome, nextAppointmentDate.Value);
            
            // Kiểm tra nếu không lưu thành công
            if (!result)
                return Json(new { success = false, message = "Không thể lưu báo cáo cho hồ sơ này" });

            // Trả về kết quả thành công và điều hướng tới trang chi tiết
            return Json(new { success = true, redirectUrl = Url.Action("Details", "MedicalRecord", new { id = id }) });
        }

        public async Task<IActionResult> Cancel(int id = 0)
        {
            bool result = await _repository.CancelMedicalRecord(id);
            if (!result)
                TempData["Message"] = "";
            return RedirectToAction("Details", new { id });
        }
        public async Task<IActionResult> Failed(int id = 0)
        {
            bool result = await _repository.Failed(id);
            if (!result)
                TempData["Message"] = "";
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            try
            {
                // Step 1: Get the invoice to be deleted
                var record = await _context.MedicalRecords.FindAsync(id);
                if (record == null)
                {
                    TempData["Message"] = "Hồ sơ không tồn tại.";
                    return RedirectToAction("Index");
                }
                _context.MedicalRecords.Remove(record);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Hồ sơ đã được xoá thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the delete operation
                TempData["Message"] = "Có lỗi xảy ra khi xoá hồ sơ: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }
    }
}
