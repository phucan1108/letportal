using System.Threading.Tasks;
using LetPortal.Portal.Entities.Files;
using LetPortal.Portal.Models.Files;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Services.Files
{
    public interface IFileService
    {
        Task<File> GetFileInfo(string fileId);

        Task<string> GetFileMIMEType(string fileName);

        Task<bool> ValidateFile(IFormFile file);

        Task<bool> ValidateFile(string localFilePath);

        Task<ResponseUploadFile> UploadFileAsync(IFormFile file, string uploader, bool allowCompress);

        Task<ResponseUploadFile> UploadFileAsync(string localFilePath, string uploader, bool allowCompress);

        Task<ResponseDownloadFile> DownloadFileAsync(string fileId, bool wantCompress);
    }
}
