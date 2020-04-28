using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_8_0 : IPortalVersion
    {
        public string VersionNumber => "0.8.0";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5ea80612bf1ac062f89f6f55");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var localizationListPage = new Page
            {
               Id = "5ea80612bf1ac062f89f6f55",
                Name = "localizations-management",
                DisplayName = "Localizations Management",
                UrlPath = "portal/page/localizations-management",
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

            versionContext.InsertData(localizationListPage);            
        }
    }
}
