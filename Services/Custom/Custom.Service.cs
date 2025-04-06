using KhachSan.Services;
using MongoDB.Driver;

namespace KhachSan.Models
{
    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<Customer> _customer;
        private readonly IAccountService _accountService;
        public CustomerService(MongoDBService mongoDBService, IAccountService accountService)
        {
            _customer = mongoDBService.GetCollection<Customer>("Customer");
            _accountService = accountService;
        }

        public async Task CreateCustomer(Customer customer, IFormFile imageFile)
        {
            if (customer == null || imageFile == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var dropboxService = new DropboxService();
            customer.Avatar = await dropboxService.UploadFileToAvatarAsync(memoryStream, imageFile.FileName);

            customer.Id = null;
            await _customer.InsertOneAsync(customer);
        }


        public async Task<bool> DeleteCustomer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _customer.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Customer>> GetAllCustomer()
        {
            // Lấy danh sách tất cả khách hàng
            var customers = await _customer.Find(_ => true).ToListAsync();


            var accountTasks = customers.Select(async customer =>
            {
                var account = await _accountService.GetAccountById(customer.AccountId);
                if (account != null)
                {
                    customer.Account = account;
                }
                else
                {
                    Console.WriteLine($"Account with Id {customer.AccountId} not found.");
                }
            }).ToList();

            // Đợi tất cả các task hoàn thành
            await Task.WhenAll(accountTasks);

            return customers;
        }



        public async Task<Customer?> GetCustomerById(string id)
        {
            return await _customer.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCustomer(string id, Customer customer)
        {
            var existingCustomer = await _customer.Find(d => d.Id == id).FirstOrDefaultAsync();
            if (existingCustomer == null)
            {
                return false;
            }
            var updateDefinition = Builders<Customer>.Update
                .Set(d => d.Name, customer.Name)
                .Set(d => d.Birthday, customer.Birthday)
                .Set(d => d.AccountId, customer.AccountId)
                .Set(d => d.Avatar, customer.Avatar);
            var result = await _customer.UpdateOneAsync(d => d.Id == id, updateDefinition);
            return result.ModifiedCount > 0;
        }
    }
}