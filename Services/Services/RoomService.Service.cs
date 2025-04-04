using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class RoomServiceService : IRoomServiceService
    {
        private readonly IMongoCollection<Service> _services;
        public RoomServiceService(MongoDBService mongoDBService)
        {
            _services = mongoDBService.GetCollection<Service>("Service");
        }

        public async Task CreateService(Service service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            service.Id = null;
            await _services.InsertOneAsync(service);
        }

        public async Task<bool> DeleteService(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _services.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Service>> GetAllService()
        {
            return await _services.Find(_ => true).ToListAsync();
        }

        public async Task<Service?> GetServiceById(string id)
        {
            return await _services.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateService(string id, Service updateservice)
        {
            if (string.IsNullOrEmpty(id) || updateservice == null)
            {
                return false;
            }
            var result = await _services.ReplaceOneAsync(s => s.Id == id, updateservice);
            return result.ModifiedCount > 0;
        }
    }
}