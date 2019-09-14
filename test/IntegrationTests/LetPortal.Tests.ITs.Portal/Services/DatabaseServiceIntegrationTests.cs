using LetPortal.Portal.Executions;
using LetPortal.Portal.Services.Databases;
using System.Threading.Tasks;
using Xunit;
using LetPortal.Core.Extensions;
using LetPortal.Core.Utils;
using LetPortal.Portal.Executions.Mongo;

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
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedQueryString);

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
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedInsertString);

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
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedUpdateString);

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
            var insertResult = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedInsertString);

            var formattedQueryString = "{\"$delete\":{\"databases\":{\"$where\":{\"_id\":\"ObjectId('" + insertResult.Result.Id + "')\"}}}}";
            // Act
            var result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedQueryString);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
