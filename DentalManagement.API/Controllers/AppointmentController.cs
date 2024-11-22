using DentalManagement.API.Data;
using DentalManagement.API.DTOs;
using DentalManagement.API.Models;
using System.Web;
using Microsoft.AspNetCore.Mvc;
namespace DentalManagement.API.Controllers
{
    public class AppointmentController
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AppointmentsController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<AppointmentsController> _logger;

            public AppointmentsController(ApplicationDbContext context, ILogger<AppointmentsController> logger)
            {
                _context = context;
                _logger = logger;
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var appointment = new Appointment
                    {
                        Name = dto.Name,
                        Phone = dto.Phone,
                        Email = dto.Email,
                        AppointmentDate = DateTime.Parse(dto.Date),
                        AppointmentTime = TimeSpan.Parse(dto.Time),
                        Service = dto.Service,
                        Notes = dto.Notes,
                        Status = AppointmentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Đặt lịch hẹn thành công",
                        appointmentId = appointment.Id
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo lịch hẹn");
                    return StatusCode(500, new { message = "Có lỗi xảy ra khi đặt lịch hẹn" });
                }
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                try
                {
                    var appointments =  _context.Appointments
                        .OrderByDescending(a => a.AppointmentDate)
                        .ThenBy(a => a.AppointmentTime)
                        .ToList();

                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lấy danh sách lịch hẹn");
                    return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách lịch hẹn" });
                }
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                try
                {
                    var appointment = await _context.Appointments.FindAsync(id);

                    if (appointment == null)
                    {
                        return NotFound(new { message = "Không tìm thấy lịch hẹn" });
                    }

                    return Ok(appointment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lấy thông tin lịch hẹn");
                    return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thông tin lịch hẹn" });
                }
            }
        }
    }
}
