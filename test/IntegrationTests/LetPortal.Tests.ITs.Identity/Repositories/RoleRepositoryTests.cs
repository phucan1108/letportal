using LetPortal.Core.Persistences;
using LetPortal.Identity.Repositories.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Identity.Repositories
{
    public class RoleRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public RoleRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Get_By_Name_Mongo_Test()
        {
            // Arrange
            var databaseOptions = _context.MongoDatabaseOptions;
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));

            // Act             
            var result = await roleRepository.GetByNameAsync("SuperAdmin");
            // Assert
            Assert.NotNull(result);
        } 

        [Fact]
        public async Task Get_Base_Claims_Mongo_Test()
        {
            // Arrange
            var databaseOptions = _context.MongoDatabaseOptions;
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));

            // Act             
            var result = await roleRepository.GetBaseClaims(new string[] { "SuperAdmin" });
            // Assert
            Assert.NotNull(result);
        } 
    }
}
