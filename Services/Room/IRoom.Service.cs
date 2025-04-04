using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IRoomService
    {
        Task<List<Room>> GetAllRoom();
        Task<Room?> GetRoomById(string roomId);
        Task CreateRoom(Room room, IFormFile imageFile);
        Task<bool> UpdateRoom(string id, Room rooms);
        Task<bool> DeleteRoom(string id);
    }
}