using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
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

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class FileServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

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
                WhiteLists = "jpg;jpeg;png;zip;json",
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

        public FileServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        #region UTs for Mongo
        [Fact]
        public async Task Upload_File_And_Save_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileMongoRepository fileRepository = new FileMongoRepository(new Core.Persistences.MongoConnection(
                    databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MongoStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            fileRepository.Dispose();
            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }

        [Fact]
        public async Task Upload_Then_Download_File_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileMongoRepository fileRepository = new FileMongoRepository(new Core.Persistences.MongoConnection(
                    databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MongoStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            LetPortal.Portal.Models.Files.ResponseDownloadFile response = await fileService.DownloadFileAsync(result.FileId, false);
            fileRepository.Dispose();
            // 3. Assert
            Assert.NotNull(response.FileBytes);
        }
        #endregion

        #region UTs for Postgre
        [Fact]
        public async Task Upload_File_And_Save_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.PostgreSqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.PostgreSQL,
                Datasource = _context.PostgreSqlDatabaseConnection.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new PostgreStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock, 
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            fileRepository.Dispose();
            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }

        [Fact]
        public async Task Upload_Then_Download_File_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.PostgreSqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.PostgreSQL,
                Datasource = _context.PostgreSqlDatabaseConnection.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new PostgreStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock, 
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            LetPortal.Portal.Models.Files.ResponseDownloadFile response = await fileService.DownloadFileAsync(result.FileId, false);
            fileRepository.Dispose();
            // 3. Assert
            Assert.NotNull(response.FileBytes);
        }
        #endregion

        #region UTs for Sql Server
        [Fact]
        public async Task Upload_File_And_Save_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.SqlServerDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.SQLServer,
                Datasource = _context.SqlServerDatabaseConnection.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new SqlServerStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            fileRepository.Dispose();
            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }

        [Fact]
        public async Task Upload_Then_Download_File_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.SqlServerDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.SQLServer,
                Datasource = _context.SqlServerDatabaseConnection.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new SqlServerStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            LetPortal.Portal.Models.Files.ResponseDownloadFile response = await fileService.DownloadFileAsync(result.FileId, false);
            fileRepository.Dispose();
            // 3. Assert
            Assert.NotNull(response.FileBytes);
        }
        #endregion

        #region UTs for MySQL
        [Fact]
        public async Task Upload_File_And_Save_My_Sql_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }

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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MySqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MySQL,
                Datasource = _context.MySqlDatabaseConnection.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MySqlStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            fileRepository.Dispose();
            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }

        [Fact]
        public async Task Upload_Then_Download_File_My_Sql_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
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

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MySqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MySQL,
                Datasource = _context.MySqlDatabaseConnection.DataSource
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            List<IStoreFileDatabase> storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MySqlStoreFileDatabase()
            };
            DatabaseFileConnectorExecution databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", true);
            memoryStream.Close();
            LetPortal.Portal.Models.Files.ResponseDownloadFile response = await fileService.DownloadFileAsync(result.FileId, true);
            fileRepository.Dispose();
            // 3. Assert
            Assert.NotNull(response.FileBytes);
        }
        #endregion

        #region UTs for Local Disk
        [Fact]
        public async Task Upload_File_And_Save_Local_Disk_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }

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

            DiskStorageOptions diskOptions = new DiskStorageOptions
            {
                AllowDayFolder = true,
                Path = "~/UploadFiles"
            };

            DatabaseOptions databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MySqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MySQL,
                Datasource = _context.MySqlDatabaseConnection.DataSource
            };

            LetPortal.Portal.Options.Files.FileOptions localFileOptions = FileOptions;
            localFileOptions.DiskStorageOptions = diskOptions;
            localFileOptions.FileStorageType = FileStorageType.Disk;

#pragma warning disable CA2000 // Dispose objects before losing scope
            FileEFRepository fileRepository = new FileEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            IOptionsMonitor<FilePublishOptions> filePublishOptionsMock = Mock.Of<IOptionsMonitor<FilePublishOptions>>(_ => _.CurrentValue == FilePublishOptions);
            IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions> fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == localFileOptions);
            IOptionsMonitor<FileValidatorOptions> fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == localFileOptions.ValidatorOptions);
            IOptionsMonitor<DiskStorageOptions> diskStorageOptionsMock = Mock.Of<IOptionsMonitor<DiskStorageOptions>>(_ => _.CurrentValue == diskOptions);
            CheckFileExtensionRule checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            CheckFileSizeRule checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            DiskFileConnectorExecution diskStorage = new DiskFileConnectorExecution(diskStorageOptionsMock);

            FileService fileService = new FileService(
                fileOptionsMock,
                filePublishOptionsMock,
                new List<IFileConnectorExecution>
            {
               diskStorage
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            LetPortal.Portal.Models.Files.ResponseUploadFile result = await fileService.UploadFileAsync(mockFile.Object, "tester", false);
            memoryStream.Close();
            fileRepository.Dispose();
            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }
        #endregion
    }
}
