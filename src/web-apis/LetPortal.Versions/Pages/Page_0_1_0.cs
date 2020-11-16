using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Versions.Pages
{
    public class Page_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5cf62f41617a831f7878ba65");
            versionContext.DeleteData<Page>("5cf63188617a831f7878ba66");
            versionContext.DeleteData<Page>("5cf8999e8466f54ae8743180");
            versionContext.DeleteData<Page>("5cc7c8b2dbfdcd4b70964c1a");
            versionContext.DeleteData<Page>("5c06a1854cc9a850bca4448c");
            versionContext.DeleteData<Page>("5cf6190b854f4dbc7921cca7");
            versionContext.DeleteData<Page>("5d0f2dca6ba2fd4ca49e3749");
            versionContext.DeleteData<Page>("5d0f2dca6ba2fd4ca49e3748");
            versionContext.DeleteData<Page>("5cf8999e8466f54ae8743181");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            #region Page
            var pageBuilder = new Page
            {
                Id = "5cf62f41617a831f7878ba65",
                Name = "page-builder",
                DisplayName = "Page Builder",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            var menus = new Page
            {
                Id = "5cf63188617a831f7878ba66",
                Name = "menus",
                DisplayName = "Menus",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/menus",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    }
            };

            var roleClaims = new Page
            {
                Id = "5cf8999e8466f54ae8743180",
                Name = "role-claims",
                DisplayName = "Role Claims",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/roles/:roleName/claims",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    }
            };

            var dynamicListBuilder = new Page
            {
                Id = "5cf8999e8466f54ae8743181",
                Name = "dynamic-list-builder",
                DisplayName = "Dynamic List Builder",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/dynamic-list/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                    {
                        PortalClaimStandards.AllowAccess
                    }
            };

            versionContext.InsertData(pageBuilder);
            versionContext.InsertData(menus);
            versionContext.InsertData(roleClaims);
            versionContext.InsertData(dynamicListBuilder);

            #endregion

            #region Managements Page

            var databaseManagementList = new Page
            {
                Id = "5cc7c8b2dbfdcd4b70964c1a",
                Name = "databases-management",
                DisplayName = "Databases Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/databases-management",
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
                            Id = "5d0c7da473e71f3330054791",
                            ConstructionType = SectionContructionType.DynamicList,
                            ComponentId = "5d0f09de62c8371c183c8c6f",
                            Name = "databasesList",
                            DisplayName = "Databases List",
                            Order = 0
                        }
                    }
                }
            };

            var appManagementList = new Page
            {
                Id = "5c06a1854cc9a850bca4448c",
                Name = "apps-management",
                DisplayName = "Apps Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/apps-management",
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
                            Name = "appsList",
                            DisplayName = "Apps List",
                            ComponentId = "5d0f2dca6ba2fd4ca49e3741",
                            ConstructionType = SectionContructionType.DynamicList,
                            Order = 0
                        }
                    }
                }
            };

            var pageManagementList = new Page
            {
                Id = "5cf6190b854f4dbc7921cca7",
                Name = "pages-management",
                DisplayName = "Pages Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/pages-management",
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
                            Id = "5d0c7da473e71f3330054793",
                            Name = "pagesList",
                            ConstructionType = SectionContructionType.DynamicList,
                            DisplayName = "Pages List",
                            ComponentId = "5d0f2dca6ba2fd4ca49e3742",
                            Order = 0
                        }
                    }
                }
            };

            var userManagementList = new Page
            {
                Id = "5d0f2dca6ba2fd4ca49e3749",
                Name = "users-management",
                DisplayName = "Users Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/users-management",
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
                            Id = "5d0c7da473e71f3330054796",
                            Name = "usersList",
                            DisplayName = "Users List",
                            ConstructionType = SectionContructionType.DynamicList,
                            ComponentId = "5d0f2dca6ba2fd4ca49e3743",
                            Order = 0
                        }
                    }
                }
            };

            var roleManagementList = new Page
            {
                Id = "5d0f2dca6ba2fd4ca49e3748",
                Name = "roles-management",
                DisplayName = "Roles Management",
                AppId = Constants.CoreAppId,
                UrlPath = "portal/page/roles-management",
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
                            Name = "rolesList",
                            DisplayName = "Roles List",
                            ConstructionType = SectionContructionType.DynamicList,
                            ComponentId = "5d0f2dca6ba2fd4ca49e3746",
                            Order = 0
                        }
                    }
                }
            };

            versionContext.InsertData(appManagementList);
            versionContext.InsertData(databaseManagementList);
            versionContext.InsertData(userManagementList);
            versionContext.InsertData(roleManagementList);
            versionContext.InsertData(pageManagementList);
            #endregion

            return Task.CompletedTask;
        }
    }
}
