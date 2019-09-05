using LetPortal.Core.Files;
using LetPortal.Portal.Exceptions.Files;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

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
            if(file.Length > _fileValidatorOptions.CurrentValue.MaximumFileSize)
            {
                throw new FileException(FileErrorCodes.ReachMaximumFile);
            }
            return Task.CompletedTask;
        }
    }
}
