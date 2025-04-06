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
        private readonly string _accessToken = "sl.u.AFqvwQ5ZrHKzW222XXYHh0pN-FlEeoBTgdEVsKNm1EF3i5gsZ__xlAFTy_o9NhUtdI0yoFGTVMNjPR-Sifr84wIs7yLiIWUtg1WWgH9la-P11uZdakbHnER8d9eTJrBmS0BdggI1DaLOmi6_X1bZSgbl-Tki6tqU52wCP2KKqelptXWnOf88ZdMrZ0F5AvtXwm5moL4Gv6SrJkD4Au6CNWS0LALBZoakZNZoSSu9bPYSuzXdiwePDJKM9Ic4_vE2tU8y2PywYwwo0rvfYnZwUvFHC2VFP9pwOvjAijI7hM-xdUNK3YZhRCsUdmOAOkQXAc6y4PG1TPjp4RkZkjwcwCBpliqqTh1O7z-5VVYbWljqGq3xWl5pitrak1_i3OEHjy1ArtakVwr7uPmgJlg7Im0HBChPChdeBL1FHp9X9JlQdvMdOpuf23UVK32esnoM02GzI19o-Hry_jzO6i2blZbuCmWiSjgzN2j_gdc8ybUNTYD_G4doKfzvz4PjIl5hEXIIuYy2FSzbjmJ04pd2uhvHuWQegd5OK7wyoZ46QcK6SWANSykeCeWOtzkOqFnd5XoUznHly0NE-p153nf9Qp81KH9rMIdYbq9Sqmn2H8foWXAeop6GjIK0kygENZqy1ZseIkI3o6Rb2z0C3guvGsR7UeDuTjzajledguCZ4o7KNw_W2Ox1xc8rPRpl2Jaa1REn8OUs1PhHtIzgdzQehPK8Aaq0JpABrtBP_BCNnK_2gkw1ryGZpYVCyMlWok4n0rzK3ZUay8Sqxth5NKKNMyQNWXhlyoChvOCk5lT1laUFL6Mq_RwMC_YnML3sL_YL9e40aeEaMQHwFgbqkC5VsBjisQSTNjWRJWwTnm4zms_BP5lAy-_nVDsdxns3QSPtjQmu1FNb2Q4p9v5DwDC7F-_6QB5PNLKRowArVByzSLYkbSiDF4rl9QS8LbSveCCPcRVoxsJNxOTzQybZLtFnl8bF6el-X5Z4QZsBkBnf4kps_nX8GqkJ3K70tEioseZHPqqmePF-R-CVUsXNGxna4WwN-Z-e7avQ11vlBRTWqlTqPONZEaignLj5joN7UByzMbme1__LKBPveiI5YqPw3B6oi56KLjM9Es-JlhBJM4552bmrZ3RWDJptyPRWvDNQj8y8WPWk_HLclVFrH6payWQlwMW9CEt1uYHRYlus9MJO8F6Q5XKpsKLq3YrPZ3lLJ2GMu5gascRD060VcTGinc54E5NFFqg9VEHaKqXTNCEiPXHbZFEAIKgxddIBjYX1RLW8LFGNycfccynxJ0cq9nAVBnplwbycS7AyvrYhwMFoB8AMfaEqrQQ8kfPO0SQkCn4qgnK2JgOnjvMrZhCnQCxbnkGiYaVXKG1KsltwnYf4zqf_YUiu-0rY_DwjZS9l6l69VbNf7sjElb3PcxxcG9AGJC6fRH0LlyHo_ABFKHUzig";
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
