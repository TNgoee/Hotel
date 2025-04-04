using KhachSan.Models;

namespace KhachSan.Services
{
    public interface ICustomDiscountService
    {
        Task<List<CustomerDiscount>> GetAllCustomerDiscount();
        Task<CustomerDiscount?> GetCustomerDiscountById(string id);
        Task CreateCustomerDiscount(CustomerDiscount customerDiscount);
        Task<bool> UpdateCustomerDiscount(string id, CustomerDiscount customerDiscount);
        Task<bool> DeleteCustomerDiscount(string id);
    }
}