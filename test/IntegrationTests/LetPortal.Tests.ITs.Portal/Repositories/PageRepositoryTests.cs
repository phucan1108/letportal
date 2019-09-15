using LetPortal.Core.Persistences;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Repositories.Pages;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class PageRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public PageRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Get_One_By_Name_In_Mongo_Test()
        {
            // Arrange
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            var pageRepository = new PageMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            var pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);

            var result = await pageRepository.GetOneByNameAsync("page-builder");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Get_All_Short_Pages_In_Mongo_Test()
        {
            // Arrange
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            var pageRepository = new PageMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            var pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder-1",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);

            var result = await pageRepository.GetAllShortPagesAsync();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_Short_Portal_Claims_Model_In_Mongo_Test()
        {
            // Arrange
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = _context.MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = _context.MongoDatabaseConenction.DataSource
            };
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            var pageRepository = new PageMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            var pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder-2",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);

            var result = await pageRepository.GetShortPortalClaimModelsAsync();

            // Assert
            Assert.NotEmpty(result);
        }
    }
}
