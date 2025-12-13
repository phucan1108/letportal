using System.Threading.Tasks;
using LetPortal.Portal.Models.Files;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Providers.Files
{
    public interface IFileSeviceProvider
    {
        Task<ResponseUploadFile> UploadFileAsync(string localFilePath, string uploader, bool allowCompress);

        Task<bool> ValidateFile(IFormFile file);

        Task<bool> ValidateFile(string localFilePath);

        Task<ResponseDownloadFile> DownloadFileAsync(string fileId);
    }
}
