using KhachSan.Services;
using MongoDB.Driver;

namespace KhachSan.Models
{
    public class RoomService : IRoomService
    {
        private readonly IMongoCollection<Room> _rooms;
        public RoomService(MongoDBService mongoDBService)
        {
            _rooms = mongoDBService.GetCollection<Room>("Room");
        }

        public async Task CreateRoom(Room room, IFormFile imageFile)
        {
            if (room == null || imageFile == null)
            {
                throw new ArgumentNullException(nameof(room));

            }
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var dropboxService = new DropboxService();
            room.Img = await dropboxService.UploadFileToRoomAsync(memoryStream, imageFile.FileName);
            room.Id = null;
            await _rooms.InsertOneAsync(room);
        }

        public async Task<bool> DeleteRoom(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _rooms.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Room>> GetAllRoom()
        {
            return await _rooms.Find(_ => true).ToListAsync();
        }

        public async Task<Room?> GetRoomById(string roomId)
        {
            return await _rooms.Find(r => r.Id == roomId).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRoom(string id, Room rooms)
        {
            var existingRoom = await _rooms.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (existingRoom == null)
            { return false; }
            var updateDefinition = Builders<Room>.Update
            .Set(r => r.NameRoom, rooms.NameRoom)
              .Set(r => r.Floor, rooms.Floor)
                .Set(r => r.Status, rooms.Status)
                  .Set(r => r.RoomTypeId, rooms.RoomTypeId)
                  .Set(r => r.Img, rooms.Img);
            var result = await _rooms.UpdateOneAsync(r => r.Id == id, updateDefinition);
            return result.ModifiedCount > 0;
        }
    }

}