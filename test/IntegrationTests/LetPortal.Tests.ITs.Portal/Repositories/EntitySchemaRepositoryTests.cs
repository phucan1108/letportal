using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Repositories.EntitySchemas;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class EntitySchemaRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public EntitySchemaRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Get_One_Entity_Schema_By_DB_And_Name_Test()
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
            EntitySchemaMongoRepository entitySchemaRepository = new EntitySchemaMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            EntitySchema entitySchemaTest = new EntitySchema
            {
                Id = DataUtil.GenerateUniqueId(),
                AppId = "abc",
                DatabaseId = "abc",
                Name = "name",
                DisplayName = "Name",
                EntityFields = new List<EntityField>
                {
                    new EntityField
                    {
                        Name = "a",
                        DisplayName = "A",
                        FieldType = "text"
                    }
                }
            };

            await entitySchemaRepository.AddAsync(entitySchemaTest);

            EntitySchema result = await entitySchemaRepository.GetOneEntitySchemaAsync("abc", "name");

            entitySchemaRepository.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_Entity_Schema_With_Kept_Name_Test()
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
            EntitySchemaMongoRepository entitySchemaRepository = new EntitySchemaMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            // Act
            EntitySchema entitySchemaTest = new EntitySchema
            {
                Id = DataUtil.GenerateUniqueId(),
                AppId = "abcd",
                DatabaseId = "abcd",
                Name = "name1",
                DisplayName = "Name",
                EntityFields = new List<EntityField>
                {
                    new EntityField
                    {
                        Name = "a",
                        DisplayName = "A",
                        FieldType = "text"
                    }
                }
            };

            await entitySchemaRepository.AddAsync(entitySchemaTest);

            await entitySchemaRepository.UpsertEntitySchemasAsync(new List<EntitySchema>
            {
                entitySchemaTest
            }, _context.MongoDatabaseConenction.Id, true);

            entitySchemaRepository.Dispose();
            // Assert
            Assert.True(true);
        }
    }
}
