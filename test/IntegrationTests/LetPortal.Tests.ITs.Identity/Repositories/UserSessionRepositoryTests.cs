using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Repositories.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Identity.Repositories
{
    public class UserSessionRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public UserSessionRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Log_User_Action_Activity_Mongo_Test()
        {
            // Arrange
            DatabaseOptions databaseOptions = _context.MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            UserSessionMongoRepository userSessionRepository = new UserSessionMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));

            // Act
            UserSession userSession = new UserSession
            {
                Id = DataUtil.GenerateUniqueId(),
                UserId = "5c06a15e4cc9a850bca44488",
                RequestIpAddress = "127.0.0.1",
                UserActivities = new List<UserActivity>(),
                SignInDate = DateTime.UtcNow
            };
            await userSessionRepository.AddAsync(userSession);
            await userSessionRepository.LogUserActivityAsync(userSession.Id, new UserActivity
            {
                Content = "Abc",
                ActivityDate = DateTime.UtcNow,
                ActivityName = "Test",
                ActivityType = ActivityType.Info
            });

            userSessionRepository.Dispose();
            // Assert
            Assert.True(true);
        }
    }
}
