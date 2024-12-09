using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Dentist},{WebUserRoles.Employee}")]
    public class MedicineController : Controller
    {
        private readonly DentalManagementDbContext _dentalManagementDbContext;
        private readonly IRepository<Medicine> _medicineRepository;
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "medicine_search";
        public MedicineController(DentalManagementDbContext dentalManagementDbContext,IRepository<Medicine> medicineRepository)
        {
            _medicineRepository = medicineRepository;
            _dentalManagementDbContext = dentalManagementDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_CONDITION);

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
                int rowCount = await _dentalManagementDbContext.Medicines
           .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.MedicineName.Contains(input.SearchValue))
           .CountAsync();
                var data = await _dentalManagementDbContext.Medicines
                    .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.MedicineName.ToUpper().Contains(input.SearchValue.ToUpper()))
                    .Skip((input.Page - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .ToListAsync();
                var model = new MedicineSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue ?? "",
                    RowCount = rowCount,
                    Medicines = data
                };
                ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
                return View(model);
            }
            catch (Exception ex) { 
            return View(ex.Message);
            }
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung thuốc";
            var medicine = new Medicine();
            return View("Edit", medicine);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Medicine model)
        {
            if (ModelState.IsValid)
            {
                // Lưu nhân viên vào cơ sở dữ liệu
                await _medicineRepository.AddAsync(model);
                await _dentalManagementDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu model không hợp lệ, trả về view với model hiện tại để hiển thị thông báo lỗi
            return View(model);
        }
        public async Task<IActionResult> Edit(int id, Medicine model)
        {
            ViewBag.Title = "Chỉnh sửa thông tin thuốc";
            var medicine = await _medicineRepository.GetByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Medicine data,IFormFile uploadPhoto)
        {
            ViewBag.Title = data.MedicineId == 0 ? "Bổ sung thuốc" : "Cập nhật thông tin thuốc";
            if (string.IsNullOrEmpty(data.MedicineName))
                ModelState.AddModelError(nameof(data.MedicineName), "Tên thuốc không được để trống");
            if (string.IsNullOrEmpty(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (string.IsNullOrEmpty(data.DosageInstructions))
                ModelState.AddModelError(nameof(data.DosageInstructions), "Liều lượng hướng dẫn không được để trống");
            if (data.StockQuantity <= 0)
            {
                ModelState.AddModelError(nameof(data.StockQuantity), "Số lượng không được để trống.");
            }
            if (data.Price <= 0)
            {
                ModelState.AddModelError(nameof(data.Price), "Giá không được để trống và phải lớn hơn 0.");
            }
            data.Usage = "";
            // Lưu thông tin ngày tạo, ngày cập nhật và người tạo/cập nhật
            if (data.MedicineId == 0) // Nếu là nhân viên mới
            {
                data.DateCreated = DateTime.Now; // Thời gian tạo
                data.UserIdCreate = User.Identity?.Name; // Người tạo
            }
            data.DateUpdated = DateTime.Now; // Cập nhật thời gian
            data.UserIdUpdated = User.Identity?.Name; // Người cập nhật

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\medicines"); // Đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); // Đường dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                data.Photo = fileName; // Lưu tên file ảnh vào thuộc tính Avatar
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (data.MedicineId == 0)
            {
                await  _medicineRepository.AddAsync(data);
            }
            else
            {
                await _medicineRepository.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id = 0)
        {//Nếu lời gọi là POST Thì ta thực hiện xoá 
            ViewBag.Title = "Xoá thông tin bệnh nhân";
            if (Request.Method == "POST")
            {
                await _medicineRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            //nếu lời gọi là GET Thì hiển thị khách hàng cần xoá
            var employee = await _medicineRepository.GetByIdAsync(id);
            if (employee == null)
                return RedirectToAction("Index");
            return View(employee);
        }
    }
}
