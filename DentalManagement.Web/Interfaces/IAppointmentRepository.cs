using DentalManagement.DomainModels;
using DentalManagement.Web.Models;

namespace DentalManagement.Web.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppointmentsForCalendarAsync();
        Task<Appointment> GetByIdAsync(int id);
        Task AddAsync(Appointment entity);
        Task UpdateAsync(Appointment entity);
        Task<bool> DeleteAsync(int id);
        bool InUse(int id);
        int CountAsync();
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>>GetAppointmentsAsyncById(int id);
        Task<bool> AcceptAppointment(int appointmentID);
        Task<bool> CancelAppointment(int appointmentID);
        Task<bool> NoShowAppointment(int appointmentID);
        Task<bool> FinishAppointment(int appointmentID);
        Task SaveAppointment(AppointmentCreateModel appointmentModel);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
       
       // Task<IEnumerable<Appointment>> GetFinishedAppointmentsByPatientIdAsync(int patientId);
    }
}
