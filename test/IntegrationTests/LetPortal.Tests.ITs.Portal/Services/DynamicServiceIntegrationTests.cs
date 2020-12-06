using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class DynamicServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public DynamicServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        #region UTs for MongoDB
        [Fact]
        public async Task Fecth_Data_Without_Filter_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));

            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new MongoDynamicListQueryDatabase(optionsMock)
            };

            DynamicList databaseListSectionPart = SampleMongoDynamicList();
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>()
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fecth_Data_With_Filter_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));

            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new MongoDynamicListQueryDatabase(optionsMock)
            };

            DynamicList databaseListSectionPart = SampleMongoDynamicList();
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "name",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }
        #endregion

        #region UTs for Postgre
        [Fact]
        public async Task Fetch_Data_Without_Filter_In_Postgre_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new PostgreDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query = "Select * from databases";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>()
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                },
                TextSearch = null
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_In_Postgre_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new PostgreDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_And_Filled_Params_In_Postgre_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new PostgreDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query
                = "Select * From databases Where name={{data.name}}";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilledParameter
                        {
                            Name = "data.name",
                            Value = "testdatabase"
                        }
                    }
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_And_Words_In_Postgre_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new PostgreDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query
                = "Select * From databases Where name={{data.name}} AND {{SEARCH}} AND {{FILTER}} Order by {{ORDER}} Limit {{PAGENUM}} offset {{PAGESTART}}";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilledParameter
                        {
                            Name = "data.name",
                            Value = "testdatabase"
                        }
                    }
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        #endregion

        #region UTs for Sql Server
        [Fact]
        public async Task Fetch_Data_Without_Filter_In_Sql_Server_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new SqlServerDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query = "Select * from databases";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>()
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                },
                TextSearch = null
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_In_Sql_Server_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new SqlServerDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_And_Filled_Params_In_Sql_Server_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new SqlServerDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query
                = "Select * From databases Where name={{data.name}}";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilledParameter
                        {
                            Name = "data.name",
                            Value = "testdatabase"
                        }
                    }
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_And_Words_In_Sql_Server_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new SqlServerDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query
                = "Select * From databases Where name={{data.name}} AND {{SEARCH}} AND {{FILTER}} Order by {{ORDER}} OFFSET {{PAGESTART}} ROWS FETCH NEXT {{PAGENUM}} ROWS ONLY";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilledParameter
                        {
                            Name = "data.name",
                            Value = "testdatabase"
                        }
                    }
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        #endregion

        #region UTs for MySQL
        [Fact]
        public async Task Fetch_Data_Without_Filter_In_MySQL_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new MySqlDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFMySqlDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query = "Select * from `databases`";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>()
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = true,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                },
                TextSearch = null
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_In_MySQL_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new MySqlDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFMySqlDynamicList();
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>()
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_And_Filled_Params_In_MySQL_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new MySqlDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFMySqlDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query
                = "Select * From `databases` Where name={{data.name}}";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilledParameter
                        {
                            Name = "data.name",
                            Value = "testdatabase"
                        }
                    }
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Fetch_Data_With_Filter_And_Words_In_MySQL_Test()
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

            List<IDynamicListQueryDatabase> dynamicListQueries = new List<IDynamicListQueryDatabase>
            {
                new MySqlDynamicListQueryDatabase(
                    new DynamicQueryBuilder(),
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper())
            };

            DynamicList databaseListSectionPart = SampleEFMySqlDynamicList();
            databaseListSectionPart.ListDatasource.DatabaseConnectionOptions.Query
                = "Select * From `databases` Where name={{data.name}} AND {{SEARCH}} AND {{FILTER}} Order by {{ORDER}} LIMIT {{PAGENUM}} OFFSET {{PAGESTART}}";
            DynamicListService dynamicListService = new DynamicListService(mockDatabaseServiceProvider.Object, dynamicListQueries);
            // Act
            LetPortal.Portal.Models.DynamicLists.DynamicListResponseDataModel result = await dynamicListService.FetchData(databaseListSectionPart,
            new LetPortal.Portal.Models.DynamicLists.DynamicListFetchDataModel
            {
                DynamicListId = "sadas",
                FilledParameterOptions = new LetPortal.Portal.Models.DynamicLists.FilledParameterOptions
                {
                    FilledParameters = new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilledParameter
                        {
                            Name = "data.name",
                            Value = "testdatabase"
                        }
                    }
                },
                FilterGroupOptions = new LetPortal.Portal.Models.DynamicLists.FilterGroupOptions
                {
                    FilterGroups = new List<LetPortal.Portal.Models.DynamicLists.FilterGroup>
                    {
                        new LetPortal.Portal.Models.DynamicLists.FilterGroup
                        {
                            FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                            FilterOptions = new List<LetPortal.Portal.Models.DynamicLists.FilterOption>
                            {
                                new LetPortal.Portal.Models.DynamicLists.FilterOption
                                {
                                    FieldName = "name",
                                    FieldValue = "database",
                                    FilterChainOperator = LetPortal.Portal.Models.DynamicLists.FilterChainOperator.None,
                                    FilterOperator = LetPortal.Portal.Models.DynamicLists.FilterOperator.Contains,
                                    FilterValueType = FieldValueType.Text
                                }
                            }
                        }
                    }
                },
                PaginationOptions = new LetPortal.Portal.Models.DynamicLists.PaginationOptions
                {
                    NeedTotalItems = false,
                    PageNumber = 0,
                    PageSize = 10
                },
                SortOptions = new LetPortal.Portal.Models.DynamicLists.SortOptions
                {
                    SortableFields = new List<LetPortal.Portal.Models.DynamicLists.SortableField>
                    {
                        new LetPortal.Portal.Models.DynamicLists.SortableField
                        {
                            FieldName = "displayName",
                            SortType = LetPortal.Portal.Models.DynamicLists.SortType.Desc
                        }
                    }
                },
                TextSearch = "database"
            });

            // Assert
            Assert.NotEmpty(result.Data);
        }

        #endregion

        private DynamicList SampleEFDynamicList()
        {
            return new DynamicList
            {
                Id = "5d0f09de62c8371c183c8c6f",
                Name = "database-list-section-part",
                DisplayName = "Databases List",
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "asda",
                        EntityName = "databases",
                        Query = "Select * from databases"
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/database-form?id={{data.id}}"
                                }
                            },
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
                                }
                            },
                            Order = 3
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
                                    JsonResource = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}]"
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
        }

        private DynamicList SampleEFMySqlDynamicList()
        {
            return new DynamicList
            {
                Id = "5d0f09de62c8371c183c8c6f",
                Name = "database-list-section-part",
                DisplayName = "Databases List",
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "asda",
                        EntityName = "databases",
                        Query = "Select * from `databases`"
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/database-form?id={{data.id}}"
                                }
                            },
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
                                }
                            },
                            Order = 3
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
                                    JsonResource = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}]"
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
        }

        private DynamicList SampleMongoDynamicList()
        {
            return new DynamicList
            {
                Id = "5d0f09de62c8371c183c8c6f",
                Name = "database-list-section-part",
                DisplayName = "Databases List",
                ListDatasource = new DynamicListDatasource
                {
                    DatabaseConnectionOptions = new SharedDatabaseOptions
                    {
                        DatabaseConnectionId = "asda",
                        EntityName = "databases",
                        Query = "{ \"$query\": { \"databases\": [ ] } }"
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
                                ActionType = ActionType.Redirect,
                                RedirectOptions = new RedirectOptions
                                {
                                    IsSameDomain = true,
                                    RedirectUrl = "portal/page/database-form?id={{data.id}}"
                                }
                            },
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
                                }
                            },
                            Order = 3
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
                                    JsonResource = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}]"
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
        }
    }
}
