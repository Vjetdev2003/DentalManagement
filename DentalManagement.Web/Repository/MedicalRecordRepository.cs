using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class MedicalRecordRepository 
    {
        private readonly DentalManagementDbContext _context;

        public MedicalRecordRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task<MedicalRecord> GetMedicalRecordByIdAsync(int id)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Dentist)
                .Include(mr => mr.Service)
                .FirstOrDefaultAsync(mr => mr.MedicalRecordId == id);
        }
        public int CountAsync()
        {
            return _context.MedicalRecords.Count();
        }
        public async Task<MedicalRecord> GetByIdAsync(int id)
        {
            return await _context.MedicalRecords.FindAsync(id);
        }
        public async Task<IEnumerable<MedicalRecord>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "")
        {

            var query = _context.MedicalRecords.AsQueryable();

            // Nếu có giá trị tìm kiếm, lọc danh sách theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(e => e.Patient.PatientName.Contains(searchValue));
            }

            // Thực hiện phân trang
            var medicalRecords = await query
            .Skip((page - 1) * pagesize) // Bỏ qua các bản ghi trước đó
            .Take(pagesize)               // Lấy số bản ghi theo kích thước trang
                .ToListAsync();               // Chuyển đổi kết quả thành danh sách

            return medicalRecords;
        }

        public async Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
            return medicalRecord;
        }
        public async Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            await _context.SaveChangesAsync();
            return medicalRecord;
        }

        //public async Task DeleteAsync(int id)
        //{
        //    var medicalRecord = await GetMedicalRecordByIdAsync(id);
        //    if (medicalRecord != null)
        //    {
        //        _context.MedicalRecords.Remove(medicalRecord);
        //        await _context.SaveChangesAsync();
        //    }
        //}
        public async Task<bool> UnProcess(int recordId)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var record = await _context.MedicalRecords.FindAsync(recordId);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (record == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (record.Status == Constants_MedicalRecord.UNPROCESS)
            {
                // Cập nhật trạng thái của lịch hẹn
                record.Status = Constants_MedicalRecord.PENDING;
                record.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái khởi tạo
            return false;
        }
        public async Task<bool> CancelMedicalRecord(int recordId)
        {
            
            var record = await _context.MedicalRecords.FindAsync(recordId);
            if (record == null)
                return false;
            if (record.Status != Constants_MedicalRecord.COMPLETE)
            {   
                record.Status = Constants_MedicalRecord.CANCELLED;
                record.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            return false;
        }
        public async Task<bool> FinishMedicalRecord(int recordId)
        {

            var record = await _context.MedicalRecords.FindAsync(recordId);
            if (record == null)
                return false;
            if (record.Status == Constants_MedicalRecord.PENDING)
            {
                record.Status = Constants_MedicalRecord.COMPLETE;
                await _context.SaveChangesAsync();
                return true;
            }

            
            return false;
        }
        public async Task<bool> Failed(int recordId)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var record = await _context.MedicalRecords.FindAsync(recordId);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (record == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (record.Status == Constants_MedicalRecord.UNPROCESS)
            {
                // Cập nhật trạng thái của lịch hẹn
                record.Status = Constants_MedicalRecord.FAILED; // Thay thế với trạng thái từ Constants
                record.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái khởi tạo
            return false;
        }
        public async Task<bool> SaveTreatmentReport(int id, string treatmentOutcome, DateTime nextAppointmentDate)
        {
            var record = await _context.MedicalRecords.FindAsync(id);

            if (record == null)
                return false;

            // Cập nhật thông tin báo cáo vào hồ sơ y tế
            record.TreatmentOutcome = treatmentOutcome;
            record.NextAppointmentDate = nextAppointmentDate;
            record.DateUpdated = DateTime.Now;
            record.Status = Constants_MedicalRecord.COMPLETE;

            // Lưu thay đổi vào cơ sở dữ liệu
             _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Find the appointment by ID
            var record = await _context.MedicalRecords.FindAsync(id);

            // Check if the appointment exists
            if (record == null)
            {
                return false; // Appointment not found
            }

            // Check if the appointment is finished, prevent deletion if it is
            if (record.Status == Constants_MedicalRecord.COMPLETE)
            {
                throw new InvalidOperationException("Cannot delete a completed appointment.");
            }

            // Remove the appointment from the database
            _context.MedicalRecords.Remove(record);

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            return true; // Deletion was successful
        }
    }
}
