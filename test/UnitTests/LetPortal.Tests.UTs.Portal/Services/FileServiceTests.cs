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
        public LetPortal.Portal.Options.Files.FileOptions FileOptions = new LetPortal.Portal.Options.Files.FileOptions
        {
            FileStorageType = FileStorageType.Database,
            DownloadableHost = "http://localhost:53595/v1.0/files",
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
            var mockFile = new Mock<IFormFile>();
            var sourceImg = System.IO.File.OpenRead(@"Artifacts\connect-iot-to-internet.jpg");
            var memoryStream = new MemoryStream();
            await sourceImg.CopyToAsync(memoryStream);
            sourceImg.Close();
            memoryStream.Position = 0;
            var fileName = "connect-iot-to-internet.jpg";
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length).Verifiable();
            mockFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream).Verifiable();
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream))
                .Verifiable();
            var mockFileRepository = new Mock<IFileRepository>();
            mockFileRepository
                .Setup(a => a.AddAsync(It.IsAny<LetPortal.Portal.Entities.Files.File>()))
                .Returns(Task.CompletedTask);

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = "mongodb://localhost:27017",
                ConnectionType = Core.Persistences.ConnectionType.MongoDB,
                Datasource = "letportal"
            };
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var mockDatabaseFileConnectorExecution = new Mock<IFileConnectorExecution>();
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

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               mockDatabaseFileConnectorExecution.Object
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, mockFileRepository.Object);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();

            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }
    }
}
