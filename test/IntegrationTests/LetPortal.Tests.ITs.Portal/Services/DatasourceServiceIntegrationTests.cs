using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
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
            var datasourceService = new DatasourceService(null, null);

            // Act
            var result = await datasourceService.GetDatasourceService(new Datasource
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
            // Arrange
            var mockDatabaseServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaseServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));

            var mongoExtractionDatasource = new MongoExtractionDatasource();
            var datasourceService = new DatasourceService(mockDatabaseServiceProvider.Object, new IExtractionDatasource[] { mongoExtractionDatasource });
            // Act
            var result = await datasourceService.GetDatasourceService(new Datasource
            {
                DatabaseId = "aaa",
                DatasourceType = DatasourceType.Database,
                Query = "{\r\n  \"databases\": { \"name\" : \"testdatabase\" }\r\n}"
            });

            // Assert
            Assert.NotEmpty(result.DatasourceModels);
        }
    }
}
