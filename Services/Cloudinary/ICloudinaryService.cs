using System.IO;
using System.Threading.Tasks;

namespace KhachSan.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder = "Discount");
        Task<string> UploadFileToRoomServiceAsync(Stream fileStream, string fileName);
        Task<string> UploadFileToRoomAsync(Stream fileStream, string fileName);
        Task<string> UploadFileToAvatarAsync(Stream fileStream, string fileName);
        Task DeleteFileAsync(string fileUrl);
    }
}
