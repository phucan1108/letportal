using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Repositories.Localizations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class LocalizationRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public LocalizationRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Test_Insert_Localization_PostgreSql()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            var localizationRepository = new LocalizationEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            var localization = new Localization
            {
                Id = DataUtil.GenerateUniqueId(),
                LocaleId = "vi-VN",
                LocalizationContents = new List<LocalizationContent>
                {
                   new LocalizationContent
                   {
                       Id = DataUtil.GenerateUniqueId(),
                       Key = "abc",
                       Text = "xyzya"
                   }
                }
            };

            // Act
            await localizationRepository.AddAsync(localization).ConfigureAwait(false);
            Assert.True(true);
        }

        [Fact]
        public async Task Test_Insert_Localization_MySql()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }

            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            var localizationRepository = new LocalizationEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            var localization = new Localization
            {
                Id = DataUtil.GenerateUniqueId(),
                LocaleId = "vi-VN",
                LocalizationContents = new List<LocalizationContent>
                {
                   new LocalizationContent
                   {
                       Id = DataUtil.GenerateUniqueId(),
                       Key = "abc",
                       Text = "xyzya"
                   }
                }
            };

            // Act
            await localizationRepository.AddAsync(localization).ConfigureAwait(false);
            Assert.True(true);
        }

        [Fact]
        public async Task Test_Insert_Localization_SqlServer()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }

            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            var localizationRepository = new LocalizationEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope

            var localization = new Localization
            {
                Id = DataUtil.GenerateUniqueId(),
                LocaleId = "vi-VN",
                LocalizationContents = new List<LocalizationContent>
                {
                   new LocalizationContent
                   {
                       Id = DataUtil.GenerateUniqueId(),
                       Key = "abc",
                       Text = "xyzya"
                   }
                }
            };

            // Act
            await localizationRepository.AddAsync(localization).ConfigureAwait(false);
            Assert.True(true);
        }

        [Fact]
        public async Task Test_Insert_Localization_MongoDb()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }

            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            var localizationRepository = new LocalizationMongoRepository(_context.GetMongoConnection());
#pragma warning restore CA2000 // Dispose objects before losing scope

            var localization = new Localization
            {
                Id = DataUtil.GenerateUniqueId(),
                LocaleId = "vi-VN",
                LocalizationContents = new List<LocalizationContent>
                {
                   new LocalizationContent
                   {
                       Id = DataUtil.GenerateUniqueId(),
                       Key = "abc",
                       Text = "xyzya"
                   }
                }
            };

            // Act
            await localizationRepository.AddAsync(localization).ConfigureAwait(false);
            Assert.True(true);
        }
    }
}
