using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Portal.Exceptions.Files;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Portal.Services.Files.Validators
{
    public class CheckAllowedExtensionFileRule : IFileValidatorRule
    {
        private readonly IOptionsMonitor<FileValidatorOptions> _fileValidatorOptions;

        public CheckAllowedExtensionFileRule(IOptionsMonitor<FileValidatorOptions> fileValidatorOptions)
        {
            _fileValidatorOptions = fileValidatorOptions;
        }

        public Task Validate(IFormFile file, string tempFilePath)
        {
            CheckAllowedFileExtensions(file.FileName);
            return Task.CompletedTask;
        }

        public Task Validate(string filePath)
        {
            CheckAllowedFileExtensions(Path.GetFileName(filePath));
            return Task.CompletedTask;
        }

        private void CheckAllowedFileExtensions(string fileName)
        {
            var allowFiles = _fileValidatorOptions.CurrentValue.WhiteLists.Split(";");
            var fileSplitted = fileName.Split(".");
            if (!allowFiles.Any(a => a == fileSplitted[^1].ToLower()))
            {
                throw new FileException(FileErrorCodes.NotAllowedFileExtension);
            }
        }
    }
}
