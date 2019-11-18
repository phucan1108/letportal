using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Versions.Components
{
    public class Chart_0_0_1 : IVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Chart>("5dc786a40f4b6b13e0a909f3");
            versionContext.DeleteData<Chart>("5dd02c6d1558c56c40d795e7");
            versionContext.DeleteData<Chart>("5dd2a66d5aa5f917603f05c5");
            versionContext.DeleteData<Chart>("5dd2a66d5aa5f917603f05c6");
            versionContext.DeleteData<Chart>("5dd2a66d5aa5f917603f05ca");
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var httpCounterChart = new Chart
            {
                Id = "5dc786a40f4b6b13e0a909f3",
                Name = "httpcounters",
                DisplayName = "HTTP Counters",
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query = 
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL 
                      ? "(SELECT 'Success' as `name`, h.successRequests as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1) UNION ALL (SELECT 'Fail' as `name`, h.failedRequests as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1) UNION ALL (SELECT 'Total' as `name`, h.totalRequestsPerDay as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceId={{queryparams.serviceId}} order by h.meansureDate desc limit 1)"
                      : "(SELECT \"Success\" as \"name\", h.\"successRequests\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1) UNION ALL (SELECT \"Fail\" as \"name\", h.\"failedRequests\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1) UNION ALL (SELECT \"Total\" as \"name\", h.\"totalRequestsPerDay\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceId\"={{queryparams.serviceId}} order by h.\"meansureDate\" desc limit 1)"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "HTTP Counters",
                    ChartType = ChartType.NumberCard,
                    MappingProjection = "name=name;value=value"
                },
                Options = Constants.ChartOptions(),
                TimeSpan = DateTime.UtcNow.Ticks
            };

            var hardwareCounterChart = new Chart
            {
                Id = "5dd02c6d1558c56c40d795e7",
                Name = "hardwarecounters",
                DisplayName = "Hardware Counters",
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
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
                TimeSpan = DateTime.UtcNow.Ticks
            };

            var hardwareCPURealTimeChart = new Chart
            {
                Id = "5dd2a66d5aa5f917603f05c5",
                Name = "hardwarecpurealtime",
                DisplayName = "CPU Monitor",
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.FourColumns,
                DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                      ? "SELECT 'CPU Usage(%)' as `group`, reportedDate as name, cpuUsage as value FROM monitorhardwarereports WHERE `serviceId`={{queryparams.serviceId}} order by reportedDate desc limit 60"
                      : "SELECT 'CPU Usage(%)' as \"group\", \"reportedDate\" as name, \"cpuUsage\" as value FROM monitorhardwarereports WHERE \"serviceId\"={{queryparams.serviceId}} order by \"reportedDate\" desc limit 60"
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

            var hardwareMemoryRealTimeChart = new Chart
            {
                Id = "5dd2a66d5aa5f917603f05c8",
                Name = "hardwarememoryrealtime",
                DisplayName = "Memory Monitor",
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.FourColumns,
                DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                     versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                     ? "SELECT 'Memory Usage(Mb)' as `group`,reportedDate as name ,  round(`memoryUsed` / 1024, 0) as value FROM monitorhardwarereports WHERE `serviceId`={{queryparams.serviceId}} order by reportedDate desc limit 60"
                     : "SELECT 'Memory Usage(Mb)' as \"group\", \"reportedDate\" as name , round(\"memoryUsed\" / 1024, 0) as value FROM monitorhardwarereports WHERE \"serviceId\"={{queryparams.serviceId}} order by \"reportedDate\" desc limit 60"
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

            var httpRealTimeChart = new Chart
            {
                Id = "5dd2a66d5aa5f917603f05c6",
                Name = "httprealtime",
                DisplayName = "HTTP Real Time Monitor",
                LayoutType = Portal.Entities.SectionParts.PageSectionLayoutType.TwoColumns,
                DatabaseOptions = new Portal.Entities.Shared.DatabaseOptions
                {
                    DatabaseConnectionId = Constants.ServiceManagementDatabaseId,
                    Query =
                      versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL
                      ? "(SELECT 'Success Requests' as `group`, reportedDate as name, successRequests as value FROM monitorhttpreports WHERE `serviceId`={{queryparams.serviceId}} order by reportedDate desc limit 60) UNION ALL (SELECT 'Failed Requests' as `group`, reportedDate as name , failRequests as value FROM monitorhttpreports WHERE `serviceId`={{queryparams.serviceId}} order by reportedDate desc limit 60) UNION ALL (SELECT 'Total Requests' as `group`, reportedDate as name , totalRequests as value FROM monitorhttpreports WHERE `serviceId`={{queryparams.serviceId}} order by reportedDate desc limit 60)"
                      : "(SELECT 'Success Requests' as \"group\", \"reportedDate\" as name, \"successRequests\" as value FROM monitorhttpreports WHERE \"serviceId\"={{queryparams.serviceId}} order by \"reportedDate\" desc limit 60) UNION ALL (SELECT 'Failed Requests' as \"group\", \"reportedDate\" as name , \"failRequests\" as value FROM monitorhttpreports WHERE \"serviceId\"={{queryparams.serviceId}} order by \"reportedDate\" desc limit 60) UNION ALL (SELECT 'Total Requests' as \"group\", \"reportedDate\" as name , \"totalRequests\" as value FROM monitorhttpreports WHERE \"serviceId\"={{queryparams.serviceId}} order by \"reportedDate\" desc limit 60)"
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

            versionContext.InsertData(httpCounterChart);
            versionContext.InsertData(hardwareCounterChart);

            versionContext.InsertData(hardwareCPURealTimeChart);
            versionContext.InsertData(hardwareMemoryRealTimeChart);
            versionContext.InsertData(httpRealTimeChart);
        }
    }
}
