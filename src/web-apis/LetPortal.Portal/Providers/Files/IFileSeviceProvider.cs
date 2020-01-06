using LetPortal.Portal.Models.Files;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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
