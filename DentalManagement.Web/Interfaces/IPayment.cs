using DentalManagement.DomainModels;

namespace DentalManagement.Web.Interfaces
{
    public interface IPayment 
    {
        Task AddPaymentAsync(Payment payment);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId);
    }
}
