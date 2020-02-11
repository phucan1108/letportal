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
            DatabaseOptions databaseOptions = _context.MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            RoleMongoRepository roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));

            // Act             
            LetPortal.Identity.Entities.Role result = await roleRepository.GetByNameAsync("SuperAdmin");
            roleRepository.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Get_Base_Claims_Mongo_Test()
        {
            // Arrange
            DatabaseOptions databaseOptions = _context.MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            RoleMongoRepository roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));

            // Act             
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LetPortal.Identity.Entities.BaseClaim>> result = await roleRepository.GetBaseClaims(new string[] { "SuperAdmin" });
            roleRepository.Dispose();
            // Assert
            Assert.NotNull(result);
        }
    }
}
