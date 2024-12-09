using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication.ExtendedProtection;

namespace DentalManagement.Web.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DentalManagementDbContext _context;
        private readonly IRepository<Employee> _employeeRepository;  // Injecting EmployeeRepository
        private readonly IRepository<Patient> _patientRepository;
        public InvoiceRepository(DentalManagementDbContext context, IRepository<Employee> employeeRepository, IRepository<Patient> patientRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
            _patientRepository = patientRepository;
        }

        public async Task AddAsync(Invoice entity)
        {
            _context.Invoices.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync(int page = 1, int pageSize = 10, string searchValue = "")
        {
            var query = _context.Invoices.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                // Giả sử bạn tìm kiếm theo tên bệnh nhân hoặc các trường liên quan
                query = query.Where(i => i.Patient.PatientName.Contains(searchValue));
            }
            query = query.OrderByDescending(i => i.DateCreated);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.Include(i => i.Patient).FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public bool InUse(int id)
        {
            // Cần xác định cách xác định xem hoá đơn có đang được sử dụng hay không
            // Ví dụ: kiểm tra nếu hoá đơn có trạng thái thanh toán là đã thanh toán
            var invoice = _context.Invoices.Find(id);
            return invoice != null && invoice.Status == 1; // Thay đổi theo logic của bạn
        }

        public async Task UpdateAsync(Invoice entity)
        {
            _context.Invoices.Update(entity);
            await _context.SaveChangesAsync();
        }

        public int CountAsync()
        {

            return _context.Invoices.Count();
        }


        public async Task<IEnumerable<Invoice>> GetInvoicesAsyncById(int id)
        {
            return await Task.Run(() =>
            {
                return _context.Invoices.Where(a => a.InvoiceId == id);
            });
        }

        public async Task<bool> UnPaid(int invoiceId)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (invoice == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (invoice.Status == Constants_Invoice.INVOICE_UNPAID)
            {
                // Cập nhật trạng thái của lịch hẹn
                invoice.Status = Constants_Invoice.INVOICE_PROCESSING;
                invoice.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái khởi tạo
            return false;
        }

        public async Task<bool> CancelInvoice(int invoiceId)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (invoice == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (invoice.Status != Constants_Invoice.INVOICE_PAID)
            {
                // Cập nhật trạng thái của lịch hẹn
                invoice.Status = Constants_Invoice.INVOICE_FAILED;
                invoice.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn đã hoàn tất
            return false;
        }

        public async Task<bool> FailedInvoice(int invoiceId)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (invoice == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (invoice.Status == Constants_Invoice.INVOICE_UNPAID)
            {
                // Cập nhật trạng thái của lịch hẹn
                invoice.Status = Constants_Invoice.INVOICE_FAILED; // Thay thế với trạng thái từ Constants
                invoice.DateUpdated = DateTime.Now; // Cập nhật thời gian
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái khởi tạo
            return false;
        }

        public async Task<bool> PaidInvoice(int invoiceId)
        {
            // Lấy thông tin lịch hẹn từ cơ sở dữ liệu bằng ID
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            // Kiểm tra nếu lịch hẹn không tồn tại
            if (invoice == null)
                return false;

            // Kiểm tra trạng thái hiện tại của lịch hẹn
            if (invoice.Status == Constants_Invoice.INVOICE_PROCESSING)
            {
                // Cập nhật trạng thái của lịch hẹn
                invoice.Status = Constants_Invoice.INVOICE_PAID;
                invoice.FinishTime = DateTime.Now; // Cập nhật thời gian hoàn tất
                await _context.SaveChangesAsync(); // Lưu cập nhật vào cơ sở dữ liệu
                return true;
            }

            // Trả về false nếu lịch hẹn không ở trạng thái đã xác nhận
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Find the appointment by ID
            var invoice = await _context.Invoices.FindAsync(id);

            // Check if the appointment exists
            if (invoice == null)
            {
                return false; // Appointment not found
            }

            // Check if the appointment is finished, prevent deletion if it is
            if (invoice.Status == Constants_Invoice.INVOICE_PAID)
            {
                throw new InvalidOperationException("Cannot delete a completed appointment.");
            }

            // Remove the appointment from the database
            _context.Invoices.Remove(invoice);

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            return true; // Deletion was successful
        }
        public async Task<int> Init(int employeeId, int patientId, IEnumerable<InvoiceDetails> details)
        {

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (employee == null || patient == null)
                throw new Exception("Employee or patient does not exist.");

            // Tính tổng tiền
            decimal totalAmount = details.Sum(d => d.Quantity * d.SalePrice);

            // Tạo hóa đơn mới
            var invoice = new Invoice
            {
                EmployeeId = employee.EmployeeId,
                PatientId = patient.PatientId,
                PatientName = patient.PatientName,
                EmployeeName = employee.FullName,
                PatientAddress = patient.Address,
                TotalPrice = totalAmount,
                Status = Constants_Invoice.INVOICE_UNPAID,
                PaymentMethod = Paymethod.CASH,  // Cập nhật phương thức thanh toán nếu cần
                DateCreated = DateTime.Now,
                Discount = 0,
            };

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            // Lấy ID hóa đơn mới tạo
            int invoiceId = invoice.InvoiceId;
            if (invoiceId > 0)
            {
                foreach (var item in details)
                {
                    SaveDetails(invoiceId, item.ServiceId, item.Quantity, item.SalePrice,item.ServiceName);
                }
                return invoiceId;
            } return 0;
        
        }
            // Thêm chi tiết vào InvoiceDetails
        //    foreach (var item in details)
        //    {
        //        var invoiceDetail = new InvoiceDetails
        //        {
        //            InvoiceId = invoiceId,
        //            ServiceId = item.ServiceId,
        //            Quantity = item.Quantity,
        //            SalePrice = item.SalePrice
        //        };

        //        await _context.InvoiceDetails.AddAsync(invoiceDetail);
        //    }

        //    // Lưu tất cả chi tiết hóa đơn
        //    await _context.SaveChangesAsync();
        //    return invoiceId;
        //}
       

        public async Task<bool> SaveDetails(int invoiceId = -1, int serviceId = -1, int quantity = -1, decimal salePrice = -1,string serviceName="")
        {
            if (invoiceId <= 0 || serviceId <= 0 || quantity <= 0 || salePrice < 0)
                return false;

            // Kiểm tra xem dịch vụ có tồn tại không
            var serviceExists = await _context.Services.AnyAsync(p => p.ServiceId == serviceId);
            if (!serviceExists)
            {
                Console.WriteLine($"Product with ID {serviceId} does not exist.");
                return false; // Không thêm chi tiết này nếu ProductId không tồn tại
            }

            var existingDetail = await _context.InvoiceDetails
                .FirstOrDefaultAsync(od => od.InvoiceId == invoiceId && od.ServiceId == serviceId);

            if (existingDetail != null)
            {
                
                existingDetail.Quantity += quantity; // Cộng thêm số lượng
                existingDetail.SalePrice = salePrice; // Cập nhật giá bán
            }
            else
            {

                var newDetail = new InvoiceDetails
                {
                    InvoiceId = invoiceId,
                    ServiceId = serviceId,
                    ServiceName = serviceName,
                    Quantity = quantity,
                    SalePrice = salePrice,
                   // PaymentStatus = "Chưa thanh toán"
                };
                await _context.InvoiceDetails.AddAsync(newDetail);
            }

            return await _context.SaveChangesAsync() > 0;
        }

    }
}