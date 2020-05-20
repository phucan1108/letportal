using System.Collections.Generic;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_4_0 : IPortalVersion
    {
        public string VersionNumber => "0.4.0";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5dcac739be0b4e533408344d");
            versionContext.DeleteData<Page>("5dcac739be0b4e5334083453");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var servicesMonitorListsPage = new Page
            {
                Id = "5dcac739be0b4e533408344d",
                Name = "services-monitor",
                DisplayName = "Services Monitor",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/services-monitor",
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
                            Id = "5d0c7da473e71f3330054792",
                            Name = "servicesmonitor",
                            DisplayName = "Services Monitor",
                            ComponentId = "5dc786a40f4b6b13e0a909f4",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            var serviceLogsListsPage = new Page
            {
                Id = "5dcac739be0b4e5334083453",
                Name = "service-logs",
                DisplayName = "Service Logs",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/service-logs",
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
                            Id = "5d0c7da473e71f3330054792",
                            Name = "servicelogs",
                            DisplayName = "Service Logs",
                            ComponentId = "5dcac739be0b4e533408344f",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            versionContext.InsertData(servicesMonitorListsPage);
            versionContext.InsertData(serviceLogsListsPage);
        }
    }
}
