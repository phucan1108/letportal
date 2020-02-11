using System.IO;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Portal.Exceptions.Files;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Portal.Services.Files.Validators
{
    public class CheckFileSizeRule : IFileValidatorRule
    {
        private readonly IOptionsMonitor<FileValidatorOptions> _fileValidatorOptions;

        public CheckFileSizeRule(IOptionsMonitor<FileValidatorOptions> fileValidatorOptions)
        {
            _fileValidatorOptions = fileValidatorOptions;
        }

        public Task Validate(IFormFile file, string tempFilePath)
        {
            if (file.Length > _fileValidatorOptions.CurrentValue.MaximumFileSize)
            {
                throw new FileException(FileErrorCodes.ReachMaximumFile);
            }
            return Task.CompletedTask;
        }

        public Task Validate(string filePath)
        {
            var file = new FileInfo(filePath);
            if (file.Length > _fileValidatorOptions.CurrentValue.MaximumFileSize)
            {
                throw new FileException(FileErrorCodes.ReachMaximumFile);
            }
            return Task.CompletedTask;
        }
    }
}
