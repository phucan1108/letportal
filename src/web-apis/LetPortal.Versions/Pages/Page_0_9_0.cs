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
    public class Page_0_9_0 : IPortalVersion
    {
        public string VersionNumber => "0.9.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5f6deef86ed70e6f98cf6702");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var appMenuBuilderPage = new Page
            {
                Id = "5f6deef86ed70e6f98cf6702",
                Name = "app-menu-builder",
                DisplayName = "App Menu Builder",
                UrlPath = "portal/page/app-menu-builder",
                AppId = Constants.CoreAppId,
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
                            Id = "85add2ce-37af-95f8-f467-07491dd9cff5",
                            Name = "appmenu",
                            DisplayName = "App Menu",
                            ConstructionType = SectionContructionType.Tree,
                            SectionDatasource = new SectionDatasource
                            {
                                DatasourceBindName = "data.menus",
                                DataStoreName = "menus"
                            },
                            ComponentId = "5f6f751c118d1754ac475aa1"
                        }
                    }
                },
                PageDatasources = new List<PageDatasource>
                {
                    new PageDatasource
                    {
                        Id = "e240508f-69d4-01ad-9e73-a2ea3a048f58",
                        Name = "data",
                        TriggerCondition = "!!queryparams.appId",
                        IsActive = true,
                        Options = new Portal.Entities.Shared.DatasourceOptions
                        {
                            Type = Portal.Entities.Shared.DatasourceControlType.Database,
                            DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                            {
                                DatabaseConnectionId = Constants.PortalDatabaseId,
                                Query = "{\"$query\":{\"apps\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.appId}}')\"}}]}}"
                            }
                        }
                    }
                },
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "ac79acb6-bccf-7236-8798-cec7d82e57be",
                        Name = "Update",
                        Icon = "edit",
                        Color = "primary",
                        IsRequiredValidation = true,
                        AllowHidden = "!queryparams.appId",
                        PlaceSectionId = "85add2ce-37af-95f8-f467-07491dd9cff5",
                        ButtonOptions = new Portal.Entities.Shared.ButtonOptions
                        {
                            ActionCommandOptions = new Portal.Entities.Shared.ActionCommandOptions
                            {
                                IsEnable = true,
                                ActionType = Portal.Entities.Shared.ActionType.ExecuteDatabase,
                                DbExecutionChains = new Portal.Entities.Shared.DatabaseExecutionChains
                                {
                                    Steps = new List<Portal.Entities.Shared.DatabaseExecutionStep>
                                    {
                                        new Portal.Entities.Shared.DatabaseExecutionStep
                                        {
                                            DatabaseConnectionId = Constants.PortalDatabaseId,
                                            ExecuteCommand = "{\"$update\":{\"apps\":{\"$data\":{\"menus\":\"{{data.menus}}\"},\"$where\":{\"_id\":\"ObjectId('{{queryparams.appId}}')\"}}}}"
                                        }
                                    }
                                },
                                NotificationOptions = new Portal.Entities.Shared.NotificationOptions
                                {
                                    CompleteMessage = "Completed!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
                                },
                                ConfirmationOptions = new Portal.Entities.Shared.ConfirmationOptions
                                {
                                    ConfirmationText = "Are you sure to update the menu?",
                                    IsEnable = true
                                }
                            }
                        }
                    },
                    new PageButton
                    {
                        Id = "2623213c-8f01-37db-1158-9832401c73a9",
                        Name = "Cancel",
                        Icon = "cancel",
                        Color = "basic",
                        PlaceSectionId = "85add2ce-37af-95f8-f467-07491dd9cff5",
                        ButtonOptions = new Portal.Entities.Shared.ButtonOptions
                        {
                            ActionCommandOptions = new Portal.Entities.Shared.ActionCommandOptions
                            {
                                ActionType = Portal.Entities.Shared.ActionType.Redirect,
                                IsEnable = false,
                                ConfirmationOptions = new Portal.Entities.Shared.ConfirmationOptions
                                {
                                }
                            },
                            RouteOptions = new Portal.Entities.Shared.RouteOptions
                            {
                                IsEnable = true,
                                Routes = new List<Portal.Entities.Shared.Route>
                                {
                                    new Portal.Entities.Shared.Route
                                    {
                                        IsSameDomain = true,
                                         Condition = "true",
                                         RedirectUrl = "portal/page/apps-management"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(appMenuBuilderPage);
            return Task.CompletedTask;
        }
    }
}
