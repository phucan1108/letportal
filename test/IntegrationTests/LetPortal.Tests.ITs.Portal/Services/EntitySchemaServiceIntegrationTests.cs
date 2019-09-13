using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Services.EntitySchemas;
using Moq;
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
            // Arrange
            var mockDatabaserServiceProvider = new Mock<IDatabaseServiceProvider>();
            mockDatabaserServiceProvider
                .Setup(a => a.GetOneDatabaseConnectionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_context.MongoDatabaseConenction));

            var entitySchemaService = new EntitySchemaService(mockDatabaserServiceProvider.Object);

            // Act
            var result = await entitySchemaService.FetchAllEntitiesFromDatabase("aaa");

            // Assert
            Assert.NotEmpty(result);
        }
    }
}
