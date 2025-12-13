using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.Sites
{
    public class Site_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Site>(Constants.MAIN_SITE_ID);

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var taliSite = new Site
            {
                Id = Constants.MAIN_SITE_ID,
                DefaultLocaleId = "default",
                DefaultPathWhenNotFound = "/404",
                Domains = new System.Collections.Generic.List<string> { "localhost:5104", "localhost:5104" },
                Enable = true,
                Name = "Personal Blog Site",
                ThemeId = "5f02cb57e63b2b3634d51371"
            };

            versionContext.InsertData(taliSite);

            return Task.CompletedTask;
        }
    }
}
