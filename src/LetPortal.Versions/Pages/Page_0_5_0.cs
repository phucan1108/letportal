using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Pages
{
    public class Page_0_5_0 : IPortalVersion
    {
        public string VersionNumber => "0.5.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5dcb80b166b49d4b78923a54");
            versionContext.DeleteData<Page>("5e1aa91e3c107562acf358b5");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var serviceDashboardPage = new Page
            {
                Id = "5dcb80b166b49d4b78923a54",
                Name = "service-dashboard",
                DisplayName = "Service Dashboard",
                AppId = Constants.CoreAppId,
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
                },
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "5eaebdb89533066fa085a848",
                        Name = "Cancel",
                        Icon = "close",
                        Color = "basic",
                        AllowHidden = "false",
                        ButtonOptions = new ButtonOptions
                        {                                
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                IsEnable = false
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Route>
                                {
                                    new Route
                                    {                                         
                                        Condition = "true",
                                        RedirectUrl = "portal/page/services-monitor",
                                        IsSameDomain = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var backupManagementPage = new Page
            {
                Id = "5e1aa91e3c107562acf358b5",
                Name = "backup-management",
                DisplayName = "Backup Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/backup-management",
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
                            Id = "5e1aa91e3c107562acf358b5",
                            Name = "backupslist",
                            DisplayName = "Backups List",
                            ComponentId = "5e1aa91e3c107562acf358b2",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            versionContext.InsertData(serviceDashboardPage);
            versionContext.InsertData(backupManagementPage);
            return Task.CompletedTask;
        }
    }
}
