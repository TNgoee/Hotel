using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IMongoCollection<RoomType> _roomType;
        private readonly ICloudinaryService _cloudinaryService;

        public RoomTypeService(MongoDBService mongoDBService, ICloudinaryService cloudinaryService)
        {
            _roomType = mongoDBService.GetCollection<RoomType>("RoomType");
            _cloudinaryService = cloudinaryService;
        }

        public async Task CreateRoomType(RoomType roomType, IFormFile imageFile)
        {
            if (roomType == null || imageFile == null)
            {
                throw new ArgumentNullException(nameof(roomType));
            }

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Upload ảnh lên Cloudinary thay vì Dropbox
            roomType.Img = await _cloudinaryService.UploadFileToRoomServiceAsync(memoryStream, imageFile.FileName);

            roomType.Id = null; // Đảm bảo không truyền id cũ khi tạo mới
            await _roomType.InsertOneAsync(roomType);
        }

        public async Task<bool> DeleteRoomType(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            var roomType = await _roomType.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (roomType != null && !string.IsNullOrEmpty(roomType.Img))
            {
                // Xóa ảnh khỏi Cloudinary
                await _cloudinaryService.DeleteFileAsync(roomType.Img);
            }

            var result = await _roomType.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<RoomType>> GetAllRoomType()
        {
            return await _roomType.Find(_ => true).ToListAsync();
        }

        public async Task<RoomType?> GetRoomTypeById(string id)
        {
            return await _roomType.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRoomType(string id, RoomType roomType)
        {
            var existingRoomType = await _roomType.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (existingRoomType == null)
            {
                return false;
            }

            var updateDefinition = Builders<RoomType>.Update
                .Set(r => r.RoomTypeName, roomType.RoomTypeName)
                .Set(r => r.Img, roomType.Img)
                .Set(r => r.Description, roomType.Description)
                .Set(r => r.Price, roomType.Price)
                .Set(r => r.RoomCount, roomType.RoomCount)
                .Set(r => r.ServiceIds, roomType.ServiceIds);

            // Nếu có cập nhật ảnh mới, upload lên Cloudinary
            if (roomType.Img != existingRoomType.Img)
            {
                if (!string.IsNullOrEmpty(existingRoomType.Img))
                {
                    await _cloudinaryService.DeleteFileAsync(existingRoomType.Img); // Xóa ảnh cũ nếu có
                }

                var uploadedImageUrl = await _cloudinaryService.UploadFileToRoomServiceAsync(new MemoryStream(), roomType.Img);
                roomType.Img = uploadedImageUrl; // Cập nhật ảnh mới
            }

            var result = await _roomType.UpdateOneAsync(r => r.Id == id, updateDefinition);
            return result.ModifiedCount > 0;
        }
    }
}
