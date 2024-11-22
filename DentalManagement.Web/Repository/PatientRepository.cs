using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class PatientRepository : IRepository<Patient>
    {
        private readonly DentalManagementDbContext _context;

        public PatientRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Patient entity)
        {
             _context.Patients.Add(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
            return _context.Patients.Count();
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {

            var query = _context.Patients.AsQueryable();

            // Nếu có giá trị tìm kiếm, lọc danh sách theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(e => e.PatientName.Contains(searchValue) || e.Email.Contains(searchValue));
            }

            // Thực hiện phân trang
            var patients = await query
            .Skip((page - 1) * pagesize) // Bỏ qua các bản ghi trước đó
            .Take(pagesize)               // Lấy số bản ghi theo kích thước trang
                .ToListAsync();               // Chuyển đổi kết quả thành danh sách

            return patients;
        }
      


        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
            
        }

        public async Task<IEnumerable<Patient>> GetDetailAsyncById(int id)
        {
            return await Task.Run(() =>
            {
                return _context.Patients.Where(a => a.PatientId == id);
            });
        }

        public Task<List<Patient>> GetElementById(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public bool  InUse(int id)
        {
            bool inUsePatient = _context.Set<Patient>().Any(o => o.PatientId == id);

            // Có thể kiểm tra thêm các bảng khác tùy thuộc vào yêu cầu
            return inUsePatient;
        }

        public async Task UpdateAsync(Patient entity)
        {
            var existingPatient = await _context.Patients.FindAsync(entity.PatientId);
            if (existingPatient != null)
            {
                // Chỉ định các thuộc tính muốn cập nhật
                _context.Entry(existingPatient).Property(e => e.PatientName).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.PatientDOB).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.Height).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.Weight).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.Gender).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.Phone).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.Email).IsModified = true;
                _context.Entry(existingPatient).Property(e => e.Address).IsModified = true;


                existingPatient.PatientName = entity.PatientName;
                existingPatient.PatientDOB = entity.PatientDOB;
                existingPatient.Height = entity.Height;
                existingPatient.Weight = entity.Weight;
                existingPatient.Gender = entity.Gender;

                existingPatient.Address = entity.Address;
                existingPatient.Phone = entity.Phone;
                existingPatient.Email = entity.Email;
                await _context.SaveChangesAsync();
            }
        }
    }
}
