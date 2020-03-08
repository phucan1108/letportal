using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Repositories.Apps;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class AppRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public AppRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Update_Menu_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            DatabaseOptions databaseOptions = new DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            AppMongoRepository appMongoRepository = new AppMongoRepository(new Core.Persistences.MongoConnection(databaseOptionsMock.CurrentValue));

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp123",
                DisplayName = "Test App"
            };
            await appMongoRepository.AddAsync(app);

            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appMongoRepository.UpdateMenuAsync(app.Id, menus);

            appMongoRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Menu_Profile_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            DatabaseOptions databaseOptions = new DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            AppMongoRepository appMongoRepository = new AppMongoRepository(new Core.Persistences.MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp1",
                DisplayName = "Test App"
            };
            await appMongoRepository.AddAsync(app);
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appMongoRepository.UpdateMenuAsync(app.Id, menus);
            MenuProfile menuProfile = new MenuProfile
            {
                MenuIds = new List<string>
                    {
                        menus[0].Id
                    },
                Role = "Admin"
            };

            await appMongoRepository.UpdateMenuProfileAsync(app.Id, menuProfile);

            appMongoRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Compare_App_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            DatabaseOptions databaseOptions = new DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            AppMongoRepository appMongoRepository = new AppMongoRepository(new Core.Persistences.MongoConnection(databaseOptionsMock.CurrentValue));

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp123456",
                DisplayName = "Test App"
            };
            await appMongoRepository.AddAsync(app);

            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            app.Menus = menus;
            ComparisonResult result = await appMongoRepository.Compare(app);

            appMongoRepository.Dispose();
            // Assert
            Assert.True(result.Result.Properties.First(a => a.Name == "Menus").ComparedState == ComparedState.Changed);
        }
    }
}
