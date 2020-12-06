using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Exceptions.Recoveries;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Options.Recoveries;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.CompositeControls;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.Recoveries;
using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Recoveries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class BackupServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        private readonly string zipFileName = "637136665426993102.zip";

        public BackupServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Create_One_Backup_File_Mongo()
        {
            // Arrange 
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            BackupOptions backupOptions = new BackupOptions
            {
                BackupFolderPath = "."
            };

            BackupMongoRepository backupRepository = new BackupMongoRepository(_context.GetMongoConnection());
            IBackupService backupService = GetMockBackupService(backupRepository, backupOptions);

            // Act
            LetPortal.Portal.Models.Recoveries.BackupResponseModel result = await backupService.CreateBackupFile(new LetPortal.Portal.Models.Recoveries.BackupRequestModel
            {
                Name = "BK_Test",
                Description = "BK Test",
                Creator = "Admin"
            });

            backupRepository.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result.DownloadableUrl));
        }

        [Fact]
        public async Task Reach_Maximum_Backup_File_Mongo()
        {
            // Arrange 
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            BackupOptions backupOptions = new BackupOptions
            {
                BackupFolderPath = "Backup",
                MaximumObjects = 0
            };

            BackupMongoRepository backupRepository = new BackupMongoRepository(_context.GetMongoConnection());
            IBackupService backupService = GetMockBackupService(backupRepository, backupOptions);

            // Act
            try
            {
                LetPortal.Portal.Models.Recoveries.BackupResponseModel result = await backupService.CreateBackupFile(new LetPortal.Portal.Models.Recoveries.BackupRequestModel
                {
                    Name = "BK_Test",
                    Description = "BK Test",
                    Creator = "Admin"
                });
            }
            catch(Exception ex)
            {
                Assert.True(ex is BackupException backupException && backupException.ErrorCode.MessageCode == BackupErrorCodes.ReachMaximumBackupObjects.MessageCode);
            }
            finally
            {
                backupRepository.Dispose();
            }
        }

        [Fact]
        public async Task Restore_One_Backup_File_Mongo()
        {
            // Arrange 
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            Mock<IFormFile> mockFile = new Mock<IFormFile>();
            FileStream sourceZip = System.IO.File.OpenRead(@"Artifacts\" + zipFileName);
            MemoryStream memoryStream = new MemoryStream();
            await sourceZip.CopyToAsync(memoryStream);
            sourceZip.Close();
            memoryStream.Position = 0;
            string fileName = zipFileName;
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length).Verifiable();
            mockFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream).Verifiable();
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream))
                .Verifiable();

            BackupOptions backupOptions = new BackupOptions
            {
                BackupFolderPath = "Backup",
                MaximumObjects = 0,
                RestoreFolderPath = "Restore"
            };

            BackupMongoRepository backupRepository = new BackupMongoRepository(_context.GetMongoConnection());
            IBackupService backupService = GetMockBackupService(backupRepository, backupOptions);
            // Act
            UploadBackupResponseModel result = await backupService.UploadBackupFile(mockFile.Object, "Admin");
            memoryStream.Close();
            memoryStream.Dispose();
            backupRepository.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Preview_Backup_Mongo()
        {
            // Arrange 
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            Mock<IFormFile> mockFile = new Mock<IFormFile>();
            FileStream sourceZip = System.IO.File.OpenRead(@"Artifacts\" + zipFileName);
            MemoryStream memoryStream = new MemoryStream();
            await sourceZip.CopyToAsync(memoryStream);
            sourceZip.Close();
            memoryStream.Position = 0;
            string fileName = zipFileName;
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length).Verifiable();
            mockFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream).Verifiable();
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream))
                .Verifiable();
            BackupOptions backupOptions = new BackupOptions
            {
                BackupFolderPath = "Backup",
                MaximumObjects = 0,
                RestoreFolderPath = "Restore"
            };

            BackupMongoRepository backupRepository = new BackupMongoRepository(_context.GetMongoConnection());
            IBackupService backupService = GetMockBackupService(backupRepository, backupOptions);

            // Act
            UploadBackupResponseModel backupResponse = await backupService.UploadBackupFile(mockFile.Object, "Admin");

            LetPortal.Portal.Models.Recoveries.PreviewRestoreModel result = await backupService.PreviewBackup(backupResponse.Id);
            memoryStream.Close();
            memoryStream.Dispose();
            backupRepository.Dispose();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Restore_Backup_Mongo()
        {
            // Arrange 
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            Mock<IFormFile> mockFile = new Mock<IFormFile>();
            FileStream sourceZip = System.IO.File.OpenRead(@"Artifacts\" + zipFileName);
            MemoryStream memoryStream = new MemoryStream();
            await sourceZip.CopyToAsync(memoryStream);
            sourceZip.Close();
            memoryStream.Position = 0;
            string fileName = zipFileName;
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length).Verifiable();
            mockFile.Setup(f => f.FileName).Returns(fileName).Verifiable();
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream).Verifiable();
            mockFile
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => memoryStream.CopyToAsync(stream))
                .Verifiable();
            BackupOptions backupOptions = new BackupOptions
            {
                BackupFolderPath = "Backup",
                MaximumObjects = 0,
                RestoreFolderPath = "Restore"
            };

            BackupMongoRepository backupRepository = new BackupMongoRepository(_context.GetMongoConnection());
            IBackupService backupService = GetMockBackupServiceForRestore(backupRepository, backupOptions);

            // Act
            UploadBackupResponseModel backupResponse = await backupService.UploadBackupFile(mockFile.Object, "Admin");

            await backupService.RestoreBackupPoint(backupResponse.Id);
            memoryStream.Close();
            memoryStream.Dispose();
            backupRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        private IBackupService GetMockBackupService(
            IBackupRepository backupRepository,
            BackupOptions options)
        {
            Mock<IAppServiceProvider> mockAppProvider = new Mock<IAppServiceProvider>();
            mockAppProvider
                .Setup(a => a.GetAppsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<App>>(null));

            Mock<IStandardServiceProvider> mockStandardProvider = new Mock<IStandardServiceProvider>();
            mockStandardProvider
                .Setup(a => a.GetStandardComponentsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<StandardComponent>>(null));

            Mock<IChartServiceProvider> mockChartProvider = new Mock<IChartServiceProvider>();
            mockChartProvider
                .Setup(a => a.GetChartsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<Chart>>(null));

            Mock<IDynamicListServiceProvider> mockDynamicListProvider = new Mock<IDynamicListServiceProvider>();
            mockDynamicListProvider
                .Setup(a => a.GetDynamicListsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<DynamicList>>(null));

            Mock<IPageServiceProvider> mockPageProvider = new Mock<IPageServiceProvider>();
            mockPageProvider
                .Setup(a => a.GetPagesByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<Page>>(null));

            Mock<IDatabaseServiceProvider> mockDatabaseProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseProvider
                .Setup(a => a.GetDatabaseConnectionsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<DatabaseConnection>>(new List<DatabaseConnection> { _context.MongoDatabaseConenction }));

            mockDatabaseProvider
                .Setup(a => a.CompareDatabases(It.IsAny<IEnumerable<DatabaseConnection>>()))
                .Returns(Task.FromResult<IEnumerable<ComparisonResult>>(new List<ComparisonResult> { }));

            Mock<IFileSeviceProvider> mockFileProvider = new Mock<IFileSeviceProvider>();
            mockFileProvider
                .Setup(a => a.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ResponseUploadFile { DownloadableUrl = "http://localhost", FileId = DataUtil.GenerateUniqueId() }));

            mockFileProvider
                .Setup(a => a.ValidateFile(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(true));

            mockFileProvider
                .Setup(a => a.DownloadFileAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResponseDownloadFile
                {
                    FileBytes = File.ReadAllBytes(@"Artifacts\" + zipFileName),
                    FileName = zipFileName,
                    MIMEType = "application/zip"
                }));

            Mock<ICompositeControlServiceProvider> mockCompositeControlProvider = new Mock<ICompositeControlServiceProvider>();
            mockCompositeControlProvider
                .Setup(a => a.GetByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<CompositeControl>>(null));

            IOptionsMonitor<BackupOptions> backupOptionsMock = Mock.Of<IOptionsMonitor<BackupOptions>>(_ => _.CurrentValue == options);

            BackupService backupService = new BackupService(
                mockAppProvider.Object,
                mockStandardProvider.Object,
                mockChartProvider.Object,
                mockDynamicListProvider.Object,
                mockDatabaseProvider.Object,
                mockPageProvider.Object,
                mockFileProvider.Object,
                backupRepository,
                mockCompositeControlProvider.Object,
                backupOptionsMock,
                new FakeServiceLogger<BackupService>());

            return backupService;
        }

        private IBackupService GetMockBackupServiceForRestore(IBackupRepository backupRepository,
            BackupOptions options)
        {
            Mock<IAppServiceProvider> mockAppProvider = new Mock<IAppServiceProvider>();
            mockAppProvider
                .Setup(a => a.GetAppsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<App>>(null));

            Mock<IStandardServiceProvider> mockStandardProvider = new Mock<IStandardServiceProvider>();
            mockStandardProvider
                .Setup(a => a.GetStandardComponentsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<StandardComponent>>(null));

            Mock<IChartServiceProvider> mockChartProvider = new Mock<IChartServiceProvider>();
            mockChartProvider
                .Setup(a => a.GetChartsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<Chart>>(null));

            Mock<IDynamicListServiceProvider> mockDynamicListProvider = new Mock<IDynamicListServiceProvider>();
            mockDynamicListProvider
                .Setup(a => a.GetDynamicListsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<DynamicList>>(null));

            Mock<IPageServiceProvider> mockPageProvider = new Mock<IPageServiceProvider>();
            mockPageProvider
                .Setup(a => a.GetPagesByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<Page>>(null));

#pragma warning disable CA2000 // Dispose objects before losing scope
            InternalDatabaseServiceProvider databaserProvider = new InternalDatabaseServiceProvider(
                new DatabaseService(null, null),
                new DatabaseMongoRepository(_context.GetMongoConnection()));
#pragma warning restore CA2000 // Dispose objects before losing scope

            Mock<IFileSeviceProvider> mockFileProvider = new Mock<IFileSeviceProvider>();
            mockFileProvider
                .Setup(a => a.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ResponseUploadFile { DownloadableUrl = "http://localhost", FileId = DataUtil.GenerateUniqueId() }));

            mockFileProvider
                .Setup(a => a.ValidateFile(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(true));

            mockFileProvider
                .Setup(a => a.DownloadFileAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResponseDownloadFile
                {
                    FileBytes = File.ReadAllBytes(@"Artifacts\" + zipFileName),
                    FileName = zipFileName,
                    MIMEType = "application/zip"
                }));

            IOptionsMonitor<BackupOptions> backupOptionsMock = Mock.Of<IOptionsMonitor<BackupOptions>>(_ => _.CurrentValue == options);

            Mock<ICompositeControlServiceProvider> mockCompositeControlProvider = new Mock<ICompositeControlServiceProvider>();
            mockCompositeControlProvider
                .Setup(a => a.GetByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<CompositeControl>>(null));

            BackupService backupService = new BackupService(
                mockAppProvider.Object,
                mockStandardProvider.Object,
                mockChartProvider.Object,
                mockDynamicListProvider.Object,
                databaserProvider,
                mockPageProvider.Object,
                mockFileProvider.Object,
                backupRepository,
                mockCompositeControlProvider.Object,
                backupOptionsMock,
                new FakeServiceLogger<BackupService>());

            databaserProvider.Dispose();

            return backupService;
        }
    }
}
