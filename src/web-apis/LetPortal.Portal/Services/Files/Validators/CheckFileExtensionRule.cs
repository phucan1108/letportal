using LetPortal.Core.Files;
using LetPortal.Portal.Exceptions.Files;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            if(_fileValidatorOptions.CurrentValue.CheckFileExtension)
            {
                // Read 32 bytes for checking signature
                string firstBytes = string.Empty;
                using(var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using(var reader = new BinaryReader(fileStream))
                    {
                        reader.BaseStream.Position = 0;
                        byte[] data = reader.ReadBytes(32);
                        firstBytes = BitConverter.ToString(data);
                        reader.Close();
                    }
                }
                // Get correct magic number by ext
                var ext = fileName.Split(".")[1];
                var magicNumbers = _fileValidatorOptions
                                    .CurrentValue
                                    .ExtensionMagicNumbers
                                    .First(a => a.Key == ext.ToLower())
                                    .Value;

                //  Validate extension
                if(firstBytes.IndexOf(magicNumbers) != 0)
                {
                    throw new FileException(FileErrorCodes.WrongFileExtension);
                }
            }
        }
    }
}
