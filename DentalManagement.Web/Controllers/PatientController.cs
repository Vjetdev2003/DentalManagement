﻿using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Patient},{WebUserRoles.Employee},{WebUserRoles.Dentist},{WebUserRoles.Administrator}")]
    public class PatientController : Controller
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly DentalManagementDbContext _dentalManagementDbContext;
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "patients_search";
        public PatientController(DentalManagementDbContext dentalManagementDbContext, PatientRepository patientRepository)
        {
            _dentalManagementDbContext = dentalManagementDbContext;
            _patientRepository = patientRepository;
        }
        public async Task<IActionResult> Index()
        {
            var a = 0;
            // Kiểm tra nếu cookie có chứa UserRoles
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Login");
            }
            if (userData != null && userData.Roles.Contains(WebUserRoles.Patient))
            {
                return RedirectToAction($"Detail", "Patient", new { id = userData.UserId });
            }
            // Lấy thông tin từ cookie hoặc tạo mới nếu hông có
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>("SEARCH_CONDITION");

            // Nếu input chưa tồn tại thì khởi tạo
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
            int rowCount = await _dentalManagementDbContext.Patients
       .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.PatientName.Contains(input.SearchValue))
       .CountAsync();

            var data = await _dentalManagementDbContext.Patients
                .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.PatientName.ToUpper().Contains(input.SearchValue.ToUpper()))
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();
            var model = new PatientSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Patients = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung bệnh nhân";
            var patient = new Patient();
            return View("Edit", patient);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Patient model)
        {
            if (ModelState.IsValid)
            {
                // Lưu nhân viên vào cơ sở dữ liệu
                await _patientRepository.AddAsync(model);
                await _dentalManagementDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu model không hợp lệ, trả về view với model hiện tại để hiển thị thông báo lỗi
            return View(model);
        }
        public  IActionResult Detail(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var patient =  _dentalManagementDbContext.Patients
                .FirstOrDefault(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        } 
        public async Task<IActionResult> Edit(int id, Patient model)
        {
            ViewBag.Title = "Chỉnh sửa thông tin bênh nhân";
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Patient data, string birthDateInput)
        {
            ViewBag.Title = data.PatientId == 0 ? "Bổ sung bệnh nhân" : "Cập nhật thông tin bệnh nhân";
            if (string.IsNullOrEmpty(data.PatientName))
                ModelState.AddModelError(nameof(data.PatientName), "Tên bệnh nhân không được để trống");
            if (string.IsNullOrEmpty(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
            if (string.IsNullOrEmpty(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");



            // Xử lý ngày sinh
            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                data.PatientDOB = birthDate.Value;

            // Lưu thông tin ngày tạo, ngày cập nhật và người tạo/cập nhật
            if (data.PatientId == 0) // Nếu là nhân viên mới
            {
                data.DateCreated = DateTime.Now; // Thời gian tạo
                data.UserIdCreate = User.Identity?.Name; // Người tạo
            }
            data.DateUpdated = DateTime.Now; // Cập nhật thời gian
            data.UserIdUpdated = User.Identity?.Name; // Người cập nhật
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (data.PatientId == 0)
            {
               await _patientRepository.AddAsync(data);
            }
            else
            {
                await _patientRepository.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id = 0)
        {//Nếu lời gọi là POST Thì ta thực hiện xoá 
            ViewBag.Title = "Xoá thông tin bệnh nhân";
            if (Request.Method == "POST")
            {
                await _patientRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            //nếu lời gọi là GET Thì hiển thị khách hàng cần xoá
            var employee = await _patientRepository.GetByIdAsync(id);
            if (employee == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !_patientRepository.InUse(id);
            return View(employee);
        }
    }
}
