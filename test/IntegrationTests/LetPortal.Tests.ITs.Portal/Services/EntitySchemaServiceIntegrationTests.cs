using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
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
            Mock<IDatabaseServiceProvider> mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));

            List<IAnalyzeDatabase> analyzeDatabases = new List<IAnalyzeDatabase>
            {
                new MongoAnalyzeDatabase()
            };
            EntitySchemaService entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object, analyzeDatabases);

            // Act
            IEnumerable<LetPortal.Portal.Entities.EntitySchemas.EntitySchema> result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

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
            Mock<IDatabaseServiceProvider> mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.PostgreSqlDatabaseConnection));

            List<IAnalyzeDatabase> analyzeDatabases = new List<IAnalyzeDatabase>
            {
                new PostgreAnalyzeDatabase()
            };
            EntitySchemaService entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object, analyzeDatabases);

            // Act
            IEnumerable<LetPortal.Portal.Entities.EntitySchemas.EntitySchema> result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Fetch_All_Entities_From_Sql_Server_Database_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.SqlServerDatabaseConnection));

            List<IAnalyzeDatabase> analyzeDatabases = new List<IAnalyzeDatabase>
            {
                new SqlServerAnalyzeDatabase()
            };
            EntitySchemaService entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object, analyzeDatabases);

            // Act
            IEnumerable<LetPortal.Portal.Entities.EntitySchemas.EntitySchema> result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Fetch_All_Entities_From_MySQL_Database_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            Mock<IDatabaseServiceProvider> mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MySqlDatabaseConnection));

            List<IAnalyzeDatabase> analyzeDatabases = new List<IAnalyzeDatabase>
            {
                new MySqlAnalyzeDatabase()
            };
            EntitySchemaService entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object, analyzeDatabases);

            // Act
            IEnumerable<LetPortal.Portal.Entities.EntitySchemas.EntitySchema> result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

            // Assert
            Assert.NotEmpty(result);
        }
    }
}
