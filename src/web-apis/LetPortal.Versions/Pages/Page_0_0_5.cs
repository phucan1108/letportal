using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Pages;
using System.Collections.Generic;

namespace LetPortal.Versions.Pages
{
    public class Page_0_0_5 : IVersion
    {
        public string VersionNumber => "0.0.5";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5dcb80b166b49d4b78923a54");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var serviceDashboardPage = new Page
            {
                Id = "5dcb80b166b49d4b78923a54",
                Name = "service-dashboard",
                DisplayName = "Service Dashboard",
                UrlPath = "portal/page/service-dashboard",
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
                            Name = "servicehttpcounter",
                            DisplayName = "HTTP Counters",
                            ComponentId = "5dc786a40f4b6b13e0a909f3",
                            ConstructionType = SectionContructionType.Chart,
                            Order = 0
                        },
                        new PageSection
                        {
                            Id = "5dd02c6d1558c56c40d795e8",
                            Name = "servicehardwarecounter",
                            DisplayName = "Hardware Counters",
                            ComponentId = "5dd02c6d1558c56c40d795e7",
                            ConstructionType = SectionContructionType.Chart,
                            Order = 1 
                        },
                        new PageSection
                        {
                            Id = "5dd2a66d5aa5f917603f05c7",
                            Name = "httprealtimemonitor",
                            DisplayName = "HTTP Requests",
                            ComponentId = "5dd2a66d5aa5f917603f05c6",
                            ConstructionType = SectionContructionType.Chart,
                            Order = 2
                        },
                        new PageSection
                        {
                            Id = "5dd2a66d5aa5f917603f05c8",
                            Name = "hardwarecpurealtimemonitor",
                            DisplayName = "CPU(%)",
                            ComponentId = "5dd2a66d5aa5f917603f05c5",
                            ConstructionType = SectionContructionType.Chart,
                            Order = 3
                        },
                        new PageSection
                        {
                            Id = "5dd2a66d5aa5f917603f05ca",
                            Name = "hardwarememoryrealtimemonitor",
                            DisplayName = "Memory(Mb)",
                            ComponentId = "5dd2a66d5aa5f917603f05c8",
                            ConstructionType = SectionContructionType.Chart,
                            Order = 3
                        }
                    }
                }
            };

            versionContext.InsertData(serviceDashboardPage);
        }
    }
}
