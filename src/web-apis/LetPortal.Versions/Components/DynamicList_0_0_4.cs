using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Versions.Components
{
    public class DynamicList_0_0_4 : IVersion
    {
        public string VersionNumber => "0.0.4";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5dc786a40f4b6b13e0a909f4");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var serviceMonitorsList = new DynamicList
            {
                Id = "5dc786a40f4b6b13e0a909f4",
                Name = "servicemonitorslist",
                DisplayName = "Service Monitors List",
                Options = Constants.DynamicListOptions,
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
                    {
                        DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                        EntityName = "components",
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL 
                        ? "Select * from `services` Where `serviceState`=1" 
                        : "Select * from services Where \"serviceState\"=1"
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
                            Name = "name",
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
                        new ColumndDef
                        {
                            Name = "instanceNo",
                            DisplayName = "Instance No.",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.Number,
                                AllowTextSearch = false
                            },
                            Order = 2
                        },
                        new ColumndDef
                        {
                            Name = "runningVersion",
                            DisplayName = "Version",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = false
                            },
                            Order = 3
                        },
                        new ColumndDef
                        {
                            Name = "lastCheckingDate",
                            DisplayName = "Last Checking Date",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.DatePicker,
                                AllowTextSearch = false
                            },
                            Order = 4
                        },
                        new ColumndDef
                        {
                            Name = "serviceState",
                            DisplayName = "State",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.Select,
                                AllowTextSearch = false
                            },
                            DatasourceOptions = new DynamicListDatasourceOptions
                            {
                                Type = DatasourceControlType.StaticResource,
                                DatasourceStaticOptions = new DatasourceStaticOptions
                                {
                                    JsonResource = "[\r\n  { \r\n    \"name\":\"Start\",\r\n    \"value\": 0\r\n  },\r\n  {\r\n    \"name\":\"Run\",\r\n    \"value\": 1\r\n  },\r\n  {\r\n    \"name\":\"Shutdown\",\r\n    \"value\": 2\r\n  },\r\n  {\r\n    \"name\":\"Lost\",\r\n    \"value\": 3\r\n  }\r\n]"
                                }
                            },
                            Order = 5
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
                                    RedirectUrl = "portal/builder/chart/{{data.id}}",
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


        }
    }
}
