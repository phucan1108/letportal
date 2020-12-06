using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Models;
using LetPortal.CMS.Core.Providers;
using LetPortal.CMS.Core.Repositories.Sites;
using LetPortal.Core.Utils;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.UTs.CMS.Providers
{
    public class ThemeProviderTests
    {
        [Fact]
        public async Task Load_Manifest_Value_Type_Test()
        {
            // Arrange
            Mock<ISiteManifestRepository> mockSiteManfiestRepository = new Mock<ISiteManifestRepository>();
            mockSiteManfiestRepository
                .Setup(a => a.GetSiteManifestsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new List<LetPortal.CMS.Core.Entities.SiteManifest>
                {
                    new LetPortal.CMS.Core.Entities.SiteManifest
                    {
                        Key = "CompanyLink",
                        ConfigurableValue = "Company A",
                        EditorType = LetPortal.CMS.Core.Entities.ManifestEditorType.Textbox
                    }
                }));
            ThemeProvider themeProvider = new ThemeProvider(mockSiteManfiestRepository.Object, null);
            // Act
            SampleModel result = await themeProvider.LoadAsync(new SampleModel(), "1");
            // Assert

            Assert.True(!string.IsNullOrEmpty(result.CompanyLink));
        }

        [Fact]
        public async Task Load_Manifest_Json_Type_Test()
        {
            // Arrange
            Mock<ISiteManifestRepository> mockSiteManfiestRepository = new Mock<ISiteManifestRepository>();
            mockSiteManfiestRepository
                .Setup(a => a.GetSiteManifestsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new List<LetPortal.CMS.Core.Entities.SiteManifest>
                {
                    new LetPortal.CMS.Core.Entities.SiteManifest
                    {
                        Key = "CompanyLink",
                        ConfigurableValue = ConvertUtil.SerializeObject(new LinkModel{ }),
                        EditorType = LetPortal.CMS.Core.Entities.ManifestEditorType.Textbox
                    }
                }));
            ThemeProvider themeProvider = new ThemeProvider(mockSiteManfiestRepository.Object, null);
            // Act
            JsonModel result = await themeProvider.LoadAsync(new JsonModel(), "1");
            // Assert

            Assert.NotNull(result.CompanyLink);
        }
    }

    public class SampleModel : IThemeManifest
    {
        public string CompanyLink { get; set; }
    }

    public class JsonModel : IThemeManifest
    {
        public LinkModel CompanyLink { get; set; }
    }
}
