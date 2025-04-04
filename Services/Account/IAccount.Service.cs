using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAccount();
        Task<Account?> GetAccountById(string id);
        Task CreateAccount(Account account);
        Task<bool> UpdateAccount(string id, Account account);

        Task<bool> DeleteAccount(string id);
    }
}