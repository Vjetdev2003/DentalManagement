using DentalManagement.DomainModels;

namespace DentalManagement.Web.Interfaces
{
    public interface IUserAccount
    {
        Task<UserAccount?> Authorize(string userName, string password);
        Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);
        Task<bool> Register(string userName, string password, string email);
    }
}
