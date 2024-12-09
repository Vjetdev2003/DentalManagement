using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class PrescriptionRepository
    {
        private readonly DentalManagementDbContext _context;

        public PrescriptionRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public int Count()
        {
          return  _context.Prescriptions.Count();
        }
        public async Task AddAsync(Prescription precription)
        {
            _context.Prescriptions.Add(precription);
           await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(Prescription prescription)
        {
             _context.Prescriptions.Update(prescription);
             await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
           var pre = await _context.Prescriptions.FindAsync(id);
            if (pre != null)
            {
                _context.Prescriptions.Remove(pre);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _context.Prescriptions.FindAsync(id);
        }
        public async Task<IEnumerable<Prescription>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {

            var query = _context.Prescriptions.AsQueryable();


            // Thực hiện phân trang
            var prescriptions = await query
            .Skip((page - 1) * pagesize) // Bỏ qua các bản ghi trước đó
            .Take(pagesize)               // Lấy số bản ghi theo kích thước trang
                .ToListAsync();               // Chuyển đổi kết quả thành danh sách

            return prescriptions;
        }
    }
}
