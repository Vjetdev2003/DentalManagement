using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class AppointmentRepository : IRepository<Appointment>
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

        public async Task DeleteAsync(int id)
        {
            var appoitment = await _context.Appointments.FindAsync(id);
            if (appoitment != null)
            {
                _context.Appointments.Remove(appoitment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        public Task<IEnumerable<Appointment>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Appointment> GetByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        public bool InUse(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Appointment entity)
        {
            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
