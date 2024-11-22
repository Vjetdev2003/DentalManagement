using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using System.Drawing.Printing;

namespace DentalManagement.Web.Repository
{
    public class InvoiceDetailsRepository : IInvoiceDetails
    {
        private readonly DentalManagementDbContext _context;

        public InvoiceDetailsRepository(DentalManagementDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceDetails>> GetAllAsync(int page = 1, int pageSize = 10, string searchValue = "")
        {
            var query = _context.InvoiceDetails.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                // Giả sử bạn tìm kiếm theo tên bệnh nhân hoặc các trường liên quan
                query = query.Where(i => i.Service.ServiceName.Contains(searchValue));
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
      
        public async Task<InvoiceDetails> GetByIdAsync(int id)
        {
            return await _context.InvoiceDetails.FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public  IQueryable<InvoiceDetails> GetInvoicesById(int invoiceId)
        {
            return  _context.InvoiceDetails
                  .Where(id => id.InvoiceId == invoiceId)
                  .Include(id => id.Service); // Eager load Service entity        }
        }
            public async Task<List<InvoiceDetails>> GetInvoicesByIdAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                                  .Where(d => d.InvoiceId == invoiceId)
                                  .ToListAsync();
        }
    }
}
