using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace DentalManagement.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Employee},{WebUserRoles.Dentist},{WebUserRoles.Administrator}")]
    public class DentistController : Controller
    {

        private readonly DentalManagementDbContext _dentalManagementDbContext;
        private readonly IRepository<Dentist> _dentist;
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "dentist_search";
        public DentistController(IRepository<Dentist> dentist, DentalManagementDbContext dentalManagementDbContext)
        {
            _dentist = dentist;
            _dentalManagementDbContext=dentalManagementDbContext;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin từ session hoặc tạo mới nếu không có
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
                int rowCount = await _dentalManagementDbContext.Dentists
           .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.DentistName.Contains(input.SearchValue))
           .CountAsync();

                var data = await _dentalManagementDbContext.Dentists
        .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.DentistName.ToUpper().Contains(input.SearchValue.ToUpper()))
        .Skip((input.Page - 1) * input.PageSize)
        .Take(input.PageSize)
        .ToListAsync();
                var model = new DentistSearchResult()
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    SearchValue = input.SearchValue ?? "",
                    RowCount = rowCount,
                    Dentists = data
                };
                ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
                return View(model);
            }
            catch (Exception ex) { 
                return Json(ex);
            }
        }
        public IActionResult Create()
        {
            // Khởi tạo một đối tượng Dentist mới
            ViewBag.Title = "Bổ sung nha sĩ";
            var model = new Dentist();
            return View("Edit",model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Dentist model)
        {
            if (ModelState.IsValid)
            {
                // Lưu dentist vào cơ sở dữ liệu
                await _dentist.AddAsync(model);
                await _dentalManagementDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu model không hợp lệ, trả về view với model hiện tại để hiển thị thông báo lỗi
            return View(model);
        }
        public async Task<IActionResult> Edit(int id, Employee model)
        {
            ViewBag.Title = "Chỉnh sửa thông tin nha sĩ";
            var dentist = await _dentist.GetByIdAsync(id);
            if (dentist == null)
            {
                return NotFound();
            }
            return View(dentist);
        }

        public async Task<IActionResult> Save(IFormFile? uploadPhoto, Dentist dentist)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(dentist.DentistName))
                ModelState.AddModelError(nameof(dentist.DentistName), "Họ và tên nha sĩ không được để trống");
            if (string.IsNullOrWhiteSpace(dentist.Email))
                ModelState.AddModelError(nameof(dentist.Email), "Vui lòng nhập email");

            

            // Lưu thông tin ngày tạo, ngày cập nhật và người tạo/cập nhật
            if (dentist.DentistId == 0) // Nếu là nhân viên mới
            {
                dentist.DateCreated = DateTime.Now; // Thời gian tạo
                dentist.UserIdCreate = User.Identity?.Name; // Người tạo
            }
            dentist.DateUpdated = DateTime.Now; // Cập nhật thời gian
            dentist.UserIdUpdated = User.Identity?.Name; // Người cập nhật

            // Xử lý upload ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\accounts"); // Đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); // Đường dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                dentist.Avatar = fileName; // Lưu tên file ảnh vào thuộc tính Avatar
            }

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi trong ModelState, trả lại view với dữ liệu đã nhập
                return View("Edit", dentist);
            }

            if (dentist.DentistId == 0)
            {
                await _dentist.AddAsync(dentist);
            }
            else
            {
                await _dentist.UpdateAsync(dentist);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id = 0)
        {//Nếu lời gọi là POST Thì ta thực hiện xoá 
            ViewBag.Title = "Xoá thông tin nha sĩ";
            if (Request.Method == "POST")
            {
                await _dentist.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            //nếu lời gọi là GET Thì hiển thị khách hàng cần xoá
            var dentist = await _dentist.GetByIdAsync(id);
            if (dentist == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !_dentist.InUse(id);
            return View(dentist);
        }
    }
}
