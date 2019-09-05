using LetPortal.Core.Files;
using LetPortal.Core.Utils;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Repositories.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        private readonly IOptionsMonitor<Options.Files.FileOptions> _fileOptions;

        private readonly IEnumerable<IFileValidatorRule> _fileValidatorRules;

        private readonly IEnumerable<IFileConnectorExecution> _fileConnectorExecutions;

        public FileService(
            IOptionsMonitor<Options.Files.FileOptions> fileOptions,
            IEnumerable<IFileConnectorExecution> fileConnectorExecutions,
            IEnumerable<IFileValidatorRule> fileValidatorRules,
            IFileRepository fileRepository)
        {
            _fileOptions = fileOptions;
            _fileValidatorRules = fileValidatorRules;
            _fileConnectorExecutions = fileConnectorExecutions;
            _fileRepository = fileRepository;
        }

        public async Task<ResponseDownloadFile> DownloadFileAsync(string fileId)
        {
            var file = await _fileRepository.GetOneAsync(fileId);

            var fileConnector = _fileConnectorExecutions.First(a => a.FileStorageType == file.FileStorageType);

            return new ResponseDownloadFile
            {
                FileName = file.Name,
                MIMEType = file.MIMEType,
                FileBytes = await fileConnector.GetFileAsync(new StoredFile { FileIdentifierOptions = file.IdentifierOptions })
            };
        }

        public async Task<Entities.Files.File> GetFileInfo(string fileId)
        {
            return await _fileRepository.GetOneAsync(fileId);
        }

        public Task<string> GetFileMIMEType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if(!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return Task.FromResult(contentType);
        }

        public async Task<ResponseUploadFile> UploadFileAsync(IFormFile file, string uploader)
        {
            // Store a file into temp disk before proceeding
            var localFilePath = await SaveFormFileAsync(file);

            // 1. Check all rules
            foreach(var rule in _fileValidatorRules)
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
                DownloadableUrl = storedFile.UseServerHost 
                    ? _fileOptions.CurrentValue.DownloadableHost + "/" + createdId 
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

        private async Task<string> SaveFormFileAsync(IFormFile file)
        {
            var tempFileName = Path.GetRandomFileName();

            tempFileName = tempFileName.Split(".")[0] + "." + file.FileName.Split(".")[1];

            using(var stream = new FileStream(tempFileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return tempFileName;
        }
    }
}
