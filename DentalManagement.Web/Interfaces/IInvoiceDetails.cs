using DentalManagement.DomainModels;

namespace DentalManagement.Web.Interfaces
{
    public interface IInvoiceDetails
    {
        Task<InvoiceDetails> GetByIdAsync(int id);
        Task<IEnumerable<InvoiceDetails>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "");
        Task<List<InvoiceDetails>> GetInvoicesByIdAsync(int invoiceId);
        IQueryable<InvoiceDetails> GetInvoicesById(int invoiceId);
    }
}
