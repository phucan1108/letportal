using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
using System.Collections.Generic;

namespace LetPortal.Versions.Components
{
    public class DynamicList_0_0_4 : IVersion
    {
        public string VersionNumber => "0.0.4";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DynamicList>("5dc786a40f4b6b13e0a909f4");
            versionContext.DeleteData<DynamicList>("5dcac739be0b4e533408344f");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var serviceMonitorsList = new DynamicList
            {
                Id = "5dc786a40f4b6b13e0a909f4",
                Name = "servicesmonitorlist",
                DisplayName = "Services Monitor List",
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
                    {
                        DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                        Query = 
                        versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                        "{ \"$query\": { \"services\": [{ \r\n \"$match\" : {\r\n  \"$or\": [\r\n    { \"serviceState\" : 1 },\r\n    { \"serviceState\" : 3 }\r\n  ]\r\n}}] }}" : 
                        versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                        ? "SELECT * FROM services s join servicehardwareinfos i on s.id = i.`serviceId` where s.`serviceState` = 1 or s.`serviceState` = 3"
                        : "SELECT * FROM services s join servicehardwareinfos i on s.id = i.\"serviceId\" where s.\"serviceState\" = 1 or s.\"serviceState\" = 3"
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
                            Name = "lastCheckedDate",
                            DisplayName = "Last Checked Date",
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
                            Name = "totalRunningTime",
                            DisplayName = "Total Running Time",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false
                            },
                            Order = 5
                        },
                        new ColumndDef
                        {
                            Name = "serviceState",
                            DisplayName = "State",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            DisplayFormatAsHtml = true,
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
                            Order = 6
                        },
                        new ColumndDef
                        {
                            Name = "ipAddress",
                            DisplayName = "IP Address",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = false
                            },
                            Order = 7
                        },
                        new ColumndDef
                        {
                            Name = versionContext.ConnectionType != Core.Persistences.ConnectionType.MongoDB ? "os" : "servicehardwareinfos.os",
                            DisplayName = "OS",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = false
                            },
                            Order = 8
                        }
                    }
                },
                CommandsList = new CommandsList
                {
                    CommandButtonsInList = new List<CommandButtonInList>
                    {
                        new CommandButtonInList
                        {
                            Name = "dashboard",
                            DisplayName = "Dashboard",
                            Color = "primary",
                            Icon = "developer_board",
                            CommandPositionType = CommandPositionType.InList,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                RedirectOptions = new RedirectOptions
                                {
                                    RedirectUrl = "portal/page/service-dashboard?serviceId={{data.id}}",
                                    IsSameDomain = true,
                                    RedirectType = RedirectType.ThroughUrl
                                },
                                ActionType = ActionType.Redirect
                            },
                            Order = 0
                        }
                    }
                }
            };

            var serviceLogsList = new DynamicList
            {
                Id = "5dcac739be0b4e533408344f",
                Name = "servicelogs",
                DisplayName = "Service Logs List",
                Options = Constants.DynamicListOptions(),
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new DatabaseOptions
                    {
                        DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                        Query = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ? "{ \"$query\": { \"logevents\": [ ] } }" : "SELECT * FROM logevents"
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
                            Name = "traceId",
                            DisplayName = "Trace Id",
                            AllowSort = false,
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
                            Name = "source",
                            DisplayName = "Service Name",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 2
                        },
                        new ColumndDef
                        {
                            Name = "sourceId",
                            DisplayName = "Service Id",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 3
                        },
                        new ColumndDef
                        {
                            Name = "httpRequestUrl",
                            DisplayName = "Request Url",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 4
                        },
                        new ColumndDef
                        {
                            Name = "httpResponseStatusCode",
                            DisplayName = "Response Code",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                AllowTextSearch = true
                            },
                            Order = 5
                        },
                        new ColumndDef
                        {
                            Name = "beginRequest",
                            DisplayName = "Begin Request",
                            AllowSort = true,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = true,
                                FieldValueType = FieldValueType.DatePicker,
                                AllowTextSearch = false
                            },
                            Order = 6
                        },
                        new ColumndDef
                        {
                            Name = "stackTrace",
                            DisplayName = "stackTrace",
                            AllowSort = false,
                            DisplayFormat = "{0}",
                            SearchOptions = new SearchOptions
                            {
                                AllowInAdvancedMode = false,
                                AllowTextSearch = false
                            },                             
                            Order = 7
                        }
                    }
                }   ,
                CommandsList = new CommandsList
                {
                    CommandButtonsInList = new List<CommandButtonInList>
                    {
                        new CommandButtonInList
                        {
                            Name = "collect",
                            DisplayName = "Collect Logs",
                            Icon = "sync",
                            Color = "accent",
                            CommandPositionType = CommandPositionType.InList,
                            AllowRefreshList = true,
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.CallHttpService,
                                HttpServiceOptions = new HttpServiceOptions
                                {
                                    HttpMethod = "GET",
                                    HttpServiceUrl = "{{configs.portalBaseEndpoint}}/api/logs/gather/{{data.traceId}}",
                                    HttpSuccessCode = "200;204"                                    
                                },
                                IsEnable = true,
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "System has collected all logs that belongs to trace id.",
                                    FailedMessage = "Oops! Something went wrong, please check again."
                                }
                            }
                        }
                    }
                }
            };

            versionContext.InsertData(serviceMonitorsList);
            versionContext.InsertData(serviceLogsList);
        }
    }
}
