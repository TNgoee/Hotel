using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class CustomerDiscountService : ICustomDiscountService
    {
        private readonly IMongoCollection<CustomerDiscount> _customerDiscount;
        public CustomerDiscountService(MongoDBService mongoDBService)
        {
            _customerDiscount = mongoDBService.GetCollection<CustomerDiscount>("CustomerDiscount");
        }

        public async Task CreateCustomerDiscount(CustomerDiscount customerDiscount)
        {
            if (customerDiscount == null)
            {
                throw new ArgumentNullException(nameof(customerDiscount));
            }
            customerDiscount.Id = null;
            await _customerDiscount.InsertOneAsync(customerDiscount);
        }

        public async Task<bool> DeleteCustomerDiscount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _customerDiscount.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<CustomerDiscount>> GetAllCustomerDiscount()
        {
            return await _customerDiscount.Find(_ => true).ToListAsync();
        }

        public async Task<CustomerDiscount?> GetCustomerDiscountById(string id)
        {
            return await _customerDiscount.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCustomerDiscount(string id, CustomerDiscount customerDiscount)
        {
            if (string.IsNullOrEmpty(id) || customerDiscount == null)
            {
                return false;
            }
            var result = await _customerDiscount.ReplaceOneAsync(s => s.Id == id, customerDiscount);
            return result.ModifiedCount > 0;
        }
    }
}