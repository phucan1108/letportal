using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Services.Files;
using System.Threading.Tasks;

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

        public async Task<ResponseUploadFile> UploadFileAsync(
            string localFilePath,
            string uploader)
        {
            return await _fileService.UploadFileAsync(localFilePath, uploader);
        }
    }
}
