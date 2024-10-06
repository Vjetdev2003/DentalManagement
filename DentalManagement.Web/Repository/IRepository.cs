using DentalManagement.DomainModels;

namespace DentalManagement.Web.Repository
{
    public interface IRepository< T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(int page = 1, int pagesize = 10, string searchValue = "");
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        bool InUse(int id);

    }
}
