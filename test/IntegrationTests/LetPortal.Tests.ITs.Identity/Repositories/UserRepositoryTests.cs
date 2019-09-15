using LetPortal.Core.Persistences;
using LetPortal.Identity.Repositories.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Identity.Repositories
{
    public class UserRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public UserRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Find_User_By_Normalized_Name_Mongo_Test()
        {
            // Arrange
            var databaseOptions = _context.MongoDatabaseOptions;
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var userRepository = new UserMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            var result = await userRepository.FindByNormalizedUsername("ADMIN");

            // Assert
            Assert.NotNull(result);
        }
    }
}
