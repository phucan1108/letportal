using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_5_2 : IPortalVersion
    {
        public string VersionNumber => "0.5.2";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5e6b645a29ae8d569813fd4b");
            versionContext.DeleteData<Page>("5e6b645a29ae8d569813fd4c");
            versionContext.DeleteData<Page>("5e6b645a29ae8d569813fd4d");
            versionContext.DeleteData<Page>("5e6b645a29ae8d569813fd4e");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var chartBuilderPage = new Page
            {
                Id = "5e6b645a29ae8d569813fd4b",
                Name = "chart-builder",
                DisplayName = "Chart Builder",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/chart/:chartid",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    }
            };

            var backupBuilder = new Page
            {
                Id = "5e6b645a29ae8d569813fd4c",
                Name = "backup-builder",
                DisplayName = "Backup Builder",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/backup",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            var backupUploadBuilder = new Page
            {
                Id = "5e6b645a29ae8d569813fd4d",
                Name = "backup-upload",
                DisplayName = "Backup Upload Page",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/backup/upload",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            var backupRestoreBuilder = new Page
            {
                Id = "5e6b645a29ae8d569813fd4e",
                Name = "backup-restore",
                DisplayName = "Backup Restore Page",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/backup/restore",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            versionContext.InsertData(chartBuilderPage);
            versionContext.InsertData(backupBuilder);
            versionContext.InsertData(backupUploadBuilder);
            versionContext.InsertData(backupRestoreBuilder);
            return Task.CompletedTask;
        }
    }
}
