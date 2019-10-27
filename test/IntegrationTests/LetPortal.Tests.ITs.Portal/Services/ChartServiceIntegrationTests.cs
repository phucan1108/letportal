using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Services.Components;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class ChartServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public ChartServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        #region UTs for MySQL
        [Fact]
        public async Task Extraction_Chart_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MySqlDatabaseConnection));
            var extractionCharts = new List<IExtractionChartQuery>
            {
                new MySqlExtractionChartQuery()
            };
            var chartService = new ChartService(mockDatabaseServiceProvider.Object, extractionCharts, null);
            // Act
            var result = await chartService.Extract(
                new LetPortal.Portal.Models.Charts.ExtractingChartQueryModel
                {
                    DatabaseId = "asda",
                    Parameters = new List<LetPortal.Portal.Models.Charts.FilledParameterModel>
                    {
                       new LetPortal.Portal.Models.Charts.FilledParameterModel
                       {
                           Name = "queryparams.name",
                           Value = "testdatabase"
                       }     
                    },
                    Query = "SELECT name, `displayName`, `timeSpan` FROM `databases` Where name={{queryparams.name}}"
                });
            // Assert
            Assert.NotEmpty(result.Filters);
        }

        [Fact]
        public async Task Execution_Chart_Report_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
            var mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MySqlDatabaseConnection));
            var executionCharts = new List<IExecutionChartReport>()
            {
               new MySqlExecutionChartReport()
            };
            var chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            var result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.DatabaseOptions
                {
                    Query = "SELECT name, `displayName`, `timeSpan`, `dateCreated`, `dateModified` FROM `apps`"
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=dateCreated;group=name"
                }
            }, new LetPortal.Portal.Models.Charts.ExecutionChartRequestModel
            {
                ChartId = "asdas",
                ChartFilterValues = new List<LetPortal.Portal.Models.Charts.ChartFilterValue>
                {
                    new LetPortal.Portal.Models.Charts.ChartFilterValue
                    {
                        FilterType = LetPortal.Portal.Entities.Components.FilterType.NumberPicker,
                        Name = "timeSpan",
                        IsMultiple = true,
                        Value = "['1000-837076877586810630','10000-737076877586810630']"
                    },
                    new LetPortal.Portal.Models.Charts.ChartFilterValue
                    {
                        FilterType = LetPortal.Portal.Entities.Components.FilterType.DatePicker,
                        Name = "dateCreated",
                        IsMultiple = true,
                        Value = "['2019-10-25T11:59:45.121Z','2019-10-26T11:59:45.121Z']"
                    }
                }
            });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        #endregion
    }
}
