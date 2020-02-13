using System;
using System.IO;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Utils;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Portal.Services.Files
{
    public class DiskFileConnectorExecution : IFileConnectorExecution
    {
        private readonly IOptionsMonitor<DiskStorageOptions> _diskStorageOptions;

        public FileStorageType FileStorageType => FileStorageType.Disk;

        public DiskFileConnectorExecution(IOptionsMonitor<DiskStorageOptions> diskStorageOptions)
        {
            _diskStorageOptions = diskStorageOptions;
        }

        public async Task<byte[]> GetFileAsync(StoredFile storedFile)
        {
            var localFile = ConvertUtil.DeserializeObject<LocalFileOptions>(storedFile.FileIdentifierOptions);
            return await File.ReadAllBytesAsync(localFile.FileLocation);
        }

        public Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath)
        {
            return Task.FromResult(StoreFileDisk(file.FileName, tempFilePath));
        }

        public Task<StoredFile> StoreFileAsync(string localFilePath)
        {
            return Task.FromResult(StoreFileDisk(Path.GetFileName(localFilePath), localFilePath));
        }

        private StoredFile StoreFileDisk(string fileName, string localFilePath)
        {
            var savingFilePath =
                !string.IsNullOrEmpty(_diskStorageOptions.CurrentValue.Path) ?
                    _diskStorageOptions.CurrentValue.Path : Directory.GetCurrentDirectory();

            if (savingFilePath.StartsWith("~"))
            {
                savingFilePath = savingFilePath.Replace("~", Directory.GetCurrentDirectory());
            }
            if (_diskStorageOptions.CurrentValue.AllowDayFolder)
            {
                savingFilePath += "\\" + DateTime.UtcNow.ToString("yyyyMMdd");
            }

            Directory.CreateDirectory(savingFilePath);
            savingFilePath += "\\" + fileName;
            File.Copy(localFilePath, savingFilePath, true);

            return
                new StoredFile
                {
                    FileIdentifierOptions = ConvertUtil.SerializeObject(new LocalFileOptions
                    {
                        FileLocation = savingFilePath
                    })
                };
        }

        class LocalFileOptions
        {
            public string FileLocation { get; set; }
        }
    }
}
