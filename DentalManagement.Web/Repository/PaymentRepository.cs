using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Repository
{
    public class PaymentRepository : IPayment
    {
        private readonly DentalManagementDbContext _context;

        public PaymentRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddPaymentAsync(Payment payment)
        {
             _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);  
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
           return await _context.Payments.Where(p=>p.InvoiceId == invoiceId).ToListAsync();
        }

        public int Count()
        {
            return _context.Payments.Count();
        }
        public async Task<List<Payment>> GetPaymentsByPatientIdAsync(int patientId)
        {
            try
            {
                var payments = await (
                    from payment in _context.Payments
                    join invoice in _context.Invoices
                    on payment.InvoiceId equals invoice.InvoiceId
                    where invoice.PatientId == patientId
                    orderby payment.DateCreated
                    select payment
                ).OrderByDescending(p=>p.DateUpdated)
                .ToListAsync();


                return payments;  // Trả về danh sách các khoản thanh toán
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error fetching payments: {ex.Message}");
                return null;
            }
        }
    }
}
