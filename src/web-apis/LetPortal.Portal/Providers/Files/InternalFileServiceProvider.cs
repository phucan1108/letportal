using System.Threading.Tasks;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Services.Files;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Providers.Files
{
    public class InternalFileServiceProvider : IFileSeviceProvider
    {
        private readonly IFileService _fileService;

        public InternalFileServiceProvider(
            IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ResponseDownloadFile> DownloadFileAsync(string fileId)
        {
            return await _fileService.DownloadFileAsync(fileId, false);
        }

        public async Task<ResponseUploadFile> UploadFileAsync(
            string localFilePath,
            string uploader,
            bool allowCompress)
        {
            return await _fileService.UploadFileAsync(localFilePath, uploader, allowCompress);
        }

        public async Task<bool> ValidateFile(IFormFile file)
        {
            return await _fileService.ValidateFile(file);
        }

        public async Task<bool> ValidateFile(string localFilePath)
        {
            return await _fileService.ValidateFile(localFilePath);
        }
    }
}
