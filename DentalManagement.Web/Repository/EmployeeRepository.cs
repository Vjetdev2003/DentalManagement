using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace DentalManagement.Web.Repository
{
    public class EmployeeRepository : IRepository<Employee>
    {
        private readonly DentalManagementDbContext _context;

        public EmployeeRepository(DentalManagementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Employee entity)
        {
            await _context.Employees.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {
            var query = _context.Employees.AsQueryable();

            // Nếu có giá trị tìm kiếm, lọc danh sách theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(e => e.FullName.Contains(searchValue) || e.Email.Contains(searchValue));
            }

            // Thực hiện phân trang
            var employees = await query
            .Skip((page - 1) * pagesize) // Bỏ qua các bản ghi trước đó
            .Take(pagesize)               // Lấy số bản ghi theo kích thước trang
                .ToListAsync();               // Chuyển đổi kết quả thành danh sách

            return employees;
        }

        public Task<Patient?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public Task<IEnumerable<Employee>> GetDetailAsyncById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Employee>> GetElementById(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public bool InUse(int id)
        {
            // Kiểm tra xem Employee có đang được sử dụng trong các bản ghi khác hay không
            bool inUseInEmployee = _context.Set<Employee>().Any(o => o.EmployeeId == id);

            // Có thể kiểm tra thêm các bảng khác tùy thuộc vào yêu cầu
            return inUseInEmployee;
        }


        public async Task UpdateAsync(Employee entity)
        {
            var existingEmployee = await _context.Employees.FindAsync(entity.EmployeeId);
            if (existingEmployee != null)
            {
                // Chỉ định các thuộc tính muốn cập nhật
                _context.Entry(existingEmployee).Property(e => e.FullName).IsModified = true;
                _context.Entry(existingEmployee).Property(e => e.DateOfBirth).IsModified = true;
                _context.Entry(existingEmployee).Property(e => e.Address).IsModified = true;
                _context.Entry(existingEmployee).Property(e => e.Phone).IsModified = true;
                _context.Entry(existingEmployee).Property(e => e.Email).IsModified = true;
                _context.Entry(existingEmployee).Property(e => e.Avatar).IsModified = true;


                existingEmployee.FullName = entity.FullName;
                existingEmployee.DateOfBirth = entity.DateOfBirth;
                existingEmployee.Address = entity.Address;
                existingEmployee.Phone = entity.Phone;
                existingEmployee.Email = entity.Email;
                existingEmployee.Avatar = entity.Avatar;
                await _context.SaveChangesAsync();
            }
        }
    }
}
