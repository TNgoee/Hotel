using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IRequestService
    {
        Task<List<Request>> GetAllRequest();
        Task<Request?> GetRequestById(string id);
        Task CreateRequest(Request request);
        Task<bool> UpdateRequest(string id, Request request);
        Task<bool> DeleteRequest(string id);
    }
}