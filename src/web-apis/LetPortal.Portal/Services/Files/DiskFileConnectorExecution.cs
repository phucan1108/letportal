using LetPortal.Core.Files;
using LetPortal.Core.Files;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

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

        public Task<byte[]> GetFileAsync(StoredFile storedFile)
        {
            throw new NotImplementedException();
        }

        public Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
