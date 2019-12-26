using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Recoveries;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Options.Recoveries;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Recoveries;
using LetPortal.Portal.Services.Recoveries;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class BackupServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public BackupServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Create_One_Backup_File()
        {
            // Arrange
            var mockAppProvider = new Mock<IAppServiceProvider>();
            mockAppProvider
                .Setup(a => a.GetAppsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<App>>(null));

            var mockStandardProvider = new Mock<IStandardServiceProvider>();
            mockStandardProvider
                .Setup(a => a.GetStandardComponentsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<StandardComponent>>(null));

            var mockChartProvider = new Mock<IChartServiceProvider>();
            mockChartProvider
                .Setup(a => a.GetChartsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<Chart>>(null));

            var mockDynamicListProvider = new Mock<IDynamicListServiceProvider>();
            mockDynamicListProvider
                .Setup(a => a.GetDynamicListsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<DynamicList>>(null));

            var mockPageProvider = new Mock<IPageServiceProvider>();
            mockPageProvider
                .Setup(a => a.GetPagesByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<Page>>(null));

            var mockDatabaseProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseProvider
                .Setup(a => a.GetDatabaseConnectionsByIds(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult<IEnumerable<DatabaseConnection>>(new List<DatabaseConnection> { _context.MongoDatabaseConenction }));

            var mockFileProvider = new Mock<IFileSeviceProvider>();
            mockFileProvider
                .Setup(a => a.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ResponseUploadFile { DownloadableUrl = "http://localhost", FileId = DataUtil.GenerateUniqueId() }));

            var mockBackupRepository = new Mock<IBackupRepository>();
            mockBackupRepository
                .Setup(a => a.AddAsync(It.IsAny<Backup>()))
                .Returns(Task.CompletedTask);

            var backupOptions = new BackupOptions
            {
                FolderPath = "."
            };

            var backupOptionsMock = Mock.Of<IOptionsMonitor<BackupOptions>>(_ => _.CurrentValue == backupOptions);

            var backupService = new BackupService(
                mockAppProvider.Object,
                mockStandardProvider.Object,
                mockChartProvider.Object,
                mockDynamicListProvider.Object,
                mockDatabaseProvider.Object,
                mockPageProvider.Object,
                mockFileProvider.Object,
                mockBackupRepository.Object,
                backupOptionsMock);

            // Act
            var result = await backupService.CreateBackupFile(new LetPortal.Portal.Models.Recoveries.BackupRequestModel
            {
                Name = "BK_Test",
                Description = "BK Test",
                Creator = "Admin"
            });

            // Assert
            Assert.True(!string.IsNullOrEmpty(result.DownloadableUrl));
        }
    }
}
