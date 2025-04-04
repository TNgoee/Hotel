using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IDiscountService
    {
        Task<List<Discount>> GetAllDiscount();
        Task<Discount?> GetDiscountById(string id);
        Task CreateDiscount(Discount discount, IFormFile imageFile);

        Task<bool> UpdateDiscount(string id, Discount discount);
        Task<bool> DeleteDiscount(string id);
    }
}