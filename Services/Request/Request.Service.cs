using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class RequestService : IRequestService
    {
        private readonly IMongoCollection<Request> _request;
        public RequestService(MongoDBService mongoDBService)
        {
            _request = mongoDBService.GetCollection<Request>("Request");
        }

        public async Task CreateRequest(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.Id = null;
            await _request.InsertOneAsync(request);
        }

        public async Task<bool> DeleteRequest(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _request.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Request>> GetAllRequest()
        {
            return await _request.Find(_ => true).ToListAsync();
        }

        public async Task<Request?> GetRequestById(string id)
        {
            return await _request.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRequest(string id, Request request)
        {
            if (string.IsNullOrEmpty(id) || request == null)
            {
                return false;
            }
            var result = await _request.ReplaceOneAsync(s => s.Id == id, request);
            return result.ModifiedCount > 0;
        }
    }
}