using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class ServiceRepository : IRepository<Service>
    {
        private readonly DentalManagementDbContext _context;

        public ServiceRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Service entity)
        {
            _context.Services.Add(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
           return _context.Services.Count();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetAllAsync(int page = 1, int pageSize = 10, string searchValue = "")
        {
           
                var query = _context.Services.AsQueryable();

                // Nếu có giá trị tìm kiếm, lọc danh sách theo tên dịch vụ
                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(e => e.ServiceName.Contains(searchValue));
                }

                // Thực hiện phân trang
                var services = await query
                    .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi trước đó
                    .Take(pageSize)              // Lấy số bản ghi theo kích thước trang
                    .ToListAsync();              // Chuyển đổi kết quả thành danh sách

                return services;
                  }



        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<IEnumerable<Service>> GetDetailAsyncById(int id)
        {
            return await Task.Run(() =>
            {
                return _context.Services.Where(a => a.ServiceId == id);
            });
        }

        public async Task<List<Service>> GetElementById(List<int> ids)
        {
            return await _context.Services
                .Where(s => ids.Contains(s.ServiceId)) 
                .ToListAsync(); 
        }
        public bool  InUse(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Service entity)
        {
            _context.Services.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
