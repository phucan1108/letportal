using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Utils;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.Themes
{
    public class Theme_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Theme>("5f02cb57e63b2b3634d51371");

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var themeAssembly = Assembly.Load("Themes.PersonalBlog");
            var themeRegistration = ThemeUtils.GetThemeRegistration(themeAssembly);
            var currentTheme = new Theme
            {
                Id = "5f02cb57e63b2b3634d51371",
                Name = themeRegistration.Name,
                Description = themeRegistration.Description,
                Creator = themeRegistration.Creator,
                ScreenShotUri = themeRegistration.ScreenShotUri,
                ThemeManifests = ThemeUtils.GatherAllManifests(themeAssembly)?.ToList(),
                SectionParts = ThemeUtils.GetSectionParts(themeAssembly)?.ToList()
            };

            versionContext.InsertData(currentTheme);
            return Task.CompletedTask;
        }
    }
}
