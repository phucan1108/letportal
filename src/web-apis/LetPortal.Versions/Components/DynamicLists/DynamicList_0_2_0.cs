using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components.DynamicLists
{
    public class DynamicList_0_2_0 : IPortalVersion
    {
        public string VersionNumber => "0.2.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5d4d8adfae5f5b68b811ec1d");
            versionContext.DeleteData<DynamicList>("5d4d8adfae5f5b68b811ec1e");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var dynamicListComponent = new DynamicList
            {
                Id = "5d4d8adfae5f5b68b811ec1d",
                Name = "dynamicsList",
                DisplayName = "Dynamic Lists List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        EntityName = "components",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"components\": [{ \r\n \"$match\" : {\r\n  \"_t\": {\r\n    $elemMatch: {\r\n      $eq: \"DynamicList\"\r\n    }\r\n  }\r\n}\r\n}] }}"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `components` Where discriminator='DynamicList'" : "Select * from components Where discriminator='DynamicList'")
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
                                    RedirectUrl = "portal/builder/dynamic-list",
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
                                    RedirectUrl = "portal/builder/dynamic-list/{{data.id}}",
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/dynamiclists/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this dynamic list?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Dynamic List has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this Dynamic List."
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/dynamiclists/clone",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200",
                                    JsonBody = "{\r\n  \"cloneId\": \"{{data.id}}\",\r\n  \"cloneName\": \"{{data.name}}_clone\"\r\n}"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to clone this dynamic list?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Dynamic list has been cloned successfully!",
                                    FailedMessage = "Oops! We can't clone this dynamic list."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 3
                        }
                    }
                }
            };

            var standardComponent = new DynamicList
            {
                Id = "5d4d8adfae5f5b68b811ec1e",
                Name = "standardsList",
                DisplayName = "Standards List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        EntityName = "components",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"components\": [{ \r\n \"$match\" : {\r\n  \"_t\": {\r\n    $elemMatch: {\r\n      $eq: \"StandardComponent\"\r\n    }\r\n  }\r\n}\r\n}] }}"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `components` Where discriminator='StandardComponent'" : "Select * from components Where discriminator='StandardComponent'")
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
                            Name = "layoutType",
                            DisplayName = "Layout Type",
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.Select
                            },
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                                Type = DatasourceControlType.StaticResource,
                                DatasourceStaticOptions = new DatasourceStaticOptions
                                {
                                    JsonResource = "[{\"name\":\"One Column\",\"value\":0},{\"name\":\"Two Columns\",\"value\":1},{\"name\":\"Three Columns\",\"value\":2},{\"name\":\"Four Columns\",\"value\":3},{\"name\":\"Six Columns\",\"value\":4}]"
                                }
                            },
                            Order = 1
                        },
                        new ColumnDef
                        {
                            Name = "type",
                            DisplayName = "Type",
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.Select
                            },
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                                Type = DatasourceControlType.StaticResource,
                                DatasourceStaticOptions = new DatasourceStaticOptions
                                {
                                    JsonResource = "[{\"name\":\"Standard\",\"value\":0},{\"name\":\"Array\",\"value\":1},{\"name\":\"Tree\",\"value\":2}]"
                                }
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
                            ActionCommandOptions =new ActionCommandOptions
                            {
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/builder/standard",
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
                                    RedirectUrl = "portal/builder/standard/{{data.id}}",
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/standards/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this standard component?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Standard component has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this Standard component."
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/standards/clone",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200",
                                    JsonBody = "{\r\n  \"cloneId\": \"{{data.id}}\",\r\n  \"cloneName\": \"{{data.name}}_clone\"\r\n}"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to clone this standard?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Standard has been cloned successfully!",
                                    FailedMessage = "Oops! We can't clone this standard."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 3
                        }
                    }
                }
            };

            versionContext.InsertData(dynamicListComponent);
            versionContext.InsertData(standardComponent);

            return Task.CompletedTask;
        }
    }
}
