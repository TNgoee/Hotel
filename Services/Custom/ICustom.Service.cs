using KhachSan.Models;

namespace KhachSan.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomer();
        Task<Customer?> GetCustomerById(string id);
        Task CreateCustomer(Customer customer, IFormFile imageFile);
        Task<bool> UpdateCustomer(string id, Customer customer);

        Task<bool> DeleteCustomer(string id);
    }
}