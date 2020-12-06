using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Features.Menus.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.Menus
{
    public class Menu_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "0.0.1";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<SiteMenu>("5fb398bd8dbe2c62e0a674df");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var menu = new SiteMenu
            {
                Id = "5fb398bd8dbe2c62e0a674df",
                SiteId = Constants.MAIN_SITE_ID,
                Menus = new List<Menu>
                {
                    new Menu
                    {
                        Name = "Home",
                        Url = "/"
                    },
                    new Menu
                    {
                        Name = "Blogs",
                        Url = "/blogs",
                        Sub = new List<Menu>
                        {
                            new Menu
                            {
                                Name = ".NET Core",
                                Url = "/blogs/dotnet-core"
                            },
                            new Menu
                            {
                                Name = "Angular",
                                Url = "/blogs/angular"
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(menu);

            return Task.CompletedTask;
        }
    }
}
