using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly DentalManagementDbContext _context;

        public UserAccountRepository(DentalManagementDbContext context)
        {
            _context = context;
        }

        public async Task<UserAccount?> Authorize(string userName, string password)
        {
            // Kiểm tra trong bảng Employees
            var employeeAccount = await _context.Employees
                .Where(e => e.Email == userName && e.Password == password)
                .Select(e => new UserAccount
                {
                    UserID = e.EmployeeId.ToString(),
                    UserName = e.Email,
                    FullName = e.FullName,
                    Email = e.Email,
                    Photo = e.Avatar,
                    RoleNames = e.RoleName
                })
                .FirstOrDefaultAsync();

            if (employeeAccount != null)
                return employeeAccount;

            // Kiểm tra trong bảng Patients
            var patientAccount = await _context.Patients
                .Where(p => p.Email == userName && p.Password == password)
                .Select(p => new UserAccount
                {
                    UserID = p.PatientId.ToString(),
                    UserName = p.Email,
                    FullName = p.PatientName,
                    Email = p.Email,
                    RoleNames = "patient",
                    Photo = p.Photo,
                })
                .FirstOrDefaultAsync();

            if (patientAccount != null)
                return patientAccount;

            // Kiểm tra trong bảng Dentists
            var dentistAccount = await _context.Dentists
                .Where(d => d.Email == userName && d.Password == password)
                .Select(d => new UserAccount
                {
                    UserID = d.DentistId.ToString(),
                    UserName = d.Email,
                    FullName = d.DentistName,
                    Email = d.Email,
                    Photo = d.Avatar,
                    RoleNames = "dentist"
                })
                .FirstOrDefaultAsync();

            return dentistAccount;
        }


        public async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            // Tìm kiếm trong bảng Employees
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == userName);

            if (employee != null && VerifyPassword(oldPassword, employee.Password)) // Kiểm tra mật khẩu cũ
            {
                employee.Password = HashPassword(newPassword); // Cập nhật mật khẩu
                _context.Employees.Update(employee); // Cập nhật thông tin
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return true;
            }

            // Tìm kiếm trong bảng Patients
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == userName);

            if (patient != null && VerifyPassword(oldPassword, patient.Password)) // Kiểm tra mật khẩu cũ
            {
                patient.Password = HashPassword(newPassword); // Cập nhật mật khẩu
                _context.Patients.Update(patient); // Cập nhật thông tin
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return true;
            }

            // Tìm kiếm trong bảng Dentists
            var dentist = await _context.Dentists
                .FirstOrDefaultAsync(d => d.Email == userName);

            if (dentist != null && VerifyPassword(oldPassword, dentist.Password)) // Kiểm tra mật khẩu cũ
            {
                dentist.Password = HashPassword(newPassword); // Cập nhật mật khẩu
                _context.Dentists.Update(dentist); // Cập nhật thông tin
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return true;
            }

            // Nếu không tìm thấy người dùng nào
            return false;
        }




        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Thực hiện kiểm tra mật khẩu
            // Implement your password verification logic here
            return password == hashedPassword; // Chỉ là ví dụ, bạn nên dùng phương thức hash để kiểm tra
        }

        private string HashPassword(string password)
        {
            // Implement your password hashing logic here
            return password; // Chỉ là ví dụ, bạn nên sử dụng thư viện mã hóa như BCrypt
        }

        public async Task<bool> Register(string userName, string password, string email)
        {
            var existingUser = await _context.Patients
         .FirstOrDefaultAsync(u => u.PatientName == userName || u.Email == email);

            if (existingUser != null)
            {
                // Tên người dùng hoặc email đã tồn tại
                return false; // Hoặc có thể ném ra một exception hoặc trả về một mã lỗi khác
            }
           
            // Tạo một đối tượng người dùng mới
            var newUser = new Patient
            {
                PatientName = userName,
                Email = email,
                Password = HashPassword(password), // Giả sử bạn có hàm để mã hóa mật khẩu
                DateCreated = DateTime.UtcNow, 
                RoleName = "patient" // Ghi lại ngày tạo tài khoản         // Thiết lập UserId người tạo
            };

            // Thêm người dùng vào DbSet
            await _context.Patients.AddAsync(newUser);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            return true; // Đăng ký thành công
        }
    }
}
