using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IRoomServiceService
    {
        Task<List<Service>> GetAllService();
        Task<Service?> GetServiceById(string id);
        Task CreateService(Service service);
        Task<bool> UpdateService(string id, Service service);
        Task<bool> DeleteService(string id);
    }
}