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
                      ? "(SELECT 'Success' as `name`, h.successRequests as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceName={{queryparams.service}} order by h.meansureDateTime desc limit 1) UNION ALL (SELECT 'Fail' as `name`, h.failedRequests as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceName={{queryparams.service}} order by h.meansureDateTime desc limit 1) UNION ALL (SELECT 'Total' as `name`, h.totalRequestsPerDay as `value` FROM httpcounters h join monitorcounters m on h.monitorCounterId = m.id where m.serviceName={{queryparams.service}} order by h.meansureDateTime desc limit 1)"
                      : "(SELECT \"Success\" as \"name\", h.\"successRequests\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceName\"={{queryparams.service}} order by h.\"meansureDateTime\" desc limit 1) UNION ALL (SELECT \"Fail\" as \"name\", h.\"failedRequests\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceName\"={{queryparams.service}} order by h.\"meansureDateTime\" desc limit 1) UNION ALL (SELECT \"Total\" as \"name\", h.\"totalRequestsPerDay\" as \"value\" FROM httpcounters h join monitorcounters m on h.\"monitorCounterId\" = m.id where m.\"serviceName\"={{queryparams.service}} order by h.\"meansureDateTime\" desc limit 1)"
                },
                Definitions = new ChartDefinitions
                {
                    ChartTitle = "HTTP Counters",
                    ChartType = ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=value"
                },
                Options = Constants.ChartOptions,
                TimeSpan = DateTime.UtcNow.Ticks
            };

            versionContext.InsertData(httpCounterChart);
        }
    }
}
