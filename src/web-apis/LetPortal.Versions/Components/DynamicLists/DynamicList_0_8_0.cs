using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components.DynamicLists
{
    public class DynamicList_0_8_0 : IPortalVersion
    {
        public string VersionNumber => "0.8.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5ea80612bf1ac062f89f6f54");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var localizationList = new DynamicList
            {
                Id = "5ea80612bf1ac062f89f6f54",
                Name = "localizationsList",
                DisplayName = "Localization List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB
                       ? "{\r\n  \"$query\": {\r\n    \"localizations\":[\r\n      {\r\n        \"$match\": {\r\n          \"appId\": \"{{queryparams.appId}}\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}" :
                       versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "SELECT * FROM `localizations` Where appId={{queryparams.appId}}" : "SELECT * FROM localizations Where appId={{queryparams.appId}}"
                    },
                    SourceType = DynamicListSourceType.Database
                },
                ColumnsList = new ColumnsList
                {
                    ColumnDefs = new List<ColumnDef>
                    {
                        new ColumnDef
                        {
                            Name = "id",
                            DisplayName = "Id",
                            IsHidden = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false
                            },
                            Order = 0
                        },
                        new ColumnDef
                        {
                            Name = "appId",
                            DisplayName = "App",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false,
                                FieldValueType = FieldValueType.Select
                            },
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                              Type = DatasourceControlType.Database,
                              OutputMapProjection = "name=displayName;value=id",
                              DatabaseOptions = new SharedDatabaseOptions
                              {
                                  DatabaseConnectionId = Constants.PortalDatabaseId,
                                  Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                    "{ \"$query\": { \"apps\": [ ] } }"
                                    : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `apps`" : "Select * from apps")}
                            },
                            Order = 2
                        },
                        new ColumnDef
                        {
                            Name = "localeId",
                            DisplayName = "Locale Id",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 3
                        }
                    }
                },
                CommandsList = new CommandsList
                {
                    CommandButtonsInList = new List<CommandButtonInList>
                    {
                        new CommandButtonInList
                        {
                            Name = "create",
                            DisplayName = "Create",
                            Icon = "create",
                            Color = "primary",
                            CommandPositionType = CommandPositionType.OutList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = true,
                                   RedirectUrl = "portal/builder/localization/{{queryparams.appId}}"
                                }
                            }
                        },
                        new CommandButtonInList
                        {
                            Name = "edit",
                            DisplayName = "Edit",
                            Icon = "create",
                            Color = "primary",
                            CommandPositionType = CommandPositionType.InList,
                            AllowRefreshList = false,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                   IsSameDomain = true,
                                   RedirectUrl = "portal/builder/localization/{{data.appId}}/{{data.localeId}}"
                                }
                            }
                        },
                        new CommandButtonInList
                        {
                            Name = "delete",
                            DisplayName = "Delete",
                            Icon = "delete",
                            Color = "warn",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.CallHttpService,
                                HttpServiceOptions = new HttpServiceOptions
                                {
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/localizations/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this language package?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Language package has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this Language package."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 2
                        }
                    }
                }
            };

            versionContext.InsertData(localizationList);

            return Task.CompletedTask;
        }
    }
}
