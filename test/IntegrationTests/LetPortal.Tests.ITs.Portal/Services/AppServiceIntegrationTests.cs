using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Files;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Localizations;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Services.Apps;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class AppServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        private readonly string zipFileName = "CoreApp_0.0.1_637251308120576119.zip";

        public AppServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Package_File_Mongo()
        {
            // Arrange 
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            var appService = GetMockAppService();

            // Act
            var result = await appService.Package(new LetPortal.Portal.Models.Apps.PackageRequestModel
            {
               AppId = "abc",
               Creator = "Tester",
               Description = "Tester App"
            }).ConfigureAwait(false);

            // Assert
            Assert.True(!string.IsNullOrEmpty(result.DownloadableUrl));
        }

        [Fact]
        public async Task Unpack_File_Mongo()
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
            await sourceZip.CopyToAsync(memoryStream).ConfigureAwait(false);
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
            var appService = GetMockAppService();

            // Act
            var result = await appService.Unpack(mockFile.Object, "Admin").ConfigureAwait(false);
            memoryStream.Close();
            memoryStream.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result.UploadFileId));
        }

        [Fact]
        public async Task Install_App_Package_With_Merge_Mongo()
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
            await sourceZip.CopyToAsync(memoryStream).ConfigureAwait(false);
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
            var appService = GetMockAppService();

            // Act
            await appService.Install("123", InstallWay.Merge).ConfigureAwait(false);
            memoryStream.Close();
            memoryStream.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Install_App_Package_With_Wipe_Mongo()
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
            await sourceZip.CopyToAsync(memoryStream).ConfigureAwait(false);
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
            var appService = GetMockAppService(true);

            // Act
            await appService.Install("123", InstallWay.Wipe).ConfigureAwait(false);
            memoryStream.Close();
            memoryStream.Dispose();
            // Assert
            Assert.True(true);
        }

        private IAppService GetMockAppService(bool allowExist = false)
        {

            Mock<IStandardServiceProvider> mockStandardProvider = new Mock<IStandardServiceProvider>();
            mockStandardProvider
                .Setup(a => a.GetByAppId(It.IsAny<string>()))
                .Returns(Task.FromResult<IEnumerable<StandardComponent>>(new List<StandardComponent>
                {
                    new StandardComponent
                    {
                        Id = "abc",
                        Name = "Test Standard"
                    }
                }));

            Mock<IChartServiceProvider> mockChartProvider = new Mock<IChartServiceProvider>();
            mockChartProvider
                .Setup(a => a.GetByAppId(It.IsAny<string>()))
                .Returns(Task.FromResult<IEnumerable<Chart>>(new List<Chart>
                {
                    new Chart
                    {
                        Id = "123",
                        Name = "Test Chart" 
                    }
                }));

            Mock<IDynamicListServiceProvider> mockDynamicListProvider = new Mock<IDynamicListServiceProvider>();
            mockDynamicListProvider
                .Setup(a => a.GetByAppId(It.IsAny<string>()))
                .Returns(Task.FromResult<IEnumerable<DynamicList>>(new List<DynamicList>
                {
                    new DynamicList
                    {
                        Id = "234",
                        Name = "Test Dynamic List"
                    }
                }));

            Mock<IPageServiceProvider> mockPageProvider = new Mock<IPageServiceProvider>();
            mockPageProvider
                .Setup(a => a.GetByAppId(It.IsAny<string>()))
                .Returns(Task.FromResult<IEnumerable<Page>>(new List<Page>
                {
                    new Page
                    {
                        Id = "2345",
                        Name = "Test Page"
                    }
                }));

            Mock<ILocalizationProvider> mockLocalizationProvider = new Mock<ILocalizationProvider>();
            mockLocalizationProvider
                .Setup(a => a.GetByAppId(It.IsAny<string>()))
                .Returns(Task.FromResult<IEnumerable<Localization>>(new List<Localization>
                {
                    new Localization
                    {
                        Id = "asb",
                        LocaleId = "en-Us"
                    }
                }));
            
            mockStandardProvider
                .Setup(a => a.ForceUpdateStandards(It.IsAny<IEnumerable<StandardComponent>>()))
                .Returns(Task.CompletedTask);
            mockStandardProvider
                .Setup(a => a.DeleteAllByAppIdAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
                        
            mockChartProvider
                .Setup(a => a.ForceUpdateCharts(It.IsAny<IEnumerable<Chart>>()))
                .Returns(Task.CompletedTask);
            mockChartProvider
                .Setup(a => a.DeleteByAppIdAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
                        
            mockDynamicListProvider
                .Setup(a => a.ForceUpdateDynamicLists(It.IsAny<IEnumerable<DynamicList>>()))
                .Returns(Task.CompletedTask);
            mockDynamicListProvider
                .Setup(a => a.DeleteByAppIdAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
                        
            mockPageProvider
                .Setup(a => a.ForceUpdatePages(It.IsAny<IEnumerable<Page>>()))
                .Returns(Task.CompletedTask);
            mockPageProvider
                .Setup(a => a.DeleteByAppIdAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            mockLocalizationProvider
                .Setup(a => a.ForceUpdateLocalizations(It.IsAny<IEnumerable<Localization>>()))
                .Returns(Task.CompletedTask);
            mockLocalizationProvider
                .Setup(a => a.DeleteByAppIdAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            Mock<IFileSeviceProvider> mockFileProvider = new Mock<IFileSeviceProvider>();
            mockFileProvider
                .Setup(a => a.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new ResponseUploadFile { DownloadableUrl = "http://localhost", FileId = DataUtil.GenerateUniqueId() }));

            mockFileProvider
                .Setup(a => a.ValidateFile(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(true));

            var mockAppRepository = new Mock<IAppRepository>();
            mockAppRepository
                .Setup(a => a.GetOneAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new LetPortal.Portal.Entities.Apps.App 
                {   
                    Name = "CoreApp",
                    CurrentVersionNumber = "0.0.1"
                }));
            mockAppRepository
                .Setup(a => a.IsExistAsync(It.IsAny<Expression<Func<App, bool>>>()))
                .Returns(Task.FromResult(allowExist));

            mockFileProvider
                .Setup(a => a.DownloadFileAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResponseDownloadFile
                {
                    FileBytes = File.ReadAllBytes(@"Artifacts\" + zipFileName),
                    FileName = zipFileName,
                    MIMEType = "application/zip"
                }));

            var appService = new AppService(
                mockStandardProvider.Object,
                mockChartProvider.Object,
                mockDynamicListProvider.Object,
                mockPageProvider.Object,
                mockFileProvider.Object,
                mockLocalizationProvider.Object,
                mockAppRepository.Object
                );

            return appService;
        }
    }
}
