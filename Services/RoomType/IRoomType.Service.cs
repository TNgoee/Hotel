using KhachSan.Models;
namespace KhachSan.Services
{
    public interface IRoomTypeService
    {
        Task<List<RoomType>> GetAllRoomType();
        Task<RoomType?> GetRoomTypeById(string id);
        Task CreateRoomType(RoomType roomType, IFormFile imageFile);
        Task<bool> UpdateRoomType(string id, RoomType roomType);
        Task<bool> DeleteRoomType(string id);
    }
}