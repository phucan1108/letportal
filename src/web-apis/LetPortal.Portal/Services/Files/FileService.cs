using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Utils;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Options.Files;
using LetPortal.Portal.Repositories.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace LetPortal.Portal.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        private readonly IOptionsMonitor<Options.Files.FileOptions> _fileOptions;

        private readonly IOptionsMonitor<FilePublishOptions> _filePublishOptions;

        private readonly IEnumerable<IFileValidatorRule> _fileValidatorRules;

        private readonly IEnumerable<IFileConnectorExecution> _fileConnectorExecutions;

        public FileService(
            IOptionsMonitor<Options.Files.FileOptions> fileOptions,
            IOptionsMonitor<FilePublishOptions> filePublishOptions,
            IEnumerable<IFileConnectorExecution> fileConnectorExecutions,
            IEnumerable<IFileValidatorRule> fileValidatorRules,
            IFileRepository fileRepository)
        {
            _fileOptions = fileOptions;
            _filePublishOptions = filePublishOptions;
            _fileValidatorRules = fileValidatorRules;
            _fileConnectorExecutions = fileConnectorExecutions;
            _fileRepository = fileRepository;
        }

        public async Task<ResponseDownloadFile> DownloadFileAsync(string fileId, bool wantCompress)
        {
            var file = await _fileRepository.GetOneAsync(fileId);

            var fileConnector = _fileConnectorExecutions.First(a => a.FileStorageType == file.FileStorageType);

            var fileBytes = await fileConnector.GetFileAsync(new StoredFile { FileIdentifierOptions = file.IdentifierOptions });
            byte[] returnedBytes;
            if (wantCompress && file.AllowCompress)
            {
                using (var ms = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Fastest);
                        using (var zipStream = zipEntry.Open())
                        {
                            zipStream.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                    returnedBytes = ms.ToArray();
                    file.Name = FileUtil.GetFileNameWithoutExt(file.Name) + ".zip";
                    file.MIMEType = "application/zip";
                    GC.SuppressFinalize(fileBytes);
                }
            }
            else
            {
                returnedBytes = fileBytes;
            }
            return new ResponseDownloadFile
            {
                FileName = file.Name,
                MIMEType = file.MIMEType,
                FileBytes = returnedBytes
            };
        }

        public async Task<Entities.Files.File> GetFileInfo(string fileId)
        {
            return await _fileRepository.GetOneAsync(fileId);
        }

        public Task<string> GetFileMIMEType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return Task.FromResult(contentType);
        }

        public async Task<ResponseUploadFile> UploadFileAsync(IFormFile file, string uploader, bool allowCompress)
        {
            string localFilePath = string.Empty;
            try
            {
                // Store a file into temp disk before proceeding
                localFilePath = await SaveFormFileAsync(file);

                // 1. Check all rules
                foreach (var rule in _fileValidatorRules)
                {
                    await rule.Validate(file, localFilePath);
                }

                // 2. Call Media Connector to upload
                var storedFile = await _fileConnectorExecutions.First(a => a.FileStorageType == _fileOptions.CurrentValue.FileStorageType).StoreFileAsync(file, tempFilePath: localFilePath);

                // 3. Store its into database
                var createdId = DataUtil.GenerateUniqueId();
                var createFile = new Entities.Files.File
                {
                    Id = createdId,
                    Name = file.FileName,
                    DateUploaded = DateTime.UtcNow,
                    Uploader = uploader,
                    MIMEType = await GetFileMIMEType(localFilePath),
                    FileSize = file.Length,
                    FileStorageType = _fileOptions.CurrentValue.FileStorageType,
                    IdentifierOptions = storedFile.FileIdentifierOptions,
                    AllowCompress = allowCompress,
                    DownloadableUrl = storedFile.UseServerHost
                        ? _filePublishOptions.CurrentValue.DownloadableHost + "/" + createdId
                            : storedFile.DownloadableUrl
                };

                await _fileRepository.AddAsync(createFile);

                System.IO.File.Delete(localFilePath);
                return new ResponseUploadFile
                {
                    FileId = createFile.Id,
                    DownloadableUrl = createFile.DownloadableUrl
                };
            }
            finally
            {
                if (!string.IsNullOrEmpty(localFilePath))
                {
                    System.IO.File.Delete(localFilePath);
                }                                                        
            }               
        }

        public async Task<ResponseUploadFile> UploadFileAsync(string localFilePath, string uploader, bool allowCompress)
        {
            // 1. Check all rules
            foreach (var rule in _fileValidatorRules)
            {
                await rule.Validate(localFilePath);
            }

            // 2. Call Media Connector to upload
            var storedFile = await
                _fileConnectorExecutions
                    .First(a => a.FileStorageType == _fileOptions.CurrentValue.FileStorageType)
                    .StoreFileAsync(localFilePath);

            // 3. Store its into database
            var createdId = DataUtil.GenerateUniqueId();
            var file = new FileInfo(localFilePath);
            var createFile = new Entities.Files.File
            {
                Id = createdId,
                Name = file.Name,
                DateUploaded = DateTime.UtcNow,
                Uploader = uploader,
                MIMEType = await GetFileMIMEType(localFilePath),
                FileSize = file.Length,
                FileStorageType = _fileOptions.CurrentValue.FileStorageType,
                IdentifierOptions = storedFile.FileIdentifierOptions,
                AllowCompress = allowCompress,
                DownloadableUrl = storedFile.UseServerHost
                    ? _filePublishOptions.CurrentValue.DownloadableHost + "/" + createdId
                        : storedFile.DownloadableUrl
            };

            await _fileRepository.AddAsync(createFile);

            System.IO.File.Delete(localFilePath);
            return new ResponseUploadFile
            {
                FileId = createFile.Id,
                DownloadableUrl = createFile.DownloadableUrl
            };
        }

        public async Task<bool> ValidateFile(IFormFile file)
        {
            try
            {
                var localFilePath = await SaveFormFileAsync(file);
                foreach (var rule in _fileValidatorRules)
                {
                    await rule.Validate(file, localFilePath);
                }
                System.IO.File.Delete(localFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateFile(string localFilePath)
        {
            try
            {
                foreach (var rule in _fileValidatorRules)
                {
                    await rule.Validate(localFilePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> SaveFormFileAsync(IFormFile file)
        {
            var tempFileName = Path.GetRandomFileName();
            tempFileName = FileUtil.GetFileNameWithoutExt(tempFileName) + "." + FileUtil.GetExtension(file.FileName);

            using (var stream = new FileStream(tempFileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return tempFileName;
        }
    }
}
