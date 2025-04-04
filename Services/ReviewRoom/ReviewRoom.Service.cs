using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class ReviewRoomService : IReviewRoomService
    {
        private readonly IMongoCollection<ReviewRoom> _reviewRoom;
        public ReviewRoomService(MongoDBService mongoDBService)
        {
            _reviewRoom = mongoDBService.GetCollection<ReviewRoom>("ReviewRoom");
        }

        public async Task CreateReviewRoom(ReviewRoom reviewRoom)
        {
            if (reviewRoom == null)
            {
                throw new ArgumentNullException(nameof(reviewRoom));
            }
            reviewRoom.Id = null;
            await _reviewRoom.InsertOneAsync(reviewRoom);
        }

        public async Task<bool> DeleteReviewRoom(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _reviewRoom.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<ReviewRoom>> GetAllReviewRoom()
        {
            return await _reviewRoom.Find(_ => true).ToListAsync();
        }

        public async Task<ReviewRoom?> GetReviewRoomById(string id)
        {
            return await _reviewRoom.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateReviewRoom(string id, ReviewRoom reviewRoom)
        {
            if (string.IsNullOrEmpty(id) || reviewRoom == null)
            {
                return false;
            }
            var result = await _reviewRoom.ReplaceOneAsync(s => s.Id == id, reviewRoom);
            return result.ModifiedCount > 0;
        }
    }
}