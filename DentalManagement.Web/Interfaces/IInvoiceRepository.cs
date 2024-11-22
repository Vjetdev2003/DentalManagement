using DentalManagement.DomainModels;

namespace DentalManagement.Web.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(int id);
        Task AddAsync(Invoice entity);
        Task UpdateAsync(Invoice entity);
        Task<bool> DeleteAsync(int id);
        bool InUse(int id);
        int CountAsync();
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<IEnumerable<Invoice>> GetInvoicesAsyncById(int id);
        Task<bool> UnPaid(int invoiceId);
        Task<bool> CancelInvoice(int invoiceId);
        Task<bool> FailedInvoice(int invoiceId);
        Task<bool> PaidInvoice(int invoiceId);
        Task<int> Init(int employeeId, int patientId, IEnumerable<InvoiceDetails> details);
        Task<bool> SaveDetails(int invoiceId = -1, int serviceId = -1, int quantity = -1, decimal salePrice = -1,string serviceName="");
    }
}
