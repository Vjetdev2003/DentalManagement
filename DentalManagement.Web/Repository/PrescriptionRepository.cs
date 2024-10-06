using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class PrescriptionRepository : IRepository<Prescription>
    {
        private readonly DentalManagementDbContext _context;

        public PrescriptionRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Prescription entity)
        {
            _context.Prescriptions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                _context.Prescriptions.Remove(prescription);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await _context.Prescriptions.ToListAsync();
        }

        public Task<IEnumerable<Prescription>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _context.Prescriptions.FindAsync(id);
        }

        public bool InUse(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Prescription entity)
        {
            _context.Prescriptions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
