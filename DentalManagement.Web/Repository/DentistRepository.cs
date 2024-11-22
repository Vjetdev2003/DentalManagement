using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class DentistRepository : IRepository<Dentist>
    {
        private readonly DentalManagementDbContext _context;

        public DentistRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Dentist entity)
        {
            _context.Dentists.Add(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            var dentist = await _context.Dentists.FindAsync(id);
            if (dentist != null) { 
                _context.Dentists.Remove(dentist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Dentist>> GetAllAsync()
        {
            return await _context.Dentists.ToListAsync();
        }

        public async Task<IEnumerable<Dentist>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {
            var query = _context.Dentists.AsQueryable();

            // Nếu có giá trị tìm kiếm, lọc danh sách theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(e => e.DentistName.Contains(searchValue) || e.Email.Contains(searchValue));
            }

            // Thực hiện phân trang
            var dentists = await query
            .Skip((page - 1) * pagesize) // Bỏ qua các bản ghi trước đó
            .Take(pagesize)               // Lấy số bản ghi theo kích thước trang
                .ToListAsync();               // Chuyển đổi kết quả thành danh sách

            return dentists;
        }

       

        public async Task<Dentist> GetByIdAsync(int id)
        {
            return await _context.Dentists.FindAsync(id);
        }

        public async Task<IEnumerable<Dentist>> GetDetailAsyncById(int id)
        {
            return await Task.Run(() =>
            {
                return _context.Dentists.Where(a => a.DentistId == id);
            });
        }

        public Task<List<Dentist>> GetElementById(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public bool InUse(int id)
        {
            bool inUseDentist= _context.Set<Dentist>().Any(o => o.DentistId == id);

            // Có thể kiểm tra thêm các bảng khác tùy thuộc vào yêu cầu
            return inUseDentist;
        }

        public async Task UpdateAsync(Dentist entity)
        {
            var existingDentist = await _context.Dentists.FindAsync(entity.DentistId);
            if (existingDentist != null)
            {
                // Chỉ định các thuộc tính muốn cập nhật
                _context.Entry(existingDentist).Property(e => e.DentistName).IsModified = true;
                _context.Entry(existingDentist).Property(e => e.Specialization).IsModified = true;
                _context.Entry(existingDentist).Property(e => e.Email).IsModified = true;
                _context.Entry(existingDentist).Property(e => e.Address).IsModified = true;
                _context.Entry(existingDentist).Property(e => e.WorkSchedule).IsModified = true;
                _context.Entry(existingDentist).Property(e => e.Avatar).IsModified = true;


                existingDentist.DentistName = entity.DentistName;
                existingDentist.Specialization = entity.Specialization;
                existingDentist.Address = entity.Address;
                existingDentist.WorkSchedule = entity.WorkSchedule;
                existingDentist.Email = entity.Email;
                existingDentist.Avatar = entity.Avatar;
                await _context.SaveChangesAsync();
            }
        }
    }
}
