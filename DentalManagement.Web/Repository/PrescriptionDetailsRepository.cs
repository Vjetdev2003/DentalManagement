using DentalManagement.DomainModels;
using DentalManagement.Web.Data;

namespace DentalManagement.Web.Repository
{
    public class PrescriptionDetailsRepository
    {
        private readonly DentalManagementDbContext _context;

        public PrescriptionDetailsRepository(DentalManagementDbContext context)
        {
            _context = context;
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
