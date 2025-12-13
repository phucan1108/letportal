using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Utils;
using LetPortal.Portal.Exceptions.Files;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Portal.Services.Files.Validators
{
    public class CheckFileExtensionRule : IFileValidatorRule
    {
        private readonly IOptionsMonitor<FileValidatorOptions> _fileValidatorOptions;

        public CheckFileExtensionRule(IOptionsMonitor<FileValidatorOptions> fileValidatorOptions)
        {
            _fileValidatorOptions = fileValidatorOptions;
        }

        public Task Validate(IFormFile file, string tempFilePath)
        {
            CheckFileExt(Path.GetFileName(file.FileName), tempFilePath);

            return Task.CompletedTask;
        }

        public Task Validate(string filePath)
        {
            CheckFileExt(Path.GetFileName(filePath), filePath);

            return Task.CompletedTask;
        }

        private void CheckFileExt(string fileName, string filePath)
        {
            if (_fileValidatorOptions.CurrentValue.CheckFileExtension)
            {
                var isValid = false;
                var extFile = FileUtil.GetExtension(fileName);
                if (_fileValidatorOptions.CurrentValue.ExtensionMagicNumbers.ContainsKey(extFile))
                {
                    // Get correct magic number by ext
                    var magicNumbers = _fileValidatorOptions
                                        .CurrentValue
                                        .ExtensionMagicNumbers
                                        .First(a => a.Key == extFile)
                                        .Value;
                    if (!string.IsNullOrEmpty(magicNumbers))
                    {
                        // Read 32 bytes for checking signature
                        var firstBytes = string.Empty;
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            using (var reader = new BinaryReader(fileStream))
                            {
                                reader.BaseStream.Position = 0;
                                var data = reader.ReadBytes(32);
                                firstBytes = BitConverter.ToString(data);
                                reader.Close();
                            }
                        }

                        //  Validate extension
                        isValid = firstBytes.IndexOf(magicNumbers) == 0;
                    }
                    else
                    {
                        isValid = true;
                    }
                }

                if (!isValid)
                {
                    throw new FileException(FileErrorCodes.WrongFileExtension);
                }
            }
        }
    }
}
