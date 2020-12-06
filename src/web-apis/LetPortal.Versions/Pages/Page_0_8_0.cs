using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_8_0 : IPortalVersion
    {
        public string VersionNumber => "0.8.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5ea80612bf1ac062f89f6f55");
            versionContext.DeleteData<Page>("5eb815a1db8e096080a93f70");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var localizationListPage = new Page
            {
               Id = "5ea80612bf1ac062f89f6f55",
                Name = "localization-management",
                DisplayName = "Localization",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/localization-management",
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    },
                Builder = new PageBuilder
                {
                    Sections = new List<PageSection>
                    {
                        new PageSection
                        {
                            Id = "5e79c14931a1754a2cd38cbe",
                            Name = "localizationsList",
                            DisplayName = "Localizations List",
                            ComponentId = "5ea80612bf1ac062f89f6f54",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0,
                            SectionDatasource = new SectionDatasource
                            {
                                DatasourceBindName = "data"
                            }
                        }
                    }
                }
            };

            var localizationBuilder = new Page
            {
                Id = "5eb815a1db8e096080a93f70",
                Name = "localization-builder",
                DisplayName = "Localizationage Builder",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/builder/localization",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            versionContext.InsertData(localizationListPage);
            versionContext.InsertData(localizationBuilder);
            return Task.CompletedTask;
        }
    }
}
