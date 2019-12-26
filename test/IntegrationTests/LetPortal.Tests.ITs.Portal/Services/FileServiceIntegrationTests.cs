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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };

            var fileRepository = new FileMongoRepository(new Core.Persistences.MongoConnection(
                    databaseOptions));
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MongoStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock, 
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };

            var fileRepository = new FileMongoRepository(new Core.Persistences.MongoConnection(
                    databaseOptions));
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MongoStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock, 
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close(); 
            var response = await fileService.DownloadFileAsync(result.FileId);
            // 3. Assert

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.PostgreSqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.PostgreSQL,
                Datasource = _context.PostgreSqlDatabaseConnection.DataSource
            };

            var fileRepository = new FileEFRepository(_context.GetPostgreSQLContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new PostgreStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.PostgreSqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.PostgreSQL,
                Datasource = _context.PostgreSqlDatabaseConnection.DataSource
            };

            var fileRepository = new FileEFRepository(_context.GetPostgreSQLContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new PostgreStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();
            var response = await fileService.DownloadFileAsync(result.FileId);
            // 3. Assert

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.SqlServerDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.SQLServer,
                Datasource = _context.SqlServerDatabaseConnection.DataSource
            };

            var fileRepository = new FileEFRepository(_context.GetSQLServerContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new SqlServerStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.SqlServerDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.SQLServer,
                Datasource = _context.SqlServerDatabaseConnection.DataSource
            };

            var fileRepository = new FileEFRepository(_context.GetSQLServerContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new SqlServerStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();
            var response = await fileService.DownloadFileAsync(result.FileId);
            // 3. Assert

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MySqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MySQL,
                Datasource = _context.MySqlDatabaseConnection.DataSource
            };

            var fileRepository = new FileEFRepository(_context.GetMySQLContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MySqlStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();

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

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MySqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MySQL,
                Datasource = _context.MySqlDatabaseConnection.DataSource
            };

            var fileRepository = new FileEFRepository(_context.GetMySQLContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == FileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == FileOptions.ValidatorOptions);
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var databaseStorageOptionsMock = Mock.Of<IOptionsMonitor<DatabaseStorageOptions>>(_ => _.CurrentValue == FileOptions.DatabaseStorageOptions);

            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var storeFileDatabases = new List<IStoreFileDatabase>
            {
                new MySqlStoreFileDatabase()
            };
            var databaseFileConnectorExecution = new DatabaseFileConnectorExecution(
                databaseOptionsMock,
                databaseStorageOptionsMock,
                storeFileDatabases);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               databaseFileConnectorExecution
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();
            var response = await fileService.DownloadFileAsync(result.FileId);
            // 3. Assert

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

            var diskOptions = new DiskStorageOptions
            {
                AllowDayFolder = true,
                IsStoredInTempFolder = false,
                Path = "~"
            };

            var databaseOptions = new Core.Persistences.DatabaseOptions
            {
                ConnectionString = _context.MySqlDatabaseConnection.ConnectionString,
                ConnectionType = Core.Persistences.ConnectionType.MySQL,
                Datasource = _context.MySqlDatabaseConnection.DataSource
            };

            var localFileOptions = FileOptions;
            localFileOptions.DiskStorageOptions = diskOptions;
            localFileOptions.FileStorageType = FileStorageType.Disk;

            var fileRepository = new FileEFRepository(_context.GetMySQLContext());
            var fileOptionsMock = Mock.Of<IOptionsMonitor<LetPortal.Portal.Options.Files.FileOptions>>(_ => _.CurrentValue == localFileOptions);
            var fileValidatorMock = Mock.Of<IOptionsMonitor<FileValidatorOptions>>(_ => _.CurrentValue == localFileOptions.ValidatorOptions);       
            var diskStorageOptionsMock = Mock.Of<IOptionsMonitor<DiskStorageOptions>>(_ => _.CurrentValue == diskOptions);
            var checkFileExtensionRule = new CheckFileExtensionRule(fileValidatorMock);
            var checkFileSizeRule = new CheckFileSizeRule(fileValidatorMock);

            var diskStorage = new DiskFileConnectorExecution(diskStorageOptionsMock);

            var fileService = new FileService(fileOptionsMock, new List<IFileConnectorExecution>
            {
               diskStorage
            }, new List<IFileValidatorRule>
            {
               checkFileExtensionRule,
               checkFileSizeRule
            }, fileRepository);

            // Act
            var result = await fileService.UploadFileAsync(mockFile.Object, "tester");
            memoryStream.Close();

            // 3. Assert
            Assert.True(!string.IsNullOrEmpty(result.FileId));
        }           
        #endregion
    }
}
