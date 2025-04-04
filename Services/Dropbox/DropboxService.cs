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
        private readonly string _accessToken = "sl.u.AFqEbxfZFXoFVSBvexdhQc89tVrFe216l-cC6pFePvCcEH2CdDJoFc5RMSdtZJZfzC0M9W2iru2isVR1OifUCwPUsLMJQkmkBR4bJ1onDV5PUJK9DqnUWxaSh-cVC5HB85yw37TWzkhK9QVRkK3MUnSv7xo-42wDvxiLFlYybhjmJnbhhdC1vF1GexDgtx_33rsS1AoF5XrC50Owe0aSbV_0QqEf29OOwuBMiLyMuTKxrAk9VvqSmKvC4ntOJ3v9Fojd-UTeuQ711yyUHOCgDVWT8qmykj90XjgIsA9cWSAaPzjiL3IyyGdU1X0H8whpfYOoEB00OjeizFRtiWBP4nmxOuFcKdUp5GggDlDvx6LlVCw_NLq4Aaoba5uwaJVtp2e-xqQb6u7eBPbN-xP_g1IhIDPlGpBMY-eSxy3ZCB2uCAeRKGTH1W8HP5gJ6OJRccaqtlycb2uKKjPBENJsVrjqZJyEya4T7zkjvZyBBKYZgdUHn1omqrRYqljzWAS6VjJo_3fLXiyRtlOQ5Q9rLnknDtlVv0DZLN9WLx03sdMe0k7BfnqyRe4HoX7cZTQ9pmqhF1yjMY0s2dkWHKFlG_AU13d7q3w60g0oJEfQf0vMkcMYhJW8bisW3IpDQeZNt1Kn8o6gUTTFh_QKx6CHd8ZbhzXHEJEQeTsOuJWUX07PWYlfpkn4cHSbYspaiyHPMmCO4n_dECeBkqNI94AUoAHWvLUNUnYv0aqP7y3NHbtzIO8A5H-tG9sxtv2CMYv4sNCN9TI8HjPuqVBOEThN84iJalzVb57hwF4jLrMHAZCP4SKb4qUfc8pQTujrVJVQs-7GebBzsXk95SHeeRDbvR-Pl_zDvB77wZeoug6A-ENv9SmCT1hh4n_G8ZCq5-VKnHnGkEdqQ1COeYgnh9q_0FOtY8v-dT8hlb8mCQtjNYYcX9oJ-8nWRlsbxWmiiTa0wtUwbS66tO1Hcm7NiLY41v-tySKW8Nz4reEuRbRgpvSZ10DgypXYWD-ZZFU8v5TmmgU1z5rUKA2ok3mPBQJwKv7NQjYnv4X7d0S1bWmDD3K0qQ9xI7HhBck1wXvQKzKSwtBrckwTYBzMjIg71FRJUAiCK-Nux4lHWaojSJT8LmHoL8cKpmczSnpkSNMx8tyKR_anuBcBBzUeUhIsVZ3_BsBT9C0XRJ-lqmw2reHuHkgBPgB7T6Rz4lhJAuew6gthWeGpAuXj_ICiZ2lubOGT_TsEu2XYhH-d9bM7IOpk_SUABLgdBh7jvWyvu5mL9P1K9Osr738TpjqsPQx3ZVn9JoPPpyIftJ1Zrkl9SJ2B30PQ9YxXgb6NaxtecdTQi60q3byFt2EQGaEfvm3lb0aaTedFtfaaitMLqVnz0d4UWC1ktYUb5qOw9hh-26f7GP95r-k4Ygcs3-h-7FIfQLztMpmdUG7Yt2Dfi_JFUtVzB7z4Ag";
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
