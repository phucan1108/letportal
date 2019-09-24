using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Services.EntitySchemas;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class EntitySchemaServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public EntitySchemaServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Fetch_All_Entities_From_Mongo_Database_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));

            var analyzeDatabases = new List<IAnalyzeDatabase>
            {
                new MongoAnalyzeDatabase()
            };
            var entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object, analyzeDatabases);

            // Act
            var result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Fetch_All_Entities_From_Postgre_Database_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.PostgreSqlDatabaseConnection));

            var analyzeDatabases = new List<IAnalyzeDatabase>
            {
                new PostgreAnalyzeDatabase()
            };
            var entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object, analyzeDatabases);

            // Act
            var result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

            // Assert
            Assert.NotEmpty(result);
        }
    }
}
