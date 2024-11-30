using DentalManagement.DomainModels;
using DentalManagement.Web.Data;

namespace DentalManagement.Web.Repository
{
    public class MessageRepository
    {
        private readonly DentalManagementDbContext _context;

        public MessageRepository(DentalManagementDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Message entity)
        {
            _context.Messages.Add(entity);
            await _context.SaveChangesAsync();
        }
        public int Count()
        {
            return _context.Messages.Count();
        }
    }
}
