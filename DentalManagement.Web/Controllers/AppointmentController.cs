using DentalManagement.DomainModels;
using DentalManagement.Web;
using DentalManagement.Web.Data;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DentalManagement.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


[Authorize]

public class AppointmentController : Controller
{
    private readonly AppointmentRepository _appointmentRepository;
    private readonly IRepository<Dentist> _dentistRepository;
    private readonly IRepository<Service> _serviceRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly AppointmentStatus _status;

    private readonly DentalManagementDbContext _dentalManagementDbContext;
    private const int PAGE_SIZE = 20;
    private const string SEARCH_CONDITION = "appointment_search";

    public AppointmentController(AppointmentRepository appointmentRepository, AppointmentStatus status, DentalManagementDbContext dentalManagementDbContext, IRepository<Dentist> dentistRepository, 
        IRepository<Service> serviceRepository, IRepository<Patient> patientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _dentalManagementDbContext = dentalManagementDbContext;
        _dentistRepository = dentistRepository;
        _serviceRepository = serviceRepository;
        _patientRepository = patientRepository;
        _status= status;
    }
    // Action để hiển thị danh sách đặt lịch hẹn
    [HttpGet]
    public async Task<IActionResult> Calendar()
    {
        var appointments = await _appointmentRepository.GetAppointmentsForCalendarAsync();

        if (appointments == null || !appointments.Any())
        {
            return Json(new List<CalendarEvent>()); // Trả về danh sách trống nếu không có lịch hẹn
        }

        var events = appointments.Select(a => new CalendarEvent
        {
            Title = string.IsNullOrEmpty(a.PatientName) ? "Chưa rõ tên" : $"{a.PatientName} - {a.ServiceName}",
            Start = a.AppointmentDate ?? DateTime.Now,
            Description = string.IsNullOrEmpty(a.Notes) ? "Không có mô tả" : $"{a.Notes}",
            AppointmentId = a.AppointmentId ,
            Status = a.Status ,
            
            // Đảm bảo bạn lưu ID sự kiện
            
        }).ToList();

        return View(events);
    }
    public async Task<IActionResult> Index()
    {
        var input = ApplicationContext.GetSessionData<AppointmentSearchInput>(SEARCH_CONDITION);

        // Cập nhật các giá trị cho input
        if (input == null)
        {
            input = new AppointmentSearchInput()
            {
                Page = 1,
                PageSize = PAGE_SIZE,
                SearchValue = ""
            };
        }

        return View(input);
    }
    public async Task<IActionResult> Search(AppointmentSearchInput input)
    {
        try
        {
            var data = await _dentalManagementDbContext.Appointments.ToListAsync();
            if(input.Status != 0)
            {
                data = data.Where(a => a.Status ==input.Status).ToList();
            }
            if (!string.IsNullOrEmpty(input.DateRange))
            {
                data = data.Where(a=>a.AppointmentDate <= input.ToTime && a.AppointmentDate>= input.FromTime).ToList();
            }
            if (!string.IsNullOrEmpty(input.SearchValue)) { 
            
                data = data.Where(a=>a.PatientName.ToUpper().Contains(input.SearchValue) && a.DentistName.ToUpper().Contains(input.SearchValue)).ToList();
            }
            int rowCount = data.Count();
            data = data.Skip((input.Page - 1) * input.PageSize)
                 .Take(input.PageSize).ToList();


            var model = new AppointmentSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Appointments = data
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
    //public IActionResult CreateApp()
    //{
    //    // Dữ liệu từ backend
    //    var appointmentData = new
    //    {
    //        name = "Nguyen Van A",
    //        phone = "0123456789",
    //        email = "test@example.com",
    //        date = "2024-11-20",
    //        time = "10:00",
    //        service = "Dental Cleaning",
    //        notes = "Please arrive 10 minutes early."
    //    };

    //    // Truyền dữ liệu JSON vào ViewData
    //    ViewData["AppointmentData"] = System.Text.Json.JsonSerializer.Serialize(appointmentData);
    //    return View(); // Trả về Razor View
    //}

    //[HttpPost]
    //public IActionResult CreateApp(AppointmentViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(model);
    //    }

    //    // Xử lý logic lưu lịch hẹn
    //    TempData["SuccessMessage"] = "Appointment created successfully!";
    //    return RedirectToAction("Create");
   // }

    public IActionResult Create()
	{   

        ViewBag.Title = "Tạo lịch hẹn mới";
        var model = new AppointmentViewModel
        {
            PatientList = SelectListHelper.GetPatients(_patientRepository),
            DentistList = SelectListHelper.GetDentists(_dentistRepository),
            ServiceList = SelectListHelper.GetServices(_serviceRepository),
            Status = Constants.APPOINTMENT_INIT
        };
        return View("Edit", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Appointment appointment)
    {
      
        if (ModelState.IsValid)
        {
            // Thiết lập các thuộc tính cần thiết
            appointment.DateCreated = DateTime.Now;
            appointment.UserIdCreate = User?.Identity?.Name; // Thiết lập người tạo cuộc hẹn

            await _appointmentRepository.AddAsync(appointment); // Thêm cuộc hẹn mới vào repository
            TempData["Message"] = "Cuộc hẹn đã được đặt thành công!";
            return RedirectToAction("Index");
        }

        return View(appointment); // Nếu model state không hợp lệ, trả về dữ liệu hiện tại
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var appointment = await _appointmentRepository.GetByIdAsync(id);

        if (appointment == null)
        {
            return NotFound();
        }

        // Chuyển đổi đối tượng Appointment sang AppointmentViewModel
        var model = new AppointmentViewModel
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            PatientName = appointment.Patient?.PatientName,
            DentistId = appointment.DentistId,
            DentistName = appointment.Dentist?.DentistName,
            ServiceId = appointment.ServiceID,
            ServiceName = appointment.Service?.ServiceName,
            AppointmentDate = appointment.AppointmentDate ?? DateTime.Now,
            Status = appointment.Status,
            Notes = appointment.Notes,
            StatusDescription = appointment.StatusDescription,
            PatientList = new SelectList(await _patientRepository.GetAllAsync(), "PatientId", "PatientName"),
            DentistList = new SelectList(await _dentistRepository.GetAllAsync(), "DentistId", "DentistName"),
            ServiceList = new SelectList(await _serviceRepository.GetAllAsync(), "ServiceId", "ServiceName"),
        };

        // Trả về view với đối tượng AppointmentViewModel
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Save(AppointmentViewModel model)
    {
        if (model.AppointmentDate == null)
        {
            ModelState.AddModelError(string.Empty, "Ngày hẹn không được để trống.");
            return View(model); // Trả về view với thông báo lỗi
        }
        if (string.IsNullOrWhiteSpace(model.Notes))
            ModelState.AddModelError(nameof(model.Notes), "Vui lòng nhập mô tả");
        // Kiểm tra model có hợp lệ không
        if (!ModelState.IsValid)
        {
            // Trả về form với model hiện tại nếu dữ liệu không hợp lệ
            model.PatientList = SelectListHelper.GetPatients(_patientRepository);
            model.DentistList = SelectListHelper.GetDentists(_dentistRepository);
            model.ServiceList = SelectListHelper.GetServices(_serviceRepository);
            return View("Edit", model);
        }

        // Nếu AppointmentId = 0 nghĩa là tạo mới, ngược lại là cập nhật
        Appointment appointment = model.AppointmentId == 0
            ? new Appointment()
            : await _appointmentRepository.GetByIdAsync(model.AppointmentId);

        if (appointment == null)
        {
            return NotFound();
        }
        if(model.AppointmentId == 0)
        {
            appointment.PatientId = model.PatientId;
            appointment.DentistId = model.DentistId;
            appointment.ServiceID = model.ServiceId;
            appointment.AppointmentDate = model.AppointmentDate;
            appointment.Status = Constants.APPOINTMENT_INIT;
            appointment.Notes = model.Notes;
            appointment.Phone = model.Phone;
            appointment.Email = model.Email;
            appointment.DateUpdated = DateTime.Now;
            appointment.UserIdUpdated = User?.Identity?.Name;
            var selectedPatient = await _patientRepository.GetByIdAsync(model.PatientId);
            appointment.PatientName = selectedPatient?.PatientName; // Lưu tên bệnh nhân

            var selectedDentist = await _dentistRepository.GetByIdAsync(model.DentistId);
            appointment.DentistName = selectedDentist?.DentistName; // Lưu tên nha sĩ

            var selectedService = await _serviceRepository.GetByIdAsync(model.ServiceId);
            appointment.ServiceName = selectedService?.ServiceName;
        }
        else
        {
            appointment.PatientId = model.PatientId;
            appointment.DentistId = model.DentistId;
            appointment.ServiceID = model.ServiceId;
            appointment.AppointmentDate = model.AppointmentDate;
            appointment.Status = model.Status;
            appointment.Notes = model.Notes;
            appointment.DateUpdated = DateTime.Now;
            appointment.UserIdUpdated = User?.Identity?.Name;
            var selectedPatient = await _patientRepository.GetByIdAsync(model.PatientId);
            appointment.PatientName = selectedPatient?.PatientName; // Lưu tên bệnh nhân

            var selectedDentist = await _dentistRepository.GetByIdAsync(model.DentistId);
            appointment.DentistName = selectedDentist?.DentistName; // Lưu tên nha sĩ

            var selectedService = await _serviceRepository.GetByIdAsync(model.ServiceId);
            appointment.ServiceName = selectedService?.ServiceName;
        }

        // Ánh xạ dữ liệu từ AppointmentViewModel về Appointment
       
        // Lấy tên người dùng hiện tại để cập nhật

        if (model.AppointmentId == 0)
        {
            // Nếu là tạo mới thì thêm ngày tạo và người tạo
            appointment.DateCreated = DateTime.Now;
            appointment.UserIdCreate = User?.Identity?.Name; // Gán user tạo
            await _appointmentRepository.AddAsync(appointment);
        }
        else
        {
            await _appointmentRepository.UpdateAsync(appointment);
        }

        await _dentalManagementDbContext.SaveChangesAsync();

        // Chuyển hướng về trang Index sau khi lưu thành công
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Details(int id = 0)
    {
        ViewBag.IsFinish = false;
        ViewBag.IsDelete = false;
        ViewBag.IsEditDetails = false;
        ViewBag.IsDentist = false;

        var userData = User.GetUserData();
        
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        var services = appointment.ServiceID;
        var appointmentService = await _serviceRepository.GetByIdAsync(appointment.ServiceID);
        var dentist = appointment.DentistId;
        var dentists = await _dentistRepository.GetByIdAsync(dentist);
        if (appointment == null)
        {
            return RedirectToAction("Index");
        }
      
        if (userData.UserId.Equals(appointment.DentistId.ToString()))
        {
            ViewBag.IsDentist = true;
        }

        switch (appointment.Status)
        {
            case Constants.APPOINTMENT_INIT:
                ViewBag.IsDelete = true;
                ViewBag.IsEditDetails = true;
                break;
            case Constants.APPOINTMENT_CONFIRMED:
                ViewBag.IsEditDetails = true;
                break;
            case Constants.APPOINTMENT_FINISHED:
                ViewBag.IsFinish = true;
                break;
            case Constants.APPOINTMENT_CANCELLED:
            case Constants.APPOINTMENT_NO_SHOW:
                ViewBag.IsDelete = true;
                ViewBag.IsFinish = true;
                break;
        }

        // Fetch appointment details
        var details = await _appointmentRepository.GetAppointmentsAsyncById(id);

        // Map details to AppointmentViewModel if necessary
        var appointmentDetailsViewModel = details.Select(detail => new AppointmentViewModel
        {
            AppointmentId = detail.AppointmentId,
            PatientName = detail.PatientName,
            DentistName = detail.DentistName,
            ServiceName = detail.ServiceName,
            AppointmentDate = detail.AppointmentDate ?? DateTime.Now,
            Status = detail.Status
        }).ToList();

        var model = new AppointmentDetailModel()
        {
            Dentists = dentists,
            Services = appointmentService,
            Appointments = appointment,
            Details = appointmentDetailsViewModel // Use mapped view models
        };

        if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
        return View(model);
    }
    public async Task<IActionResult> Accept(int id = 0)
    {
        bool result = await _appointmentRepository.AcceptAppointment(id);
        if (!result)

            TempData["Message"] = "Không thể duyệt lịch hẹn này ";
        return RedirectToAction("Details", new { id });
    }
    public async Task<IActionResult> Finish(int id = 0)
    {
        bool result = await _appointmentRepository.FinishAppointment(id);
        if (!result)
            TempData["Message"] = "Không thể ghi nhận trạng thái hoàn tất của lịch hẹn";
        return RedirectToAction("Index", new { id });
    }
    public async Task<IActionResult> Cancel(int id = 0)
    {
        bool result = await _appointmentRepository.CancelAppointment(id);
        if (!result)
            TempData["Message"] = "Không thể thực hiện thao tác huỷ của lịch hẹn";
        return RedirectToAction("Details", new { id });
    }
    public async Task<IActionResult> NoShow(int id = 0)
    {
        bool result = await _appointmentRepository.NoShowAppointment(id);
        if (!result)
            TempData["Message"] = "Không thể thực hiện thao tác từ chối của lịch hẹn";
        return RedirectToAction("Details", new { id });
    }
    public async Task<IActionResult> Delete(int id )
    {
        bool result = await _appointmentRepository.DeleteAsync(id);
        if (!result)
        {
            TempData["Message"] = "Không thể xoá đơn hàng này ";
            return RedirectToAction("Details", new { id });

        }
        return RedirectToAction("Index");
    }
    
    
}
