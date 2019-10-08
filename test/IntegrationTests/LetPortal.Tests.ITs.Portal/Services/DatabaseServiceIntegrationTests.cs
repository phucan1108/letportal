using LetPortal.Portal.Executions;
using LetPortal.Portal.Services.Databases;
using System.Threading.Tasks;
using Xunit;
using LetPortal.Core.Extensions;
using LetPortal.Core.Utils;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.PostgreSql;
using System.Collections.Generic;
using LetPortal.Portal.Models.Databases;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class DatabaseServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public DatabaseServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Execute_Dynamic_Query_Command_In_MongoDb()
        {
            // Arrange
            var mongoExecutionDatabase = new MongoExecutionDatabase();
            var mongoDatabaseConnection = _context.MongoDatabaseConenction;
            var databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);
            var formattedQueryString = "{\"$query\":{\"databases\":[{\"$match\":{\"_id\":\"ObjectId('" + _context.MongoDatabaseConenction.Id + "')\"}}]}}";
            // Act
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedQueryString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Insert_In_MongoDb()
        {
            // Arrange
            var mongoExecutionDatabase = new MongoExecutionDatabase();
            var mongoDatabaseConnection = _context.MongoDatabaseConenction;
            var databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            // fake insert data
            var cloneConnection = mongoDatabaseConnection.Copy();
            cloneConnection.Id = "";
            var insertingDataString = ConvertUtil.SerializeObject(cloneConnection, true);
            var formattedInsertString = "{\"$insert\":{\"databases\":{ \"$data\": " + insertingDataString + "}}}";
            // Act
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedInsertString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Update_In_MongoDb()
        {
            // Arrange
            var mongoExecutionDatabase = new MongoExecutionDatabase();
            var mongoDatabaseConnection = _context.MongoDatabaseConenction;
            var databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            var formattedUpdateString = "{\"$update\":{\"databases\":{\"$data\": { \"displayName\" : \"Test2 Database\" },\"$where\":{\"_id\":\"ObjectId('" + mongoDatabaseConnection.Id + "')\"}}}}";
            // Act
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedUpdateString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Delete_In_MongoDb()
        {
            // Arrange
            var mongoExecutionDatabase = new MongoExecutionDatabase();
            var mongoDatabaseConnection = _context.MongoDatabaseConenction;
            var databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            // fake insert data
            var cloneConnection = mongoDatabaseConnection.Copy();
            cloneConnection.Id = "";
            var insertingDataString = ConvertUtil.SerializeObject(cloneConnection, true);
            var formattedInsertString = "{\"$insert\":{\"databases\":{ \"$data\": " + insertingDataString + "}}}";
            var insertResult = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedInsertString, new List<ExecuteParamModel>());

            var formattedQueryString = "{\"$delete\":{\"databases\":{\"$where\":{\"_id\":\"ObjectId('" + insertResult.Result.Id + "')\"}}}}";
            // Act
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedQueryString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Extract_Schema_From_Query_In_Postgre_Test()
        {
            // Arrange
            var postgreExtractionDatabase = new PostgreExtractionDatabase();

            var databaseService = new DatabaseService(null, new List<IExtractionDatabase> { postgreExtractionDatabase });
            // Act
            var warpQuery = "Select * From \"Databases\"";
            var result = await databaseService.ExtractColumnSchema(_context.PostgreSqlDatabaseConnection, warpQuery);

            // Assert
            Assert.NotEmpty(result.ColumnFields);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_In_Postgre_Test()
        {
            // Arrange
            var postgreExecutionDatabase = new PostgreExecutionDatabase();

            var databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            var result = await databaseService.ExecuteDynamic(_context.PostgreSqlDatabaseConnection, "Select * from \"Databases\"", null);

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_With_Params_In_Postgre_Test()
        {
            // Arrange
            var postgreExecutionDatabase = new PostgreExecutionDatabase();

            var databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            var result = await databaseService.ExecuteDynamic(_context.PostgreSqlDatabaseConnection, "Select * from \"Databases\" Where \"Name\"={{data.name}}", new List<ExecuteParamModel> { new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" } });

            // Assert
            Assert.NotEmpty(result.Result);
        }
    }
}
