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
using System;

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
            var warpQuery = "Select * From databases";
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
            var result = await databaseService.ExecuteDynamic(_context.PostgreSqlDatabaseConnection, "Select * from databases", null);

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
            var result = await databaseService.ExecuteDynamic(_context.PostgreSqlDatabaseConnection, "Select * from {{options.entityname}} Where name={{data.name}}", new List<ExecuteParamModel> { new ExecuteParamModel { Name = "options.entityname", ReplaceValue = "databases" }, new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" } });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Insert_In_Postgre_Test()
        {
            // Arrange
            var postgreExecutionDatabase = new PostgreExecutionDatabase();

            var databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            var result =
                await databaseService.ExecuteDynamic(
                    _context.PostgreSqlDatabaseConnection,
                        "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.name}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}})",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = Guid.NewGuid().ToString() },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase1" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
                            new ExecuteParamModel { Name = "currentTick()", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = "abc"},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = "localhost"},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = "postgresql" }
                        });

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Update_In_Postgre_Test()
        {
            // Arrange
            var postgreExecutionDatabase = new PostgreExecutionDatabase();

            var databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            var result =
                await databaseService.ExecuteDynamic(
                    _context.PostgreSqlDatabaseConnection,
                        "Update databases SET \"displayName\"={{data.displayName}} WHERE id={{data.id}}",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "data.id", ReplaceValue = _context.PostgreSqlDatabaseConnection.Id },                            
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" }
                        });

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Delete_In_Postgre_Test()
        {
            // Arrange
            var postgreExecutionDatabase = new PostgreExecutionDatabase();

            var databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);
            var insertId = Guid.NewGuid().ToString();
            // Act
            await databaseService.ExecuteDynamic(
                    _context.PostgreSqlDatabaseConnection,
                        "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.name}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}})",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = insertId },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase1" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
                            new ExecuteParamModel { Name = "currentTick()", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = "abc"},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = "localhost"},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = "postgresql" }
                        });
            var result =
                await databaseService.ExecuteDynamic(
                    _context.PostgreSqlDatabaseConnection,
                        "DELETE From databases Where id={{data.id}}",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "data.id", ReplaceValue = insertId }                          
                        });

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
