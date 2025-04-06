using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using System.Security.Cryptography;
using System.Text;
using static Dropbox.Api.Sharing.FileErrorResult;

namespace KhachSan.Services
{
    public class DropboxService
    {
        private readonly string _accessToken = "sl.u.AFrBQMrgyaaJd_oFiIdj7P4peCXEy4rHagvL_iVfm3hW9jDvh5yPELv3FESzBU_JWDSC_8t50A5BbKpTEoaS8CVtNnfVc-ZtoOi4qT2rIMCc_RcioH6Y7UZFoGUyooU55qQnH-D7ZuEtWROqF9yC7O8amWUqiwoH1naVur_QhrwEYDxhkJ9wZfFtPn5SfqEmzZY8N5QYLehGRnavat33T57wQd31PVOtMD22DPUJassb6STfRrn-1pdrmBltIHFT7IHRsAa4KhDZN31Z_F3FrJ_KC--9pLsu1M2ZQoeTAzFwcfea5CCgkmRh0xlRdNjS8h0Fo2JTZiMaG01DzU2JMQ2a7GXZcRl70XGK8VsOVMJ3DKYKJ8dTQuYm7HctPy2RgcRCX6pP-3qVj5UUAe0znCv9_SP4SOtkkD3yebCoWlR8yvDJHFj4Y6aH8_O_GxzKKDD4hSUgAJOqXYa0e_tEIzCyytQRyxAeCHtG4QExCRFZVypyo_1SFEnHHy95OAZCvPeMCV91ukdf2CkaUwcljWWs0JHHulXXkbRD2HZHxE9mF4cl6_NeKIt07iQc7O7B0XfWGKpW4gLTEurMe1MlUFdnstxFOKy0WMinUAFi4B9ldw8cYLXUNPymeHw5HEjA2EQ5lds4WuQo_oelIOvv5wb7eAjv0KwE-TWbNz4RS7uK9NTIxU8ip5Oc3kdbKvrTdm7Ywd4jCtcWV5WbDJbaDfCbJX_flKsimSPLBXSWuN47n7sUzIOflMfokFYzFF2ubBBso-2NLr0qUv3H60yi2Kn87UokLK-co9cGvYbzjVUEm1LEYvd-6FpGVNZp49vJSjB5kPRP4c6nLjdUET5Nzy1XMxSvEUYTK2UmaIisYeooQKECYmBjZCyJ3Wtrot0GSBfpR8wKJALUMXRjSWbkxvhX7tR2CrF292QsfDOhwitffLXzInUUHydCKFF0ZiFta5e6Vs73i-FQbGnVlDNrP3p6inUDBaP4ZWcVaJdx5N4hn7SIYTDRyBX_pAHsMmSvwjTiDrUGJZ9pip4jIjHNfL8SDosB3cPokvAYHMqb9H4j6Q_khmvKctGC-3W8KPecZrbmXYzbWXxS-NgC8VU9mz4wahK34hPHBap8pT8nysnUp9EADkAXrA9p4XKJcBP3-qVtRFgVV3Lh6V2C5Op8Q-4x9xKuH9_bjWKDVWYaefF3V4jiYRhd1WsNg70l5QArUtEpYOYUOLhWVjBAyzZN80IqIxGoEBlffRCvqW2qdTvhlRig7XLIqE5bdoUl6ISbqMU1myPNRIXzzbUD0Wun5YVmqBhD73k58BymBqd2_mAq-LgYweK9G14g4IotyuKfJztzefxetpNFdOs7ok-Tm8Vsn7S0CeL_TZ-Wfw_6qkUTTEAQ25cLMmMTLQZ4lUQR8AWD1RTCvtxodK3MDOkU8oLI01RF0snW53Hbf0a6-YelGw";
        private readonly string _dropboxFolder = "/ADT07Z2V_Btfu-DiKUDrPRQ/Discount";
        private static readonly SemaphoreSlim _uploadLock = new SemaphoreSlim(1, 1);

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            await _uploadLock.WaitAsync();
            try
            {
                using var dbx = new DropboxClient(_accessToken);
                string hashedFileName = GenerateHashedFileName(fileName);
                string dropboxPath = $"{_dropboxFolder}/{hashedFileName}";

                Console.WriteLine($"Uploading file: {hashedFileName}");
                fileStream.Position = 0;

                try { await dbx.Files.DeleteV2Async(dropboxPath); } catch { }

                await dbx.Files.UploadAsync(dropboxPath, WriteMode.Overwrite.Instance, body: fileStream);

                return await GetOrCreateSharedLinkAsync(dbx, dropboxPath);
            }
            finally
            {
                _uploadLock.Release();
            }
        }

