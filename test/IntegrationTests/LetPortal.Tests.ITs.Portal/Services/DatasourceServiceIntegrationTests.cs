using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Services.Datasources;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class DatasourceServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public DatasourceServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Fetch_Datasource_In_Static_Json_Test()
        {
            // Arrange
            DatasourceService datasourceService = new DatasourceService(null, null);

            // Act
            LetPortal.Portal.Models.ExecutedDataSourceModel result = await datasourceService.GetDatasourceService(new Datasource
            {
                DatasourceType = DatasourceType.Static,
                Query = "[{\"name\":\"MongoDB\",\"value\":\"mongodb\"},{\"name\":\"SQL Server\",\"value\":\"sqlserver\"}]"
            });

            // Assert
            Assert.NotEmpty(result.DatasourceModels);
        }

        [Fact]
        public async Task Fetch_Datasource_In_Mongo_Test()
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

            MongoExtractionDatasource mongoExtractionDatasource = new MongoExtractionDatasource();
            DatasourceService datasourceService = new DatasourceService(mockDatabaseServiceProvider.Object, new IExtractionDatasource[] { mongoExtractionDatasource });
            // Act
            LetPortal.Portal.Models.ExecutedDataSourceModel result = await datasourceService.GetDatasourceService(new Datasource
            {
                DatabaseId = "aaa",
                DatasourceType = DatasourceType.Database,
                Query = "{\r\n  \"databases\": { \"name\" : \"testdatabase\" }\r\n}"
            });

            // Assert
            Assert.NotEmpty(result.DatasourceModels);
        }

        [Fact]
        public async Task Fetch_Datasource_In_Postgre_Test()
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

            PostgreExtractionDatasource postgreExtractionDatasource = new PostgreExtractionDatasource();
            DatasourceService datasourceService = new DatasourceService(mockDatabaseServiceProvider.Object, new IExtractionDatasource[] { postgreExtractionDatasource });
            // Act
            LetPortal.Portal.Models.ExecutedDataSourceModel result = await datasourceService.GetDatasourceService(new Datasource
            {
                DatabaseId = "aaa",
                DatasourceType = DatasourceType.Database,
                Query = "Select \"name\", \"id\" as \"Value\" from databases"
            });

            // Assert
            Assert.NotEmpty(result.DatasourceModels);
        }

        [Fact]
        public async Task Fetch_Datasource_In_Sql_Server_Test()
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

            SqlServerExtractionDatasource sqlServerExtractionDatasource = new SqlServerExtractionDatasource();
            DatasourceService datasourceService = new DatasourceService(mockDatabaseServiceProvider.Object, new IExtractionDatasource[] { sqlServerExtractionDatasource });
            // Act
            LetPortal.Portal.Models.ExecutedDataSourceModel result = await datasourceService.GetDatasourceService(new Datasource
            {
                DatabaseId = "aaa",
                DatasourceType = DatasourceType.Database,
                Query = "Select \"name\", \"id\" as \"Value\" from databases"
            });

            // Assert
            Assert.NotEmpty(result.DatasourceModels);
        }

        [Fact]
        public async Task Fetch_Datasource_In_MySQL_Test()
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

            MySqlExtractionDatasource mysqlExtractionDatasource = new MySqlExtractionDatasource();
            DatasourceService datasourceService = new DatasourceService(mockDatabaseServiceProvider.Object, new IExtractionDatasource[] { mysqlExtractionDatasource });
            // Act
            LetPortal.Portal.Models.ExecutedDataSourceModel result = await datasourceService.GetDatasourceService(new Datasource
            {
                DatabaseId = "aaa",
                DatasourceType = DatasourceType.Database,
                Query = "Select name, id as Value from `databases`"
            });

            // Assert
            Assert.NotEmpty(result.DatasourceModels);
        }
    }
}
