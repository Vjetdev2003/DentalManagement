using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class InvoiceRepository : IRepository<Invoice>
    {
        private readonly DentalManagementDbContext _context;

        public InvoiceRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Invoice entity)
        {
            _context.Invoices.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public Task<IEnumerable<Invoice>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public Task<Patient?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Invoice> GetByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public bool InUse(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Invoice entity)
        {
            _context.Invoices.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
