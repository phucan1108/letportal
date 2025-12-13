using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Repositories.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_8_1 : IPortalVersion
    {
        private readonly IPageRepository _pageRepository;

        public Page_0_8_1(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public string VersionNumber => "0.8.1";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5ed90dd418607f61f0a0512a");
            versionContext.DeleteData<Page>("5ed90dd418607f61f0a0512b");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var appPackage = new Page
            {
                Id = "5ed90dd418607f61f0a0512a",
                Name = "app-package",
                DisplayName = "App Package",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/app-package",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    }
            };

            var appInstallation = new Page
            {
                Id = "5ed90dd418607f61f0a0512b",
                Name = "app-installation",
                DisplayName = "App Installation",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/builder/app-installation",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            versionContext.InsertData(appPackage);
            versionContext.InsertData(appInstallation);
            return Task.CompletedTask;
        }
    }
}
