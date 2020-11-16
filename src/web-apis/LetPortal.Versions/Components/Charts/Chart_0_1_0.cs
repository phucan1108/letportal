using System;
using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Components;

namespace LetPortal.Versions.Components.Charts
{
    public class Chart_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Chart>("5dc786a40f4b6b13e0a909f3");
            versionContext.DeleteData<Chart>("5dd02c6d1558c56c40d795e7");
            versionContext.DeleteData<Chart>("5dd2a66d5aa5f917603f05c5");
            versionContext.DeleteData<Chart>("5dd2a66d5aa5f917603f05c6");
            versionContext.DeleteData<Chart>("5dd2a66d5aa5f917603f05ca");

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var httpCounterChart = new Chart
            {
                Id = "5dc786a40f4b6b13e0a909f3",
                Name = "httpcounters",
                DisplayName = "HTTP Counters",
                AppId = Constants.CoreAppId,
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                      "{\r\n  \"$query\":{\r\n    \"$union\": [{\r\n        \"monitorcounters\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"httpCounter.meansureDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"Success\",\r\n            \"value\": \"$httpCounter.successRequests\"\r\n          } \r\n        },{\r\n          \"$limit\": 1\r\n        }]\r\n      },\r\n      {\r\n        \"monitorcounters\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"httpCounter.meansureDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"Fail\",\r\n            \"value\": \"$httpCounter.failedRequests\"\r\n          } \r\n        },{\r\n          \"$limit\": 1\r\n        }]\r\n      },\r\n      {\r\n        \"monitorcounters\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"httpCounter.meansureDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"Total\",\r\n            \"value\": \"$httpCounter.totalRequestsPerDay\"\r\n          } \r\n        },{\r\n          \"$limit\": 1\r\n        }]\r\n      },\r\n      {\r\n        \"monitorcounters\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"httpCounter.meansureDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"Avg (ms)\",\r\n            \"value\": \"$httpCounter.avgDuration\"\r\n          } \r\n        },{\r\n          \"$limit\": 1\r\n        }]\r\n      }]\r\n  }\r\n}" :
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                      ? "(SELECT 'Success' as `name`, h.successRequests as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1) UNION ALL (SELECT 'Fail' as `name`, h.failedRequests as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1) UNION ALL (SELECT 'Total' as `name`, h.totalRequestsPerDay as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1) UNION ALL (SELECT 'ms' as `name`, round(h.avgDuration,0) as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1)"
                      : "(SELECT \"Success\" as \"name\", h.\"successRequests\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1) UNION ALL (SELECT \"Fail\" as \"name\", h.\"failedRequests\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1) UNION ALL (SELECT \"Total\" as \"name\", h.\"totalRequestsPerDay\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1) UNION ALL (SELECT \"ms\" as \"name\", round(h.\"avgDuration\",0) as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1)"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "HTTP Counters",
                    ChartType = ChartType.NumberCard,
                    MappingProjection = "name=name;value=value"
                },
                Options = Constants.ChartOptions(),
                ChartFilters = new System.Collections.Generic.List<ChartFilter>
                {
                    new ChartFilter
                    {
                        Name = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ? "httpCounter.meansureDate" : "meansureDate",
                        DisplayName = "Counter Date",
                        IsMultiple = true,
                        Type = FilterType.DatePicker
                    }
                },
                TimeSpan = DateTime.UtcNow.Ticks
            };

            var hardwareCounterChart = new Chart
            {
                Id = "5dd02c6d1558c56c40d795e7",
                Name = "hardwarecounters",
                DisplayName = "Hardware Counters",
                AppId = Constants.CoreAppId,
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                      "{\r\n  \"$query\":{\r\n    \"$union\": [{\r\n        \"monitorcounters\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"hardwareCounter.meansureDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"Cpu Usage (%)\",\r\n            \"value\": \"$hardwareCounter.cpuUsage\"\r\n          } \r\n        },{\r\n          \"$limit\": 1\r\n        }]\r\n      },\r\n      {\r\n        \"monitorcounters\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"hardwareCounter.meansureDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"Memory Usage (Mb)\",\r\n            \"value\":  \"$hardwareCounter.memoryUsedInMb\"\r\n          } \r\n        },{\r\n          \"$limit\": 1\r\n        }]\r\n      }]\r\n  }\r\n}" :
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                      ? "(SELECT 'Cpu Usage (%)' as `name`, h.cpuUsage as `value` FROM hardwarecounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1) UNION ALL (SELECT 'Memory Usage (Mb)' as `name`, round(h.memoryUsed / 1024, 0) as `value` FROM hardwarecounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId ={{queryparams.serviceId}} order by h.meansureDate desc limit 1)"
                      : "(SELECT 'Cpu Usage (%)' as name, h.cpuUsage as value FROM hardwarecounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1) UNION ALL (SELECT 'Memory Usage (Mb)' as name, round(h.\"memoryUsed\" / 1024, 0) as value FROM hardwarecounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\" ={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1)"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "Hardware Counters",
                    ChartType = ChartType.NumberCard,
                    MappingProjection = "name=name;value=value"
                },
                Options = Constants.ChartOptions(),
                ChartFilters = new System.Collections.Generic.List<ChartFilter>
                {
                    new ChartFilter
                    {
                        Name = versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ? "hardwareCounter.meansureDate" : "meansureDate",
                        DisplayName = "Counter Date",
                        IsMultiple = true,
                        Type = FilterType.DatePicker
                    }
                },
                TimeSpan = DateTime.UtcNow.Ticks
            };

            var hardwareCPURealTimeChart = new Chart
            {
                Id = "5dd2a66d5aa5f917603f05c5",
                Name = "hardwarecpurealtime",
                DisplayName = "CPU Monitor",
                AppId = Constants.CoreAppId,
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                      "{\r\n  \"$query\":{\r\n    \"monitorhardwarereports\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"reportedDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"$reportedDate\",\r\n            \"value\": \"$cpuUsage\",\r\n            \"group\": \"CPU Usage(%)\"\r\n          } \r\n        },\r\n        {\r\n          \"$limit\": 30\r\n        },\r\n        {\r\n          \"$sort\": {\r\n            \"name\": 1\r\n          }\r\n        }]\r\n  }\r\n}" :
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                      ? "SELECT * FROM (SELECT 'CPU Usage(%)' as `group`, reportedDate as name, cpuUsage as value FROM monitorhardwarereports WHERE `serviceId`={{queryparams.serviceId}} AND {{REAL_TIME}} order by reportedDate desc limit 30) h order by name asc"
                      : "SELECT * FROM (SELECT 'CPU Usage(%)' as \"group\", \"reportedDate\" as name, \"cpuUsage\" as value FROM monitorhardwarereports WHERE \"serviceId\"={{queryparams.serviceId}} AND {{REAL_TIME}} order by \"reportedDate\" desc limit 30) h order by name asc"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "CPU Monitor",
                    ChartType = ChartType.LineChart,
                    MappingProjection = "name=name;value=value;group=group"
                },
                Options = Constants.ChartOptions(),
                TimeSpan = DateTime.UtcNow.Ticks
            };
            hardwareCPURealTimeChart.SetRealTimeField("reportedDate");
            hardwareCPURealTimeChart.SetDataRange("y=[0,100]");
            hardwareCPURealTimeChart.SetXFormatDate("HH:mm");

            var hardwareMemoryRealTimeChart = new Chart
            {
                Id = "5dd2a66d5aa5f917603f05c8",
                Name = "hardwarememoryrealtime",
                DisplayName = "Memory Monitor",
                AppId = Constants.CoreAppId,
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                     versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                     "{\r\n  \"$query\":{\r\n    \"monitorhardwarereports\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"reportedDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"$reportedDate\",\r\n            \"value\": \"$memoryUsedInMb\",\r\n            \"group\": \"Memory Usage(Mb)\"\r\n          } \r\n        },\r\n        {\r\n          \"$limit\": 30\r\n        },\r\n        {\r\n          \"$sort\": {\r\n            \"name\": 1\r\n          }\r\n        }]\r\n  }\r\n}" :
                     versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                     ? "SELECT * FROM (SELECT 'Memory Usage(Mb)' as `group`,reportedDate as name ,  round(`memoryUsed` / 1024, 0) as value FROM monitorhardwarereports WHERE `serviceId`={{queryparams.serviceId}} AND {{REAL_TIME}} order by reportedDate desc limit 30) h order by h.name asc"
                     : "SELECT * FROM (SELECT 'Memory Usage(Mb)' as \"group\", \"reportedDate\" as name , round(\"memoryUsed\" / 1024, 0) as value FROM monitorhardwarereports WHERE \"serviceId\"={{queryparams.serviceId}} AND {{REAL_TIME}} order by \"reportedDate\" desc limit 30) h order by h.name asc"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "Memory Monitor",
                    ChartType = ChartType.LineChart,
                    MappingProjection = "name=name;value=value;group=group"
                },
                Options = Constants.ChartOptions(),
                TimeSpan = DateTime.UtcNow.Ticks
            };
            hardwareMemoryRealTimeChart.SetRealTimeField("reportedDate");
            hardwareMemoryRealTimeChart.SetDataRange("y=[0,2048]");
            hardwareMemoryRealTimeChart.SetXFormatDate("HH:mm");
            var httpRealTimeChart = new Chart
            {
                Id = "5dd2a66d5aa5f917603f05c6",
                Name = "httprealtime",
                DisplayName = "HTTP Real Time Monitor",
                AppId = Constants.CoreAppId,
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.OneColumn,
                DatabaseOptions = new Portal.Entities.Shared.SharedDatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MongoDB ?
                      "{\r\n  \"$query\":{\r\n    \"$union\": [{\r\n        \"monitorhttpreports\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"reportedDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"$reportedDate\",\r\n            \"value\": \"$successRequests\",\r\n            \"group\": \"Success Requests\"\r\n          } \r\n        },{\r\n          \"$limit\": 30\r\n        },{\r\n          \"$sort\": {\r\n            \"name\": 1\r\n          }\r\n        }]\r\n      },\r\n      {\r\n        \"monitorhttpreports\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"reportedDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"$reportedDate\",\r\n            \"value\": \"$failRequests\",\r\n            \"group\": \"Failed Requests\"\r\n          } \r\n        },{\r\n          \"$limit\": 30\r\n        },{\r\n          \"$sort\": {\r\n            \"name\": 1\r\n          }\r\n        }]\r\n      },\r\n      {\r\n        \"monitorhttpreports\": [{\r\n          \"$match\": {\r\n            \"serviceId\": \"{{queryparams.serviceId}}\"\r\n          }\r\n        },{\r\n          \"$sort\": {\r\n            \"reportedDate\": -1\r\n          }\r\n        },{\r\n          \"$project\": {\r\n            \"name\": \"$reportedDate\",\r\n            \"value\": \"$totalRequests\",\r\n            \"group\": \"Total Requests\"\r\n          } \r\n        },{\r\n          \"$limit\": 30\r\n        },{\r\n          \"$sort\": {\r\n            \"name\": 1\r\n          }\r\n        }]\r\n      }]\r\n  }\r\n}" :
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                      ? "SELECT * FROM ((SELECT 'Success Requests' as `group`, reportedDate as name, successRequests as value FROM monitorhttpreports WHERE `serviceId`={{queryparams.serviceId}} AND {{REAL_TIME}} order by reportedDate desc limit 30) UNION ALL (SELECT 'Failed Requests' as `group`, reportedDate as name , failRequests as value FROM monitorhttpreports WHERE `serviceId`={{queryparams.serviceId}} AND {{REAL_TIME}} order by reportedDate desc limit 30) UNION ALL (SELECT 'Total Requests' as `group`, reportedDate as name , totalRequests as value FROM monitorhttpreports WHERE `serviceId`={{queryparams.serviceId}} AND {{REAL_TIME}} order by reportedDate desc limit 30)) h order by h.name asc"
                      : "SELECT * FROM ((SELECT 'Success Requests' as \"group\", \"reportedDate\" as name, \"successRequests\" as value FROM monitorhttpreports WHERE \"serviceId\"={{queryparams.serviceId}} AND {{REAL_TIME}} order by \"reportedDate\" desc limit 30) UNION ALL (SELECT 'Failed Requests' as \"group\", \"reportedDate\" as name , \"failRequests\" as value FROM monitorhttpreports WHERE \"serviceId\"={{queryparams.serviceId}} AND {{REAL_TIME}} order by \"reportedDate\" desc limit 30) UNION ALL (SELECT 'Total Requests' as \"group\", \"reportedDate\" as name , \"totalRequests\" as value FROM monitorhttpreports WHERE \"serviceId\"={{queryparams.serviceId}} AND {{REAL_TIME}} order by \"reportedDate\" desc limit 30)) h order by h.name asc"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "HTTP Real-Time Monitor",
                    ChartType = ChartType.LineChart,
                    MappingProjection = "name=name;value=value;group=group"
                },
                Options = Constants.ChartOptions(),
                TimeSpan = DateTime.UtcNow.Ticks
            };
            httpRealTimeChart.SetRealTimeField("reportedDate");
            httpRealTimeChart.SetXFormatDate("HH:mm");
            httpRealTimeChart.SetDataRange("y=[0,1000]");
            versionContext.InsertData(httpCounterChart);
            versionContext.InsertData(hardwareCounterChart);

            versionContext.InsertData(hardwareCPURealTimeChart);
            versionContext.InsertData(hardwareMemoryRealTimeChart);
            versionContext.InsertData(httpRealTimeChart);

            return Task.CompletedTask;
        }
    }
}
