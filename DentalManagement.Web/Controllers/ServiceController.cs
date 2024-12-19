using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Dentist},{WebUserRoles.Employee}")]
    public class ServiceController : Controller
    {
        private readonly DentalManagementDbContext _dentalManagementDbContext;
        private readonly IRepository<Service> _serviceRepository;
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "service_search";
        public ServiceController(DentalManagementDbContext dentalManagementDbContext,IRepository<Service>serviceRepository)
        {
            _dentalManagementDbContext = dentalManagementDbContext;
            _serviceRepository = serviceRepository;
        
        }

        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_CONDITION);
            if (input == null) {
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
            int rowCount = await _dentalManagementDbContext.Services
                .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.ServiceName.Contains(input.SearchValue))
                .CountAsync();

            List<Service> data = new List<Service>(); // Khởi tạo data

            try
            {
                data = await _dentalManagementDbContext.Services
                    .Where(e => string.IsNullOrEmpty(input.SearchValue) || e.ServiceName.ToUpper().Contains(input.SearchValue.ToUpper()))
                    .Skip((input.Page - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching data: {ex.Message}");
                return View("Error");
            }

            var model = new ServiceSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Services = data 
            };

            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);

            return View(model);
        }


         public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung dịch vụ";
            var service = new Service();
            return View("Edit", service);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Service model)
        {
            if (ModelState.IsValid)
            {
               
                await _serviceRepository.AddAsync(model);
                await _dentalManagementDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(int id, Service model)
        {
            ViewBag.Title = "Chỉnh sửa thông tin";
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Service data,IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ServiceId == 0 ? "Bổ sung dịch vụ" : "Cập nhật thông tin dịch vụ";
            if (string.IsNullOrEmpty(data.ServiceName))
                ModelState.AddModelError(nameof(data.ServiceName), "Tên dịch vụ không được để trống");
            if (string.IsNullOrEmpty(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả dịch vụ không được để trống");
            if (data.Price <= 0)
            {
                ModelState.AddModelError(nameof(data.Price), "Giá không được để trống và phải lớn hơn 0.");
            }
            
            if (data.ServiceId == 0) 
            {
                data.DateCreated = DateTime.Now; 
                data.UserIdCreate = User.Identity?.Name;
            }
            data.DateUpdated = DateTime.Now; 
            data.UserIdUpdated = User.Identity?.Name; 
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; 
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\accounts");
                string filePath = Path.Combine(folder, fileName); 

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                data.Photo = fileName; 
            }
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (data.ServiceId == 0)
            {
                await _serviceRepository.AddAsync(data);
            }
            else
            {
                await _serviceRepository.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id = 0)
        {//Nếu lời gọi là POST Thì ta thực hiện xoá 
            ViewBag.Title = "Xoá thông dịch vụ";
            if (Request.Method == "POST")
            {
                await _serviceRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            //nếu lời gọi là GET Thì hiển thị khách hàng cần xoá
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
                return RedirectToAction("Index");
            return View(service);
        }
    }
}
