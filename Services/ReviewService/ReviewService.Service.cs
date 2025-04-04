using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class ReviewServiceService : IReviewServiceService
    {
        private readonly IMongoCollection<ReviewService> _reviewService;
        public ReviewServiceService(MongoDBService mongoDBService)
        {
            _reviewService = mongoDBService.GetCollection<ReviewService>("ReviewService");
        }

        public async Task CreateReviewService(ReviewService reviewService)
        {
            if (reviewService == null)
            {
                throw new ArgumentNullException(nameof(reviewService));
            }
            reviewService.Id = null;
            await _reviewService.InsertOneAsync(reviewService);
        }

        public async Task<bool> DeleteReviewService(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _reviewService.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<ReviewService>> GetAllReviewService()
        {
            return await _reviewService.Find(_ => true).ToListAsync();
        }

        public async Task<ReviewService?> GetReviewServiceById(string id)
        {
            return await _reviewService.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateReviewService(string id, ReviewService reviewService)
        {
            if (string.IsNullOrEmpty(id) || reviewService == null)
            {
                return false;
            }
            var result = await _reviewService.ReplaceOneAsync(s => s.Id == id, reviewService);
            return result.ModifiedCount > 0;
        }
    }
}