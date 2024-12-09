using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class MedicineRepository : IRepository<Medicine>
    {
        private readonly DentalManagementDbContext _context;

        public MedicineRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Medicine entity)
        {
            _context.Medicines.Add(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {
            return _context.Medicines.Count();
        }

        public async Task DeleteAsync(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<IEnumerable<Medicine>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {

            var query = _context.Medicines.AsQueryable();

            // Nếu có giá trị tìm kiếm, lọc danh sách theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(e => e.MedicineName.Contains(searchValue));
            }

            // Thực hiện phân trang
            var medicines = await query
            .Skip((page - 1) * pagesize) // Bỏ qua các bản ghi trước đó
            .Take(pagesize)               // Lấy số bản ghi theo kích thước trang
                .ToListAsync();               // Chuyển đổi kết quả thành danh sách

            return medicines;
        }

        public async Task<Medicine> GetByIdAsync(int id)
        {
            return await _context.Medicines.FindAsync(id);
        }

        public Task<IEnumerable<Medicine>> GetDetailAsyncById(int id)
        {
            throw new NotImplementedException();
        }


        public bool InUse(int id)
        {
            bool inUseMedicine = _context.Set<Medicine>().Any(o => o.MedicineId == id);

            // Có thể kiểm tra thêm các bảng khác tùy thuộc vào yêu cầu
            return inUseMedicine;
        }

        public async Task<IEnumerable<Medicine>> ListAlll(string searchValue = "")
        {
            var query = _context.Medicines.AsQueryable();

            // Nếu có giá trị tìm kiếm, lọc danh sách theo tên dịch vụ
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(e => e.MedicineName.Contains(searchValue));
            }

            // Thực hiện phân trang
            var medicines = await query.ToListAsync();              // Chuyển đổi kết quả thành danh sách

            return medicines;
        }

        public async Task UpdateAsync(Medicine entity)
        {
            var existingMedicine = await _context.Medicines.FindAsync(entity.MedicineId);
            if (existingMedicine != null)
            {
                try
                { 
                    // Chỉ định các thuộc tính muốn cập nhật
                    existingMedicine.MedicineName = entity.MedicineName;
                    existingMedicine.Unit = entity.Unit;
                    existingMedicine.DosageInstructions = entity.DosageInstructions;
                    existingMedicine.StockQuantity = entity.StockQuantity;
                    existingMedicine.Usage = entity.Usage;
                    existingMedicine.Price = entity.Price;
                    existingMedicine.Photo = entity.Photo;

                    await _context.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    File.AppendAllText("log_medicine.txt", ex.Message + "\n" + ex.Source);
                }
            }
        }
    }
}
