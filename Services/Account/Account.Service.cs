using KhachSan.Services;
using MongoDB.Driver;

namespace KhachSan.Models
{
    public class AccountService : IAccountService
    {
        private readonly IMongoCollection<Account> _account;
        public AccountService(MongoDBService mongoDBService)
        {
            _account = mongoDBService.GetCollection<Account>("Account");
        }

        public async Task CreateAccount(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            account.Id = null;
            await _account.InsertOneAsync(account);
        }

        public async Task<bool> DeleteAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _account.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<Account?> GetAccountById(string id)
        {
            return await _account.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Account>> GetAllAccount()
        {
            return await _account.Find(_ => true).ToListAsync();
        }

        public async Task<bool> UpdateAccount(string id, Account account)
        {
            if (string.IsNullOrEmpty(id) || account == null)
            {
                return false;
            }
            var result = await _account.ReplaceOneAsync(s => s.Id == id, account);
            return result.ModifiedCount > 0;
        }
    }
}