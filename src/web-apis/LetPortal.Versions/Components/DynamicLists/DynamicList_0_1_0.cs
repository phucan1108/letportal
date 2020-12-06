using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.SectionParts.DynamicLists
{
    public class DynamicList_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5d0f09de62c8371c183c8c6f");
            versionContext.DeleteData<DynamicList>("5d0f2dca6ba2fd4ca49e3741");
            versionContext.DeleteData<DynamicList>("5d0f2dca6ba2fd4ca49e3742");
            versionContext.DeleteData<DynamicList>("5d0f2dca6ba2fd4ca49e3743");
            versionContext.DeleteData<DynamicList>("5d0f2dca6ba2fd4ca49e3746");

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var databaseListSectionPart = new DynamicList
            {
                Id = "5d0f09de62c8371c183c8c6f",
                Name = "databasesList",
                DisplayName = "Databases List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        EntityName = "databases",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB
                        ? "{ \"$query\": { \"databases\": [ ] } }"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * From `databases`" : "Select * From databases")
                    },
                    SourceType = DynamicListSourceType.Database
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
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/database-form"
                                }
                            },
                            CommandPositionType = CommandPositionType.OutList,
                            Order = 0
                        },
                        new CommandButtonInList
                        {
                            Name = "edit",
                            DisplayName = "Edit",
                            Icon = "create",
                            Color = "primary",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/database-form?id={{data.id}}"
                                }
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/databases/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this database?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Database has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this database."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 2
                        },
                        new CommandButtonInList
                        {
                            Name = "flush",
                            DisplayName = "Flush",
                            Icon = "sync",
                            Color = "accent",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.CallHttpService,
                                HttpServiceOptions = new HttpServiceOptions
                                {
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/entity-schemas/flush",
                                    JsonBody = "{\"databaseId\":\"{{data.id}}\",\"keptSameName\":true}",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to flush this database?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Database has been flushed successfully!",
                                    FailedMessage = "Oops! We can't flush this database."
                                }
                            },
                            Order = 3
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/databases/clone",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200",
                                    JsonBody = "{\r\n  \"cloneId\": \"{{data.id}}\",\r\n  \"cloneName\": \"{{data.name}}_clone\"\r\n}"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to clone this database?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Database has been cloned successfully!",
                                    FailedMessage = "Oops! We can't clone this database."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 4
                        }
                    }
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
                            DisplayFormat = "{0}",
                            AllowSort = true,
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 1
                        },
                        new ColumnDef
                        {
                            Name = "databaseConnectionType",
                            DisplayName = "Connection Type",
                            DisplayFormat = "{0}",
                            AllowSort = true,
                            DisplayFormatAsHtml = true,
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true,
                                FieldValueType = FieldValueType.Select
                            },
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                                DatasourceStaticOptions = new DatasourceStaticOptions
                                {
                                    JsonResource = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}, {\"name\":\"PostgreSQL\",\"value\":\"postgresql\"}, {\"name\":\"MySQL\",\"value\":\"mysql\"}]"
                                },
                                Type = DatasourceControlType.StaticResource
                            },
                            Order = 2
                        },
                        new ColumnDef
                        {
                            Name = "connectionString",
                            DisplayName = "Connection String",
                            DisplayFormat = "{0}",
                            AllowSort = true,
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 3
                        },
                        new ColumnDef
                        {
                            Name = "dataSource",
                            DisplayName = "Datasource",
                            DisplayFormat = "{0}",
                            AllowSort = true,
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 4
                        }
                    }
                }
            };

            var appListSectionPart = new DynamicList
            {
                Id = "5d0f2dca6ba2fd4ca49e3741",
                Name = "appsList",
                DisplayName = "Apps List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        EntityName = "apps",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                            "{ \"$query\": { \"apps\": [ ] } }"
                            : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `apps`" : "Select * from apps")
                    },
                    SourceType = DynamicListSourceType.Database
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/app-form"
                                }
                            },
                            Order = 0
                        },
                        new CommandButtonInList
                        {
                            Name = "edit",
                            DisplayName = "Edit",
                            Icon = "create",
                            Color = "primary",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType  = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/app-form?id={{data.id}}"
                                }
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/apps/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this app?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "App has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this app."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 2
                        },
                        new CommandButtonInList
                        {
                            Name = "menu",
                            DisplayName = "Menu",
                            Icon = "menu",
                            Color = "primary",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType  = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/app-menu-builder?appId={{data.id}}"
                                }
                            },
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/apps/clone",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200",
                                    JsonBody = "{\r\n  \"cloneId\": \"{{data.id}}\",\r\n  \"cloneName\": \"{{data.name}}_clone\"\r\n}"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to clone this app?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "App has been cloned successfully!",
                                    FailedMessage = "Oops! We can't clone this app."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 4
                        },
                        new CommandButtonInList
                        {
                            Name = "localization",
                            DisplayName = "Localization",
                            Icon = "translate",
                            Color = "warn",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType  = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/localization-management?appId={{data.id}}"
                                }
                            },
                            Order = 5
                        },
                        new CommandButtonInList
                        {
                            Name = "package",
                            DisplayName = "Package",
                            Icon = "archive",
                            Color = "accent",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType  = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/builder/app-package/{{data.id}}"
                                }
                            },
                            Order = 6
                        },
                        new CommandButtonInList
                        {
                            Name = "install",
                            DisplayName = "Install",
                            Icon = "get_app",
                            Color = "accent",
                            CommandPositionType = CommandPositionType.OutList,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType  = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/builder/app-installation"
                                }
                            },
                            Order = 7
                        }
                    }
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
                            DisplayFormat = "{0}",
                            AllowSort = true,
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 1
                        },
                        new ColumnDef
                        {
                            Name = "currentVersionNumber",
                            DisplayName = "Version",
                            DisplayFormat = "{0}",
                            AllowSort = true,
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
                        },
                        new ColumnDef
                        {
                            Name = "defaultUrl",
                            DisplayName = "Default Url",
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 3
                        },
                        new ColumnDef
                        {
                            Name = "createdDate",
                            DisplayName = "Created Date",
                            DisplayFormat = "{0:dd/MM/yyyy}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true,
                                FieldValueType = FieldValueType.DatePicker
                            },
                            Order = 4
                        },
                        new ColumnDef
                        {
                            Name = "modifiedDate",
                            DisplayName = "Modified Date",
                            DisplayFormat = "{0:dd/MM/yyyy}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true,
                                FieldValueType = FieldValueType.DatePicker
                            },
                            Order = 5
                        }
                    }
                }
            };

            var pageListSectionPart = new DynamicList
            {
                Id = "5d0f2dca6ba2fd4ca49e3742",
                Name = "pagesList",
                DisplayName = "Pages List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.PortalDatabaseId,
                        EntityName = "apps",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"pages\": [ ] } }"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `pages`" : "Select * from pages")
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
                            Name = "urlPath",
                            DisplayName = "Url",
                            AllowSort = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
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
                            Order = 3
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/builder/page",
                                    IsSameDomain = true
                                }
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/builder/page/{{data.name}}",
                                    IsSameDomain = true
                                }
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/pages/{{data.id}}",
                                    HttpMethod = "DELETE",
                                    HttpSuccessCode = "200"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to delete this page?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Page has been deleted successfully!",
                                    FailedMessage = "Oops! We can't delete this page."
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
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/pages/clone",
                                    HttpMethod = "POST",
                                    HttpSuccessCode = "200",
                                    JsonBody = "{\r\n  \"cloneId\": \"{{data.id}}\",\r\n  \"cloneName\": \"{{data.name}}_clone\"\r\n}"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to clone this page?"
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Page has been cloned successfully!",
                                    FailedMessage = "Oops! We can't clone this page."
                                }
                            },
                            AllowRefreshList = true,
                            Order = 3
                        }
                    }
                }
            };

            var userListSectionPart = new DynamicList
            {
                Id = "5d0f2dca6ba2fd4ca49e3743",
                Name = "usersList",
                DisplayName = "Users List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.IdentityDatabaseId,
                        EntityName = "users",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"users\": [ ] } }"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `users`" : "Select * from users")
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
                            Name = "username",
                            DisplayName = "Username",
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
                            Name = "email",
                            DisplayName = "Email",
                            AllowSort = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
                        },
                        new ColumnDef
                        {
                            Name = "roles",
                            DisplayName = "Roles",
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true,
                                FieldValueType = FieldValueType.Select
                            },
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                                Type = DatasourceControlType.Database,
                                DatabaseOptions = new SharedDatabaseOptions
                                {
                                    DatabaseConnectionId = Constants.IdentityDatabaseId,
                                    EntityName = "roles",
                                    Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                                        "{\r\n  \"$query\": {\r\n    \"roles\": [\r\n      {\r\n        \"$project\": {\r\n          \"name\": \"$displayName\",\r\n          \"value\": \"$name\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}"
                                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select `displayName` as name, name as value from `roles`" : "Select \"displayName\" as name, name as value from roles")
                                }
                            },
                            Order = 3
                        },
                        new ColumnDef
                        {
                            Name = "isLockoutEnabled",
                            DisplayName = "Lock",
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true,
                                FieldValueType = FieldValueType.Slide
                            },
                            Order = 4
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/page/add-user-form",
                                    IsSameDomain = true
                                }
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
                                    RedirectUrl = "portal/page/user-form?id={{data.id}}",
                                    IsSameDomain = true
                                }
                            },
                            Order = 1
                        }
                    }
                }
            };

            var roleListSectionPart = new DynamicList
            {
                Id = "5d0f2dca6ba2fd4ca49e3746",
                Name = "rolesList",
                DisplayName = "Roles List",
                AppId = Constants.CoreAppId,
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = Constants.IdentityDatabaseId,
                        EntityName = "roles",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"roles\": [ ] } }"
                        : (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL ? "Select * from `roles`" : "Select * from roles")
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
                            DisplayName = "Role",
                            AllowSort = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 1
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
                                    RedirectUrl = "portal/page/role-form",
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
                                    RedirectUrl = "portal/page/role-form?id={{data.id}}",
                                    IsSameDomain = true
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 1
                        },
                        new CommandButtonInList
                        {
                            Name = "editclaims",
                            DisplayName = "Claims",
                            Color = "accent",
                            Icon = "https",
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/builder/roles/{{data.name}}/claims",
                                    IsSameDomain = true
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 2
                        }
                    }
                }
            };

            versionContext.InsertData(databaseListSectionPart);
            versionContext.InsertData(appListSectionPart);
            versionContext.InsertData(pageListSectionPart);
            versionContext.InsertData(userListSectionPart);
            versionContext.InsertData(roleListSectionPart);

            return Task.CompletedTask;
        }
    }
}
