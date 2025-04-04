using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IReviewRoomService
    {
        Task<List<ReviewRoom>> GetAllReviewRoom();
        Task<ReviewRoom?> GetReviewRoomById(string id);
        Task CreateReviewRoom(ReviewRoom reviewRoom);
        Task<bool> UpdateReviewRoom(string id, ReviewRoom reviewRoom);
        Task<bool> DeleteReviewRoom(string id);
    }
}