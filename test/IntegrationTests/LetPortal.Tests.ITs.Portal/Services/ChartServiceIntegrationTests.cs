using LetPortal.Core.Persistences;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Mappers.PostgreSql;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Services.Components;
using Microsoft.Extensions.Options;
using Moq;
using System;
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

        #region UTs for Mongo

        [Fact]
        public async Task Execution_Chart_Report_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            // Arrange               
            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new MongoExecutionChartReport(new MongoQueryExecution(optionsMock))
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "{\r\n  \"$query\":{\r\n    \"apps\":[]\r\n  }\r\n}"
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Name = "createdDate",
                        IsMultiple = true,
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                }
            });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        #endregion

        #region UTs for Postgre
        [Fact]
        public async Task Extraction_Chart_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.PostgreSqlDatabaseConnection));
            List<IExtractionChartQuery> extractionCharts = new List<IExtractionChartQuery>
            {
                new PostgreExtractionChartQuery(new PostgreSqlMapper(_context.MapperOptions), new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, extractionCharts, null);
            // Act
            LetPortal.Portal.Models.Charts.ExtractionChartFilter result = await chartService.Extract(
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
                    Query = "SELECT \"name\", \"displayName\", \"timeSpan\" FROM \"databases\" Where name={{queryparams.name}}"
                });
            // Assert
            Assert.NotEmpty(result.Filters);
        }

        [Fact]
        public async Task Execution_Chart_Report_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.PostgreSqlDatabaseConnection));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new PostgreExecutionChartReport(
                   new ChartReportQueryBuilder(),
                   new ChartReportProjection(),
                   new PostgreSqlMapper(_context.MapperOptions),
                   new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "SELECT name, \"displayName\", \"timeSpan\", \"createdDate\", \"modifiedDate\" FROM \"apps\""
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                }
            });
            ;

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execution_Chart_Report_Real_Time_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.PostgreSqlDatabaseConnection));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new PostgreExecutionChartReport(
                   new ChartReportQueryBuilder(),
                   new ChartReportProjection(),
                   new PostgreSqlMapper(_context.MapperOptions),
                   new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "SELECT name, \"displayName\", \"timeSpan\", \"createdDate\", \"modifiedDate\" FROM \"apps\""
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                },
                IsRealTime = true,
                LastRealTimeComparedDate = DateTime.UtcNow.AddDays(-1),
                RealTimeField = "dateCreated"
            });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        #endregion

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
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MySqlDatabaseConnection));
            List<IExtractionChartQuery> extractionCharts = new List<IExtractionChartQuery>
            {
                new MySqlExtractionChartQuery(new MySqlMapper(_context.MapperOptions), new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, extractionCharts, null);
            // Act
            LetPortal.Portal.Models.Charts.ExtractionChartFilter result = await chartService.Extract(
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
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MySqlDatabaseConnection));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new MySqlExecutionChartReport(
                   new ChartReportQueryBuilder(),
                   new ChartReportProjection(),
                   new MySqlMapper(_context.MapperOptions),
                   new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "SELECT name, `displayName`, `timeSpan`, `createdDate`, `modifiedDate` FROM `apps`"
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                }
            });
            ;

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execution_Chart_Report_Real_Time_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MySqlDatabaseConnection));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new MySqlExecutionChartReport(
                   new ChartReportQueryBuilder(),
                   new ChartReportProjection(),
                   new MySqlMapper(_context.MapperOptions),
                   new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "SELECT name, `displayName`, `timeSpan`, `createdDate`, `modifiedDate` FROM `apps`"
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                },
                IsRealTime = true,
                LastRealTimeComparedDate = DateTime.UtcNow.AddDays(-1),
                RealTimeField = "dateCreated"
            });

            // Assert
            Assert.NotEmpty(result.Result);
        }
        #endregion

        #region UTs for Sql Server
        [Fact]
        public async Task Extraction_Chart_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.PostgreSqlDatabaseConnection));
            List<IExtractionChartQuery> extractionCharts = new List<IExtractionChartQuery>
            {
                new PostgreExtractionChartQuery(new PostgreSqlMapper(_context.MapperOptions), new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, extractionCharts, null);
            // Act
            LetPortal.Portal.Models.Charts.ExtractionChartFilter result = await chartService.Extract(
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
                    Query = "SELECT \"name\", \"displayName\", \"timeSpan\" FROM \"databases\" Where name={{queryparams.name}}"
                });
            // Assert
            Assert.NotEmpty(result.Filters);
        }

        [Fact]
        public async Task Execution_Chart_Report_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }

            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.SqlServerDatabaseConnection));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new SqlServerExecutionChartReport(
                   new ChartReportQueryBuilder(),
                   new ChartReportProjection(),
                   new SqlServerMapper(_context.MapperOptions),
                   new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "SELECT name, \"displayName\", \"timeSpan\", \"createdDate\", \"modifiedDate\" FROM \"apps\""
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                }
            });
            ;

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execution_Chart_Report_Real_Time_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }

            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.SqlServerDatabaseConnection));
            List<IExecutionChartReport> executionCharts = new List<IExecutionChartReport>()
            {
               new SqlServerExecutionChartReport(
                   new ChartReportQueryBuilder(),
                   new ChartReportProjection(),
                   new SqlServerMapper(_context.MapperOptions),
                   new CSharpMapper())
            };
            ChartService chartService = new ChartService(mockDatabaseServiceProvider.Object, null, executionCharts);

            // Act
            LetPortal.Portal.Models.Charts.ExecutionChartResponseModel result = await chartService.Execute(new LetPortal.Portal.Entities.Components.Chart
            {
                DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                {
                    Query = "SELECT name, \"displayName\", \"timeSpan\", \"createdDate\", \"modifiedDate\" FROM \"apps\""
                },
                Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions
                {
                    ChartTitle = "aaa",
                    ChartType = LetPortal.Portal.Entities.Components.ChartType.VerticalBarChart,
                    MappingProjection = "name=name;value=createdDate;group=name"
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
                        Value = string.Format("['{0}','{1}']",DateTime.UtcNow.AddDays(-1).ToString("o"), DateTime.UtcNow.AddDays(1).ToString("o"))
                    }
                },
                IsRealTime = true,
                LastRealTimeComparedDate = DateTime.UtcNow.AddDays(-1),
                RealTimeField = "dateCreated"
            });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        #endregion
    }
}
