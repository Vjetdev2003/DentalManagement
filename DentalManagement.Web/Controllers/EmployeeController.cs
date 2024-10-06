using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentalManagement.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        private readonly DentalManagementDbContext _dentalManagementDbContext;
        private readonly IRepository<Employee> _employeeRepository;
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "employees_search";
        public EmployeeController(IRepository<Employee> employeeRepository, DentalManagementDbContext dentalManagementDbContext)
        {
            _employeeRepository = employeeRepository;
            _dentalManagementDbContext = dentalManagementDbContext;
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
            int rowCount = await _dentalManagementDbContext.Employees
       .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.FullName.Contains(input.SearchValue))
       .CountAsync();

            var data = await _dentalManagementDbContext.Employees
    .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.FullName.ToUpper().Contains(input.SearchValue.ToUpper()))
    .Skip((input.Page - 1) * input.PageSize)
    .Take(input.PageSize)
    .ToListAsync();
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Employees = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }


        public IActionResult Create()
        {
            // Khởi tạo một đối tượng Employee mới
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee(
                );
            return View("Edit",model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Employee model)
        {
            if (ModelState.IsValid)
            {
                // Lưu nhân viên vào cơ sở dữ liệu
                await _employeeRepository.AddAsync(model);
                await _dentalManagementDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu model không hợp lệ, trả về view với model hiện tại để hiển thị thông báo lỗi
            return View(model);
        }
        public async Task<IActionResult> Edit(int id, Employee model)
        {
            ViewBag.Title = "Chỉnh sửa thông tin nhân viên";
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        public async Task<IActionResult> Save(IFormFile? uploadPhoto, string birthDateInput, Employee employee)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(employee.FullName))
                ModelState.AddModelError(nameof(employee.FullName), "Họ và tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(employee.Email))
                ModelState.AddModelError(nameof(employee.Email), "Vui lòng nhập email");

            // Xử lý ngày sinh
            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                employee.DateOfBirth = birthDate.Value;

            // Lưu thông tin ngày tạo, ngày cập nhật và người tạo/cập nhật
            if (employee.EmployeeId == 0) // Nếu là nhân viên mới
            {
                employee.DateCreated = DateTime.Now; // Thời gian tạo
                employee.UserIdCreate = User.Identity?.Name; // Người tạo
            }
            employee.DateUpdated = DateTime.Now; // Cập nhật thời gian
            employee.UserIdUpdated = User.Identity?.Name; // Người cập nhật

            // Xử lý upload ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\employees"); // Đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); // Đường dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                employee.Avatar = fileName; // Lưu tên file ảnh vào thuộc tính Avatar
            }

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi trong ModelState, trả lại view với dữ liệu đã nhập
                return View("Edit", employee);
            }

            if (employee.EmployeeId == 0)
            {
                await _employeeRepository.AddAsync(employee);
            }
            else
            {
                await _employeeRepository.UpdateAsync(employee);
            }
            return RedirectToAction("Index");
        }
       
        public async Task<IActionResult> Delete(int id = 0)
        {//Nếu lời gọi là POST Thì ta thực hiện xoá 
            ViewBag.Title = "Xoá thông tin khách hàng";
            if (Request.Method == "POST")
            {
                await _employeeRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            //nếu lời gọi là GET Thì hiển thị khách hàng cần xoá
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !_employeeRepository.InUse(id);
            return View(employee);
        }


    }
}
