using System.Collections.Generic;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Versions.Components
{
    public class DynamicList_0_0_2 : IPortalVersion
    {
        public string VersionNumber => "0.0.2";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5d4d8adfae5f5b68b811ec1d");
            versionContext.DeleteData<DynamicList>("5d4d8adfae5f5b68b811ec1e");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var dynamicListComponent = new DynamicList
            {
                Id = "5d4d8adfae5f5b68b811ec1d",
                Name = "dynamiclistcomponent",
                DisplayName = "Dynamic Lists List",
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
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
                    ColumndDefs = new List<ColumndDef>
                    {
                        new ColumndDef
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
                        new ColumndDef
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
                                    IsSameDomain = true,
                                    RedirectType = RedirectType.ThroughUrl
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
                                    IsSameDomain = true,
                                    RedirectType = RedirectType.ThroughUrl
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 1
                        }
                    }
                }
            };

            var standardComponent = new DynamicList
            {
                Id = "5d4d8adfae5f5b68b811ec1e",
                Name = "standardlistcomponent",
                DisplayName = "Standards List",
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
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
                    ColumndDefs = new List<ColumndDef>
                    {
                        new ColumndDef
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
                        new ColumndDef
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
                                    IsSameDomain = true,
                                    RedirectType = RedirectType.ThroughUrl
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
                                    IsSameDomain = true,
                                    RedirectType = RedirectType.ThroughUrl
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 1
                        }
                    }
                }
            };

            versionContext.InsertData(dynamicListComponent);
            versionContext.InsertData(standardComponent);
        }
    }
}
