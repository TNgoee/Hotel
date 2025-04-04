using KhachSan.Models;

namespace KhachSan.Services
{
    public interface IReviewServiceService
    {
        Task<List<ReviewService>> GetAllReviewService();
        Task<ReviewService?> GetReviewServiceById(string id);
        Task CreateReviewService(ReviewService reviewService);
        Task<bool> UpdateReviewService(string id, ReviewService reviewService);
        Task<bool> DeleteReviewService(string id);
    }
}