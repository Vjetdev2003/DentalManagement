using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class MedicalRecordRepository : IRepository<MedicalRecord>
    {
        private readonly DentalManagementDbContext _context;

        public MedicalRecordRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(MedicalRecord entity)
        {
            _context.MedicalRecords.Add(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords.ToListAsync();
        }

        public Task<IEnumerable<MedicalRecord>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<MedicalRecord> GetByIdAsync(int id)
        {
            return await _context.MedicalRecords.FindAsync(id);
        }

        public Task<IEnumerable<MedicalRecord>> GetDetailAsyncById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<MedicalRecord>> GetElementById(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public bool InUse(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(MedicalRecord entity)
        {
            _context.MedicalRecords.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
