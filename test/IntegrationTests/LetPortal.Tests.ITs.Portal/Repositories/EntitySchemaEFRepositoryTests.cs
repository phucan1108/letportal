using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Repositories.EntitySchemas;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class EntitySchemaEFRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public EntitySchemaEFRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        #region UTs for Postgre
        [Fact]
        public async Task Get_One_Entity_Schema_By_DB_And_Name_PostgreSQL_Test()
        {
            // Arrange
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            EntitySchemaEFRepository entitySchemaRepository = new EntitySchemaEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
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
        public async Task Update_Entity_Schema_With_Kept_Name_PostgreSQL_Test()
        {
            // Arrange
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            EntitySchemaEFRepository entitySchemaRepository = new EntitySchemaEFRepository(_context.GetPostgreSQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
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
            }, _context.PostgreSqlDatabaseConnection.Id ,true);

            entitySchemaRepository.Dispose();

            // Assert
            Assert.True(true);
        }
        #endregion

        #region UTs for SqlServer
        [Fact]
        public async Task Get_One_Entity_Schema_By_DB_And_Name_SqlServer_Test()
        {
            // Arrange
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            EntitySchemaEFRepository entitySchemaRepository = new EntitySchemaEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
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
        public async Task Update_Entity_Schema_With_Kept_Name_SqlServer_Test()
        {
            // Arrange
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            EntitySchemaEFRepository entitySchemaRepository = new EntitySchemaEFRepository(_context.GetSQLServerContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
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
            }, _context.SqlServerDatabaseConnection.Id, true);

            entitySchemaRepository.Dispose();
            // Assert
            Assert.True(true);
        }
        #endregion

        #region UTs for MySQL
        [Fact]
        public async Task Get_One_Entity_Schema_By_DB_And_Name_MySql_Test()
        {
            // Arrange
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            EntitySchemaEFRepository entitySchemaRepository = new EntitySchemaEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
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
        public async Task Update_Entity_Schema_With_Kept_Name_MySql_Test()
        {
            // Arrange
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
#pragma warning disable CA2000 // Dispose objects before losing scope
            EntitySchemaEFRepository entitySchemaRepository = new EntitySchemaEFRepository(_context.GetMySQLContext());
#pragma warning restore CA2000 // Dispose objects before losing scope
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
            }, _context.MySqlDatabaseConnection.Id, true);

            entitySchemaRepository.Dispose();
            // Assert
            Assert.True(true);
        }

        #endregion
    }
}
