using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.History;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using DocumentFormat.OpenXml.InkML;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using iTextSharp.text;

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
        public PrescriptionController(DentalManagementDbContext context, IRepository<Medicine> medicineRepo,
            IRepository<Dentist> dentistRepo, IRepository<Patient> patientRepo, PrescriptionRepository prescriptionRepo, MedicalRecordRepository medicalRecordRepo)
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
                    .Include(p => p.Patient)
                    .Include(p => p.Dentist)
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

            if (data.SalePrice <= 0 || data.Quantity <= 0)
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

                ViewBag.IsFinish = false;
                ViewBag.IsDelete = false;


                // Lấy dữ liệu người dùng hiện tại
                var userData = User.GetUserData();


                var pres = await _prescriptionRepo.GetByIdAsync(id);
                var dentist = await _context.Dentists.SingleOrDefaultAsync(d => d.DentistId == pres.DentistId);
                var patient = await _context.Patients.SingleOrDefaultAsync(d => d.PatientId == pres.PatientId);
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



                var presDetailViewModel = presDetails.PrescriptionDetails?.Select(d => new PrescriptionCreateModel
                {
                    MedicineName = d.MedicineName,
                    Quantity = d.Quantity,
                    SalePrice = d.SalePrice,
                    TotalPrice = d.TotalMedicine,

                }).ToList();

                var model = new PrescriptionDetailModel
                {
                    Prescriptions = presDetails,
                    Details = presDetailViewModel,
                    Dentists = dentist,
                    Patients = patient
                };


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
        [Authorize(Roles = WebUserRoles.Dentist)]
        public async Task<IActionResult> Init(int patientId, IEnumerable<PrescriptionMedicineViewModel> details, PrescriptionViewModel model)
        {
            try
            {
                
                Console.WriteLine("Begin Init - Start");
                Console.WriteLine($"PatientId: {patientId}");

                
                var prescription_medicine = GetMedicine();
                Console.WriteLine($"Treatment Voucher Count: {prescription_medicine.Count}, Contents: {string.Join(", ", prescription_medicine.Select(t => t.MedicineId))}");
                if (prescription_medicine.Count == 0)
                    return BadRequest("Chưa có danh sách thuốc, không thể lập đơn thuốc.");

                if (patientId <= 0)
                    return BadRequest("Vui lòng nhập đầy đủ thông tin.");

                
                int dentistId = Convert.ToInt32(User.GetUserData()?.UserId);
                var dentist = await _dentistRepo.GetByIdAsync(dentistId);
                var patient = await _patientRepo.GetByIdAsync(patientId);

                if (dentist == null || patient == null)
                    return NotFound("Nhân viên hoặc bệnh nhân không tồn tại.");

                // Xác nhận Patient tồn tại trong DB
                var existingPatient = _context.Patients.Find(patientId);
                if (existingPatient == null)
                    throw new Exception($"Patient với ID = {patientId} không tồn tại.");

               
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
                    
                    await _context.Prescriptions.AddAsync(prescription);
                    await _context.SaveChangesAsync(); 

                    Console.WriteLine($"Added Prescription - ID: {prescription.PrescriptionId}");

                    
                    int prescriptionId = prescription.PrescriptionId;

                   
                    foreach (var item in prescription_medicine)
                    {
                        var prescriptionMedicine = new PrescriptionDetails
                        {
                            PrescriptionId = prescriptionId,
                            MedicineId = item.MedicineId,
                            MedicineName = item.MedicineName,
                            SalePrice = item.SalePrice,
                            Quantity = item.Quantity,
                            
                        };

                        await _context.PrescriptionDetails.AddAsync(prescriptionMedicine);
                    }

                   
                    await _context.SaveChangesAsync();

                    
                    ClearPrescription();

                   
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction committed successfully.");

                    return Json($"Details/{prescriptionId}");
                }
                catch (Exception ex)
                {
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
        public async Task<IActionResult> Delete(int id = 0)
        {
            try
            {
              
                var prescription = await _context.Prescriptions.SingleOrDefaultAsync(p=>p.PrescriptionId == id);
                if (prescription == null)
                {
                    TempData["Message"] = "đơn thuốc không tồn tại.";
                    return RedirectToAction("Index");
                }
                await _prescriptionRepo.DeleteAsync(id);
                await _context.SaveChangesAsync();

                TempData["Message"] = "đơn thuốc đã được xoá thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
               
                TempData["Message"] = "Có lỗi xảy ra khi xoá đơn thuốc: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }
        [Authorize(Roles = WebUserRoles.Patient)]
        public async Task<IActionResult> PrescriptionHistory(int patientId)
        {
            var userData = User.GetUserData();

            patientId = Int32.Parse(userData.UserId);
            var history = await _prescriptionRepo.GetPrescriptionByPatientIdAsync(patientId);
            var model = new PrescriptionHistoryViewModel
            {
                Prescriptions  = history.ToList(),
            };
            return View(model);
        }
        public async Task<IActionResult> PrintPrescription(int id)
        {
            try
            {
                // Lấy dữ liệu đơn thuốc với thông tin chi tiết thuốc
                var presDetails = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Dentist)
                    .Include(p => p.PrescriptionDetails)
                        .ThenInclude(pd => pd.Medicine)
                    .FirstOrDefaultAsync(p => p.PrescriptionId == id);

                if (presDetails == null)
                {
                    return NotFound("Đơn thuốc không tồn tại.");
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

                    // Thêm tiêu đề và thông tin đơn thuốc
                    document.Add(new Paragraph("ĐƠN THUỐC", font) );
                    document.Add(new Paragraph("=================================", font));
                    document.Add(new Paragraph($"Mã Đơn Thuốc: {presDetails.PrescriptionId}", font));
                    document.Add(new Paragraph($"Ngày Lập: {presDetails.DateCreated:dd/MM/yyyy}", font));
                    document.Add(new Paragraph($"Bác Sĩ: {presDetails.Dentist.DentistName}", font));
                    document.Add(new Paragraph($"Bệnh Nhân: {presDetails.Patient.PatientName}", font));
                    document.Add(new Paragraph(""));

                
                    // Thêm thông tin bổ sung
                    document.Add(new Paragraph($"Chẩn Đoán: {presDetails.Diagnosis}", font));
                    document.Add(new Paragraph($"Liều Lượng: {presDetails.Dosage}", font));
                    document.Add(new Paragraph($"Tần Suất: {presDetails.Frequency}", font));
                    document.Add(new Paragraph($"Thời Gian: {presDetails.Duration} ngày", font));
                    document.Add(new Paragraph($"Ghi Chú: {presDetails.Notes}", font));

                    document.Add(new Paragraph("\n"));


                    // Thêm bảng thông tin thuốc
                    var table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 3, 3, 2, 2 });

                    table.AddCell(new PdfPCell(new Phrase("Tên Thuốc", new Font(baseFont, 12, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Số Lượng", new Font(baseFont, 12, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Đơn Giá", new Font(baseFont, 12, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("Thành Tiền", new Font(baseFont, 12, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER });

                    // Duyệt qua danh sách thuốc và thêm vào bảng
                    foreach (var detail in presDetails.PrescriptionDetails)
                    {
                        table.AddCell(new PdfPCell(new Phrase(detail.Medicine.MedicineName, font)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        table.AddCell(new PdfPCell(new Phrase(detail.Quantity.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(detail.SalePrice.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(detail.TotalMedicine.ToString("C"), font)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    document.Add(table);
                    document.Add(new Paragraph(""));

                    // Đóng tài liệu
                    document.Close();

                    // Trả về file PDF
                    var fileBytes = ms.ToArray();
                    return File(fileBytes, "application/pdf", $"DonThuoc_{presDetails.PrescriptionId}_{presDetails.Patient.PatientName}.pdf");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine(ex.Message);
                return BadRequest("Có lỗi xảy ra khi in đơn thuốc.");
            }
        }



    }
}
