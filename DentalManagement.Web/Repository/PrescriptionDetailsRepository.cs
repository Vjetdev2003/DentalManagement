using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class PrescriptionDetailsRepository
    {
        private readonly DentalManagementDbContext _context;

        public PrescriptionDetailsRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PrescriptionDetails >> GetAllAsync()
        {
            return await _context.PrescriptionDetails.ToListAsync();
        }
        public async Task<List<PrescriptionDetails>> GetByPrescriptionIdAsync(int prescriptionId)
        {
            return await _context.PrescriptionDetails
                                                   .Where(pd => pd.PrescriptionId == prescriptionId)
                                                   .ToListAsync();
        }

        public async Task<PrescriptionDetails> GetByIdAsync(int id)
        {
            return await _context.PrescriptionDetails.FirstOrDefaultAsync(p=>p.PrescriptionId == id);
        }
        public async Task AddAsync(PrescriptionDetails prescription)
        {
            _context.PrescriptionDetails.Add(prescription);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(PrescriptionDetails prescription)
        {
            _context.PrescriptionDetails.Update(prescription);
            await _context.SaveChangesAsync();
        }
    }
}
