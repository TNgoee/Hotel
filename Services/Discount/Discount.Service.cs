using KhachSan.Models;
using MongoDB.Driver;

namespace KhachSan.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IMongoCollection<Discount> _discount;
        private readonly ICloudinaryService _cloudinaryService;  // Thêm ICloudinaryService

        // Constructor nhận ICloudinaryService
        public DiscountService(MongoDBService mongoDBService, ICloudinaryService cloudinaryService)
        {
            _discount = mongoDBService.GetCollection<Discount>("Discount");
            _cloudinaryService = cloudinaryService;  // Khởi tạo ICloudinaryService
        }

        // Phương thức tạo mới discount
        public async Task CreateDiscount(Discount discount, IFormFile imageFile)
        {
            if (discount == null || imageFile == null)
            {
                throw new ArgumentNullException(nameof(discount));
            }

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Upload ảnh lên Cloudinary thay vì Dropbox
            discount.Img = await _cloudinaryService.UploadFileAsync(memoryStream, imageFile.FileName);

            discount.Id = null;
            await _discount.InsertOneAsync(discount);
        }

        public async Task<bool> DeleteDiscount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var result = await _discount.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Discount>> GetAllDiscount()
        {
            return await _discount.Find(_ => true).ToListAsync();
        }

        public async Task<Discount?> GetDiscountById(string id)
        {
            return await _discount.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateDiscount(string id, Discount discount)
        {
            var existingDiscount = await _discount.Find(d => d.Id == id).FirstOrDefaultAsync();
            if (existingDiscount == null)
            {
                return false;
            }
            var updateDefinition = Builders<Discount>.Update
                .Set(d => d.NameDiscount, discount.NameDiscount)
                .Set(d => d.MinAmount, discount.MinAmount)
                .Set(d => d.MaxQuantity, discount.MaxQuantity)
                .Set(d => d.StartDate, discount.StartDate)
                .Set(d => d.EndDate, discount.EndDate)
                .Set(d => d.Img, discount.Img);

            var result = await _discount.UpdateOneAsync(d => d.Id == id, updateDefinition);

            return result.ModifiedCount > 0;
        }
    }
}