        public async Task<string> UploadFileToRoomServiceAsync(Stream fileStream, string fileName)
        {
            await _uploadLock.WaitAsync();
            try
            {
                using var dbx = new DropboxClient(_accessToken);
                string hashedFileName = GenerateHashedFileName(fileName);
                string dropboxFolderForRoomService = "/ADT07Z2V_Btfu-DiKUDrPRQ/RoomService";  // Thư mục RoomService
                string dropboxPath = $"{dropboxFolderForRoomService}/{hashedFileName}";

                Console.WriteLine($"Uploading file to RoomService: {hashedFileName}"); // Log để kiểm tra số lần gọi
                fileStream.Position = 0;

                // try { await dbx.Files.DeleteV2Async(dropboxPath); } catch { }

                await dbx.Files.UploadAsync(dropboxPath, WriteMode.Overwrite.Instance, body: fileStream);

                return await GetOrCreateSharedLinkAsync(dbx, dropboxPath);
            }
            finally
            {
                _uploadLock.Release();
            }
        }
        public async Task<string> UploadFileToRoomAsync(Stream fileStream, string fileName)
        {
            await _uploadLock.WaitAsync();
            try
            {
                using var dbx = new DropboxClient(_accessToken);
                string hashedFileName = GenerateHashedFileName(fileName);
                string dropboxFolderForRoomService = "/ADT07Z2V_Btfu-DiKUDrPRQ/Room";  // Thư mục RoomService
                string dropboxPath = $"{dropboxFolderForRoomService}/{hashedFileName}";

                Console.WriteLine($"Uploading file to RoomService: {hashedFileName}"); // Log để kiểm tra số lần gọi
                fileStream.Position = 0;

                try { await dbx.Files.DeleteV2Async(dropboxPath); } catch { }

                await dbx.Files.UploadAsync(dropboxPath, WriteMode.Overwrite.Instance, body: fileStream);

                return await GetOrCreateSharedLinkAsync(dbx, dropboxPath);
            }
            finally
            {
                _uploadLock.Release();
            }
        }
        public async Task<string> UploadFileToAvatarAsync(Stream fileStream, string fileName)
        {
            await _uploadLock.WaitAsync();
            try
            {
                using var dbx = new DropboxClient(_accessToken);
                string hashedFileName = GenerateHashedFileName(fileName);
                string dropboxFolderForRoomService = "/ADT07Z2V_Btfu-DiKUDrPRQ/Customer";
                string dropboxPath = $"{dropboxFolderForRoomService}/{hashedFileName}";
                try
                {
                    var metadata = await dbx.Files.GetMetadataAsync(dropboxFolderForRoomService);
                }
                catch (ApiException<FileNotFoundError>)
                {
                    // Nếu thư mục không tồn tại, tạo mới thư mục đó
                    Console.WriteLine($"Folder not found, creating folder: {dropboxFolderForRoomService}");
                    await dbx.Files.CreateFolderV2Async(dropboxFolderForRoomService);
                }
                // Xóa file cũ nếu tồn tại
                try
                {
                    await dbx.Files.DeleteV2Async(dropboxPath);
                }
                catch { }
                // Upload tệp mới
                fileStream.Position = 0;
                await dbx.Files.UploadAsync(dropboxPath, WriteMode.Overwrite.Instance, body: fileStream);

                return await GetOrCreateSharedLinkAsync(dbx, dropboxPath);
            }
            finally
            {
                _uploadLock.Release();
            }
        }

        public async Task DeleteFileAsync(string filePath)
        {
            using var dbx = new DropboxClient(_accessToken);

            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    await dbx.Files.DeleteV2Async(filePath);
                }
                else
                {
                    throw new Exception("Invalid file path.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file from Dropbox: {ex.Message}");
            }
        }

        // Hàm mã hóa tên file bằng SHA256
        private string GenerateHashedFileName(string fileName)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fileName));
                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                string extension = Path.GetExtension(fileName);
                return $"{hash}{extension}";
            }
        }


        // Đảm bảo thư mục tồn tại trên Dropbox
        private async Task EnsureFolderExists(DropboxClient dbx, string folderPath)
        {
            try
            {
                await dbx.Files.GetMetadataAsync(folderPath);
            }
            catch (ApiException<GetMetadataError> ex) when (ex.ErrorResponse.IsPath)
            {
                await dbx.Files.CreateFolderV2Async(folderPath);
            }
        }

        // Tạo hoặc lấy link chia sẻ của file
        private async Task<string> GetOrCreateSharedLinkAsync(DropboxClient dbx, string dropboxPath)
        {
            try
            {
                return await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxPath)
                    .ContinueWith(task => task.Result.Url.Replace("?dl=0", "?raw=1"));
            }
            catch (ApiException<CreateSharedLinkWithSettingsError> ex) when (ex.ErrorResponse.IsSharedLinkAlreadyExists)
            {
                var existingLinks = await dbx.Sharing.ListSharedLinksAsync(dropboxPath);
                return existingLinks.Links.First().Url.Replace("?dl=0", "?raw=1");
            }
        }

        public async Task<string> GetDropboxPathFromUrl(string fileUrl)
        {
            using var dbx = new DropboxClient(_accessToken);
            var links = await dbx.Sharing.ListSharedLinksAsync();

            var link = links.Links.FirstOrDefault(l => l.Url.Equals(fileUrl, StringComparison.OrdinalIgnoreCase));
            return link?.PathLower ?? string.Empty;
        }


    }
}
