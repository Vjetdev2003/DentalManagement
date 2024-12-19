
using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DentalManagement.Web.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly DentalManagementDbContext _context;
        public AppointmentRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Appointment entity)
        {
            _context.Appointments.Add(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
            return _context.Appointments.Count();
        }
        public async Task<IEnumerable<AppointmentStatus>> StatusList(AppointmentStatus appointmentStatus)
        {
            return await _context.AppointmentStatuses.ToListAsync();
        }

        public bool InUse(int id)
        {
            var appointment = _context.Appointments
            .FirstOrDefault(a => a.AppointmentId == id);

            // Kiểm tra nếu lịch hẹn đã được xử lý (status khác "Pending")
            return appointment != null && appointment.Status != Constants.APPOINTMENT_FINISHED;
        }

        public async Task UpdateAsync(Appointment entity)
        {
            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForCalendarAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Service)
                .Include(a => a.Dentist)
                .Select(a => new Appointment
                {
                    AppointmentId = a.AppointmentId, // Đảm bảo rằng bạn lấy AppointmentId
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    DentistId = a.DentistId,
                    DentistName = a.DentistName,
                    PatientName = a.Patient.PatientName,
                    ServiceName = a.Service.ServiceName,
                    DateCreated = a.DateCreated,
                    Notes = a.Notes,
                    Status = a.Status,
                })
                .ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(int id)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Dentist)
                .Include(a => a.Service)
                .Where(a => a.AppointmentId == id)
                .ToListAsync();

            if (!appointments.Any())
            {
                // Xử lý trường hợp không tìm thấy appointment
                throw new Exception("Appointment not found.");
            }

            return appointments.FirstOrDefault(); // Trả về appointment đầu tiên
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Dentist)
                .Include(a => a.Service)
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();
        }


        public async Task<IEnumerable<Appointment>> GetAppointmentsAsyncById(int id)
        {
            return await Task.Run(() =>
            {
                return _context.Appointments.Where(a => a.AppointmentId == id);
            });
        }

        public async Task<bool> AcceptAppointment(int appointmentID)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var appointment = await _context.Appointments.FindAsync(appointmentID);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (appointment == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (appointment.Status == Constants.APPOINTMENT_INIT)
            {
                // Cập nhật trạng thái của lịch hẹn
                appointment.Status = Constants.APPOINTMENT_CONFIRMED;
                appointment.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái khởi tạo
            return false;
        }

        public async Task<bool> CancelAppointment(int appointmentID)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var appointment = await _context.Appointments.FindAsync(appointmentID);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (appointment == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (appointment.Status != Constants.APPOINTMENT_FINISHED)
            {
                // Cập nhật trạng thái của lịch hẹn
                appointment.Status = Constants.APPOINTMENT_CANCELLED;
                appointment.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn đã hoàn tất
            return false;
        }

        public async Task<bool> NoShowAppointment(int appointmentID)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var appointment = await _context.Appointments.FindAsync(appointmentID);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (appointment == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (appointment.Status == Constants.APPOINTMENT_INIT)
            {
                // Cập nhật trạng thái của lịch hẹn
                appointment.Status = Constants.APPOINTMENT_NO_SHOW; // Thay thế với trạng thái từ Constants
                appointment.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái khởi tạo
            return false;
        }

        public async Task<bool> FinishAppointment(int appointmentID)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var appointment = await _context.Appointments.FindAsync(appointmentID);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (appointment == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (appointment.Status == Constants.APPOINTMENT_CONFIRMED)
            {
                // Cập nhật trạng thái của lịch hẹn
                appointment.Status = Constants.APPOINTMENT_FINISHED;
                appointment.FinishedTime = DateTime.Now;
                appointment.DateUpdated = DateTime.Now;// Cập nhật thời gian hoàn tất
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái đã xác nhận
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Find the appointment by ID
            var appointment = await _context.Appointments.FindAsync(id);

            // Check if the appointment exists
            if (appointment == null)
            {
                return false; // Appointment not found
            }

            // Check if the appointment is finished, prevent deletion if it is
            if (appointment.Status == Constants.APPOINTMENT_FINISHED)
            {
                throw new InvalidOperationException("Cannot delete a completed appointment.");
            }

            // Remove the appointment from the database
            _context.Appointments.Remove(appointment);

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            return true; // Deletion was successful
        }


        public async Task SaveAppointment(AppointmentCreateModel model)
        {
            var userData = new WebUserData();

            // Lấy thông tin dịch vụ từ cơ sở dữ liệu theo ServiceId
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == model.ServiceId);

            if (service == null)
            {
                // Xử lý nếu không tìm thấy dịch vụ (nếu cần)
                throw new Exception("Dịch vụ không tồn tại.");
            }
            var timeSlotParts = model.TimeSlot.Split('-');
            var appointment = new Appointment
            {
                PatientId = Int32.Parse(userData.UserId),
                PatientName = userData.DisplayName,
                ServiceID = model.ServiceId,
                DentistId = Int32.Parse(userData.UserId),
                DentistName = model.PatientName,
                ServiceName = service.ServiceName, // Gán tên dịch vụ từ cơ sở dữ liệu
                StartTime = TimeSpan.FromHours(int.Parse(timeSlotParts[0])),
                EndTime = TimeSpan.FromHours(int.Parse(timeSlotParts[1])),
                Phone = model.Phone,
                Email = model.Email,
                AppointmentDate = model.AppointmentDate,
                Notes = model.Notes,
                Status = Constants.APPOINTMENT_INIT,
                DateCreated = DateTime.Now,
                UserIdCreate = "currentUserId" // Gán ID người dùng hiện tại
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
               .Where(a => a.PatientId == patientId) // Lọc theo ID bệnh nhân
               .Include(a => a.Service)
               .Include(a => a.Dentist)
               .OrderByDescending(a => a.DateCreated) // Sắp xếp theo ngày tạo giảm dần
               .ToListAsync();
        }
    }
}
