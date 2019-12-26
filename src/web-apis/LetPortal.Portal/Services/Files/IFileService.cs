using LetPortal.Portal.Entities.Files;
using LetPortal.Portal.Models.Files;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Files
{
    public interface IFileService
    {
        Task<File> GetFileInfo(string fileId);

        Task<string> GetFileMIMEType(string fileName);

        Task<ResponseUploadFile> UploadFileAsync(IFormFile file, string uploader);

        Task<ResponseUploadFile> UploadFileAsync(string localFilePath, string uploader);

        Task<ResponseDownloadFile> DownloadFileAsync(string fileId);
    }
}
