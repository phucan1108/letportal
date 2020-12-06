using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components.DynamicLists
{
    public class DynamicList_0_3_0 : IPortalVersion
    {
        public string VersionNumber => "0.3.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5dabf30467cb8d0bd02643f8");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var chartListComponent = new DynamicList
            {
                Id = "5dabf30467cb8d0bd02643f8",
                Name = "chartsList",
                DisplayName = "Charts List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        EntityName = "components",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"components\": [{ \r\n \"$match\" : {\r\n  \"_t\": {\r\n    $elemMatch: {\r\n      $eq: \"Chart\"\r\n    }\r\n  }\r\n}\r\n}] }}"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `components` Where discriminator='Chart'" : "Select * from components Where discriminator='Chart'")
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
                            Name = "name",
                            DisplayName = "Name",
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
                            Name = "displayName",
                            DisplayName = "Name",
                            AllowSort = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 1
                        },
                        new ColumnDef
                        {
                            Name = "appId",
                            DisplayName = "App",
                            DisplayFormat = "{0}",
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                                Type = DatasourceControlType.Database,
                                DatabaseOptions = new SharedDatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.PortalDatabaseId,
                                    Query = "{\"$query\":{\"apps\":[{\"$project\":{\"name\":\"$displayName\",\"value\":\"$_id\"}}]}}"
                                }
                            },
                            SearchOptions = new SearchOptions
                            {
                                FieldValueType = FieldValueType.Select,
                                AllowInAdvancedMode = true
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
                            Color = "primary",
                            CommandPositionType = CommandPositionType.OutList,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/builder/chart",
                                    IsSameDomain = true
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 0
                        },
                        new CommandButtonInList
                        {
                            Name = "edit",
                            DisplayName = "Edit",
                            Color = "primary",
                            Icon = "create",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/builder/chart/{{data.id}}",
                                    IsSameDomain = true
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 1
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/charts/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this chart?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Chart has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this Chart"
                                }
                            },
                            AllowRefreshList = true,
                            Order = 2
                        },
                        new CommandButtonInList
                        {
                            Name = "clone",
                            DisplayName = "Clone",
                            Icon = "file_copy",
                            Color = "primary",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.CallHttpService,
                                HttpServiceOptions = new HttpServiceOptions
                                {
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/charts/clone",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200",
                                    JsonBody = "{\r\n  \"cloneId\": \"{{data.id}}\",\r\n  \"cloneName\": \"{{data.name}}_clone\"\r\n}"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to clone this chart?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Chart has been cloned successfully!",
                                    FailedMessage = "Oops! We can't clone this chart."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 3
                        }
                    }
                }
            };

            versionContext.InsertData(chartListComponent);
            return Task.CompletedTask;
        }
    }
}
