using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Repositories.Apps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class AppEFRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public AppEFRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Update_Menu_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            AppEFRepository appEFRepository = new AppEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp",
                DisplayName = "Test App"
            };
            await appEFRepository.AddAsync(app);

            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appEFRepository.UpdateMenuAsync(app.Id, menus);

            appEFRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Menu_Profile_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            AppEFRepository appEFRepository = new AppEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp1",
                DisplayName = "Test App"
            };
            await appEFRepository.AddAsync(app);
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appEFRepository.UpdateMenuAsync(app.Id, menus);
            MenuProfile menuProfile = new MenuProfile
            {
                MenuIds = new List<string>
                    {
                        menus[0].Id
                    },
                Role = "Admin"
            };

            await appEFRepository.UpdateMenuProfileAsync(app.Id, menuProfile);

            appEFRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Menu_In_SqlServer_EF_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }

            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            AppEFRepository appEFRepository = new AppEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp",
                DisplayName = "Test App"
            };
            await appEFRepository.AddAsync(app);

            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appEFRepository.UpdateMenuAsync(app.Id, menus);

            appEFRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Menu_Profile_In_SqlServer_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            AppEFRepository appEFRepository = new AppEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp1",
                DisplayName = "Test App"
            };
            await appEFRepository.AddAsync(app);
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appEFRepository.UpdateMenuAsync(app.Id, menus);
            MenuProfile menuProfile = new MenuProfile
            {
                MenuIds = new List<string>
                    {
                        menus[0].Id
                    },
                Role = "Admin"
            };

            await appEFRepository.UpdateMenuProfileAsync(app.Id, menuProfile);

            appEFRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Menu_In_MySql_EF_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            AppEFRepository appEFRepository = new AppEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp",
                DisplayName = "Test App"
            };
            await appEFRepository.AddAsync(app);

            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appEFRepository.UpdateMenuAsync(app.Id, menus);

            appEFRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Menu_Profile_In_MySql_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            AppEFRepository appEFRepository = new AppEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            // Act
            App app = new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp1",
                DisplayName = "Test App"
            };
            await appEFRepository.AddAsync(app);
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = "Core"
                }
            };

            await appEFRepository.UpdateMenuAsync(app.Id, menus);
            MenuProfile menuProfile = new MenuProfile
            {
                MenuIds = new List<string>
                    {
                        menus[0].Id
                    },
                Role = "Admin"
            };

            await appEFRepository.UpdateMenuProfileAsync(app.Id, menuProfile);

            appEFRepository.Dispose();
            // Assert
            Assert.True(true);
        }
    }
}
