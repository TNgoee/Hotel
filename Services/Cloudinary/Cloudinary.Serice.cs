using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KhachSan.Services
{


    // Implementation of ICloudinaryService
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var account = new Account(
                "dooy48crn",
                "599742763179868",
                "Hj-tgFgjxeRqZqvmkyD2n0DmV98"
            );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        // Hàm tạo tên tệp tin đã mã hóa
        private string GenerateHashedFileName(string fileName)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fileName));
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return $"{hash}{Path.GetExtension(fileName)}";
        }

        // Upload file chính
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder = "Discount")
        {
            string publicId = $"{folder}/{GenerateHashedFileName(fileName)}";

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = publicId,
                Overwrite = true,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString(); // Trả về đường dẫn ảnh sau khi upload
        }

        // Upload file vào thư mục RoomService
        public Task<string> UploadFileToRoomServiceAsync(Stream fileStream, string fileName) =>
            UploadFileAsync(fileStream, fileName, "RoomService");

        // Upload file vào thư mục Room
        public Task<string> UploadFileToRoomAsync(Stream fileStream, string fileName) =>
            UploadFileAsync(fileStream, fileName, "Room");

        // Upload file vào thư mục Customer (cho avatar)
        public Task<string> UploadFileToAvatarAsync(Stream fileStream, string fileName) =>
            UploadFileAsync(fileStream, fileName, "Customer");

        // Xóa file dựa trên URL
        public async Task DeleteFileAsync(string fileUrl)
        {
            var publicId = GetPublicIdFromUrl(fileUrl);
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };
            await _cloudinary.DestroyAsync(deletionParams);
        }

        // Lấy publicId từ URL file
        private string GetPublicIdFromUrl(string fileUrl)
        {
            var uri = new Uri(fileUrl);
            var segments = uri.AbsolutePath.Split('/');
            var fileName = Path.GetFileNameWithoutExtension(segments[^1]);
            var folder = segments[^2];
            return $"{folder}/{fileName}";
        }
    }
}
