using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Options.Files;
using LetPortal.Portal.Repositories.Files;
using LetPortal.Portal.Services.Files;
using LetPortal.Portal.Services.Files.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.UTs.Portal.Services
{
    public class FileServiceTests
    {
        public FilePublishOptions FilePublishOptions = new FilePublishOptions
        {
            DownloadableHost = "http://localhost:53595/v1.0/files"
        };

        public LetPortal.Portal.Options.Files.FileOptions FileOptions = new LetPortal.Portal.Options.Files.FileOptions
        {
            FileStorageType = FileStorageType.Database,            
            ValidatorOptions = new FileValidatorOptions
            {
                CheckFileExtension = true,
                MaximumFileSize = 16777216,
                ExtensionMagicNumbers = new Dictionary<string, string>
                    {
                        { "jpg", "FF-D8" }
                    }
            },
            DatabaseStorageOptions = new DatabaseStorageOptions
            {
                SameAsPortal = true
            }
        };

        [Fact]
        public async Task Upload_File_Test()
        {
            // 1. Arrange
            Mock<IFormFile> mockFile = new Mock<IFormFile>();
            FileStream sourceImg = System.IO.File.OpenRead(@"Artifacts\connect-iot-to-internet.jpg");
            MemoryStream memoryStream = new MemoryStream();
            await sourceImg.CopyToAsync(memoryStream);
            sourceImg.Close();
            memoryStream.Position = 0;
            string fileName = "connect-iot-to-internet.jpg";
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length).Verifiable();
            mockFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream).Verifiable();
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream))
                .Verifiable();
            Mock<IFileRepository> mockFileRepository = new Mock<IFileRepository>();
            mockFileRepository
                .Setup(a => a.AddAsync(It.IsAny<LetPortal.Portal.Entities.Files.File>()))
                .Returns(Task.CompletedTask);

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = "mongodb://localhost:27017",
                ConnectionType = Core.Persistences.ConnectionType.MongoDB,
                Datasource = "letportal"
            };
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            Mock<IFileConnectorExecution> mockDatabaseFileConnectorExecution = new Mock<IFileConnectorExecution>();
            mockDatabaseFileConnectorExecution
                .Setup(a => a.FileStorageType)
                .Returns(FileStorageType.Database);

            mockDatabaseFileConnectorExecution
                .Setup(a => a.StoreFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new StoredFile
                {
                    DownloadableUrl = "localhost",
                    FileIdentifierOptions = "test",
                    UseServerHost = true
                }));

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               mockDatabaseFileConnectorExecution.Object
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, mockFileRepository.Object);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();

            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }
    }
}
