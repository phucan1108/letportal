using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Mappers.PostgreSql;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Services.Databases;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Services
{
    public class DatabaseServiceIntegrationTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public DatabaseServiceIntegrationTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        #region UTs for MongoDB

        [Fact]
        public async Task Execute_Dynamic_Query_Command_In_MongoDb()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            MongoExecutionDatabase mongoExecutionDatabase = new MongoExecutionDatabase(optionsMock);
            LetPortal.Portal.Entities.Databases.DatabaseConnection mongoDatabaseConnection = _context.MongoDatabaseConenction;
            DatabaseService databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);
            string formattedQueryString = "{\"$query\":{\"databases\":[{\"$match\":{\"_id\":\"ObjectId('" + _context.MongoDatabaseConenction.Id + "')\"}}]}}";
            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedQueryString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Insert_In_MongoDb()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            MongoExecutionDatabase mongoExecutionDatabase = new MongoExecutionDatabase(optionsMock);
            LetPortal.Portal.Entities.Databases.DatabaseConnection mongoDatabaseConnection = _context.MongoDatabaseConenction;
            DatabaseService databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            // fake insert data
            LetPortal.Portal.Entities.Databases.DatabaseConnection cloneConnection = mongoDatabaseConnection.Copy();
            cloneConnection.Id = "";
            string insertingDataString = ConvertUtil.SerializeObject(cloneConnection, true);
            string formattedInsertString = "{\"$insert\":{\"databases\":{ \"$data\": " + insertingDataString + "}}}";
            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedInsertString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Update_In_MongoDb()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            MongoExecutionDatabase mongoExecutionDatabase = new MongoExecutionDatabase(optionsMock);
            LetPortal.Portal.Entities.Databases.DatabaseConnection mongoDatabaseConnection = _context.MongoDatabaseConenction;
            DatabaseService databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            string formattedUpdateString = "{\"$update\":{\"databases\":{\"$data\": { \"displayName\" : \"Test2 Database\" },\"$where\":{\"_id\":\"ObjectId('" + mongoDatabaseConnection.Id + "')\"}}}}";
            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedUpdateString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Delete_In_MongoDb()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            MongoExecutionDatabase mongoExecutionDatabase = new MongoExecutionDatabase(optionsMock);
            LetPortal.Portal.Entities.Databases.DatabaseConnection mongoDatabaseConnection = _context.MongoDatabaseConenction;
            DatabaseService databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            // fake insert data
            LetPortal.Portal.Entities.Databases.DatabaseConnection cloneConnection = mongoDatabaseConnection.Copy();
            cloneConnection.Id = "";
            string insertingDataString = ConvertUtil.SerializeObject(cloneConnection, true);
            string formattedInsertString = "{\"$insert\":{\"databases\":{ \"$data\": " + insertingDataString + "}}}";
            LetPortal.Portal.Models.ExecuteDynamicResultModel insertResult = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedInsertString, new List<ExecuteParamModel>());

            dynamic formattedQueryString = "{\"$delete\":{\"databases\":{\"$where\":{\"_id\":\"ObjectId('" + insertResult.Result.Id + "')\"}}}}";
            // Act
            dynamic result = await databaserService.ExecuteDynamic(mongoDatabaseConnection, formattedQueryString, new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Multiple_Commands_In_MongoDb()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            var optionsMock = Mock.Of<IOptionsMonitor<MongoOptions>>(_ => _.CurrentValue == _context.MongoOptions);
            MongoExecutionDatabase mongoExecutionDatabase = new MongoExecutionDatabase(optionsMock);
            LetPortal.Portal.Entities.Databases.DatabaseConnection mongoDatabaseConnection = _context.MongoDatabaseConenction;
            DatabaseService databaserService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabase }, null);

            var executionChains = new DatabaseExecutionChains
            {
                Steps = new List<DatabaseExecutionStep>()
            };

            // S1: Query
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
               DatabaseConnectionId = _context.MongoDatabaseConenction.Id,
               ExecuteCommand = "{\"$query\":{\"databases\":[{\"$match\":{\"_id\":\"ObjectId('" + _context.MongoDatabaseConenction.Id + "')\"}}]}}"
            });

            // S2: Update
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
               DatabaseConnectionId = _context.MongoDatabaseConenction.Id,
               ExecuteCommand = "{\"$update\":{\"databases\":{\"$data\": { \"displayName\" : \"Test2 Database\" },\"$where\":{\"_id\":\"ObjectId('{{$step1.id}}')\"}}}}"
            });

            // S3: Insert
            // fake insert data
            LetPortal.Portal.Entities.Databases.DatabaseConnection cloneConnection = mongoDatabaseConnection.Copy();
            cloneConnection.Id = "";
            string insertingDataString = ConvertUtil.SerializeObject(cloneConnection, true);            
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MongoDatabaseConenction.Id,
                ExecuteCommand = "{\"$insert\":{\"databases\":{ \"$data\": " + insertingDataString + "}}}"
            });

            // S4: Insert multiple children
            var loopDatas = new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>();
            for(int i = 0; i < 5; i++)
            {
                LetPortal.Portal.Entities.Databases.DatabaseConnection cloneChildConnection = mongoDatabaseConnection.Copy();
                cloneChildConnection.Id = "";
                cloneChildConnection.Name += $"_{i}";
                loopDatas.Add(cloneChildConnection);
            }
            var loopDataModels = new List<LoopDataParamModel>();
            var loopDataModel = new LoopDataParamModel
            {
                Name = "data.databases.inserts",
                Parameters = new List<List<ExecuteParamModel>>()
            };
            foreach(var clone in loopDatas)
            {
                loopDataModel.Parameters.Add(new List<ExecuteParamModel>
                {
                    new ExecuteParamModel 
                        { 
                            Name = "data", 
                            RemoveQuotes = true, 
                            ReplaceValue = ConvertUtil.SerializeObject(cloneConnection, true) 
                        }
                });
            }
            loopDataModels.Add(loopDataModel);
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
               DatabaseConnectionId = _context.MongoDatabaseConenction.Id,
               DataLoopKey = "data.databases.inserts",
               ExecuteCommand = "{\"$insert\":{\"databases\":{ \"$data\": \"{{data}}\"}}}"
            });

            // S5: Delete
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MongoDatabaseConenction.Id,
                ExecuteCommand = "{\"$delete\":{\"databases\":{\"$where\":{\"_id\":\"ObjectId('{{$step3.id}}')\"}}}}"
            });

            // Act
            var result = await databaserService.ExecuteDynamic(
                new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>
                {
                    _context.MongoDatabaseConenction
                },
                executionChains,
                new List<ExecuteParamModel>(),
                loopDataModels
                ).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region UTs for Postgre
        [Fact]
        public async Task Extract_Schema_From_Query_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExtractionDatabase postgreExtractionDatabase = new PostgreExtractionDatabase(
                new PostgreSqlMapper(_context.MapperOptions),
                new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(null, new List<IExtractionDatabase> { postgreExtractionDatabase });
            // Act
            string warpQuery = "Select * From databases";
            ExtractingSchemaQueryModel result = await databaseService.ExtractColumnSchema(_context.PostgreSqlDatabaseConnection, warpQuery, null);

            // Assert
            Assert.NotEmpty(result.ColumnFields);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExecutionDatabase postgreExecutionDatabase =
                new PostgreExecutionDatabase(
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(_context.PostgreSqlDatabaseConnection, "Select * from databases", null);

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_With_Params_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExecutionDatabase postgreExecutionDatabase =
                new PostgreExecutionDatabase(
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(_context.PostgreSqlDatabaseConnection, "Select * from databases Where name={{data.name}}", new List<ExecuteParamModel> { new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" } });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Insert_In_Postgre_Test()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExecutionDatabase postgreExecutionDatabase =
                new PostgreExecutionDatabase(
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.PostgreSqlDatabaseConnection,
                        "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.name}},{{data.displayName}}, {{currentTick()|long}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}})",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = Guid.NewGuid().ToString() },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase1" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
                            new ExecuteParamModel { Name = "currentTick()|long", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
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
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExecutionDatabase postgreExecutionDatabase =
                new PostgreExecutionDatabase(
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
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
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExecutionDatabase postgreExecutionDatabase =
                new PostgreExecutionDatabase(
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);
            string insertId = Guid.NewGuid().ToString();
            // Act
            await databaseService.ExecuteDynamic(
                    _context.PostgreSqlDatabaseConnection,
                        "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.name}},{{data.displayName}}, {{currentTick()|long}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}})",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = insertId },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase1" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
                            new ExecuteParamModel { Name = "currentTick()|long", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = "abc"},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = "localhost"},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = "postgresql" }
                        });
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
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

        [Fact]
        public async Task Execute_Dynamic_Multiple_Commands_In_PostgreSql()
        {
            if(!_context.AllowPostgreSQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            PostgreExecutionDatabase postgreExecutionDatabase =
                new PostgreExecutionDatabase(
                    new PostgreSqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { postgreExecutionDatabase }, null);

            var executionChains = new DatabaseExecutionChains
            {
                Steps = new List<DatabaseExecutionStep>()
            };

            // S1: Query
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.PostgreSqlDatabaseConnection.Id,
                ExecuteCommand = "Select * from databases Where name={{data.name}}"
            });

            // S2: Update
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.PostgreSqlDatabaseConnection.Id,
                ExecuteCommand = "Update databases SET \"displayName\"={{data.updatingDisplayName}} WHERE id={{$step1.id}}"
            });

            // S3: Insert            
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.PostgreSqlDatabaseConnection.Id,
                ExecuteCommand = "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.insertingName}},{{data.displayName}}, {{currentTick()|long}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}});" +
                " Select * from databases where name={{data.insertingName}}"
            });

            // S4: Insert multiple children
            var loopDatas = new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>();
            for(int i = 0; i < 5; i++)
            {
                LetPortal.Portal.Entities.Databases.DatabaseConnection cloneChildConnection = _context.PostgreSqlDatabaseConnection.Copy();
                cloneChildConnection.Id = "";
                cloneChildConnection.Name += $"_{i}";
                loopDatas.Add(cloneChildConnection);
            }
            var loopDataModels = new List<LoopDataParamModel>();
            var loopDataModel = new LoopDataParamModel
            {
                Name = "data.databases.inserts",
                Parameters = new List<List<ExecuteParamModel>>()
            };
            foreach(var clone in loopDatas)
            {
                loopDataModel.Parameters.Add(new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = Guid.NewGuid().ToString() },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = clone.Name },
                            new ExecuteParamModel { Name = "data.insertingName", ReplaceValue = clone.Name },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = clone.DisplayName },
#pragma warning disable CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "currentTick()|long", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
#pragma warning restore CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = clone.ConnectionString},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = clone.DataSource},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = clone.DatabaseConnectionType }
                        });
            }
            loopDataModels.Add(loopDataModel);
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.PostgreSqlDatabaseConnection.Id,
                DataLoopKey = "data.databases.inserts",
                ExecuteCommand = "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.insertingName}},{{data.displayName}}, {{currentTick()|long}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}});" +
                " Select * from databases where name={{data.insertingName}}"
            });

            // S5: Delete
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.PostgreSqlDatabaseConnection.Id,
                ExecuteCommand = "DELETE From databases Where id={{$step3.id}}"
            });

            // Act
            string insertId = Guid.NewGuid().ToString();
            var result = await databaseService.ExecuteDynamic(
                new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>
                {
                    _context.PostgreSqlDatabaseConnection
                },
                executionChains,
                new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = insertId },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" },
                            new ExecuteParamModel { Name = "data.insertingName", ReplaceValue = "testdatabase123" },
                            new ExecuteParamModel { Name = "data.updatingDisplayName", ReplaceValue = "Test Database ABC" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
#pragma warning disable CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "currentTick()|long", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
#pragma warning restore CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = "abc"},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = "localhost"},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = "postgresql" }
                        },
                loopDataModels
                ).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
        }


        #endregion

        #region UTs for SQL Server
        [Fact]
        public async Task Extract_Schema_From_Query_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExtractionDatabase sqlServerExtractionDatabase =
                new SqlServerExtractionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(null, new List<IExtractionDatabase> { sqlServerExtractionDatabase });
            // Act
            string warpQuery = "Select * From databases";
            ExtractingSchemaQueryModel result = await databaseService.ExtractColumnSchema(_context.SqlServerDatabaseConnection, warpQuery, null);

            // Assert
            Assert.NotEmpty(result.ColumnFields);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExecutionDatabase sqlServerExecutionDatabase =
                new SqlServerExecutionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { sqlServerExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(_context.SqlServerDatabaseConnection, "Select * from databases", null);

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_With_Params_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExecutionDatabase sqlServerExecutionDatabase =
                new SqlServerExecutionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { sqlServerExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(_context.SqlServerDatabaseConnection, "Select * from databases Where name={{data.name}}", new List<ExecuteParamModel> { new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" } });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Insert_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExecutionDatabase sqlServerExecutionDatabase =
                new SqlServerExecutionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { sqlServerExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.SqlServerDatabaseConnection,
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
        public async Task Execute_Update_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExecutionDatabase sqlServerExecutionDatabase =
                new SqlServerExecutionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { sqlServerExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.SqlServerDatabaseConnection,
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
        public async Task Execute_Delete_In_Sql_Server_Test()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExecutionDatabase sqlServerExecutionDatabase =
                new SqlServerExecutionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { sqlServerExecutionDatabase }, null);
            string insertId = Guid.NewGuid().ToString();
            // Act
            await databaseService.ExecuteDynamic(
                    _context.SqlServerDatabaseConnection,
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
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.SqlServerDatabaseConnection,
                        "DELETE From databases Where id={{data.id}}",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "data.id", ReplaceValue = insertId }
                        });

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Multiple_Commands_In_SqlServer()
        {
            if(!_context.AllowSQLServer)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            SqlServerExecutionDatabase sqlServerExecutionDatabase =
                new SqlServerExecutionDatabase(
                    new SqlServerMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { sqlServerExecutionDatabase }, null);

            var executionChains = new DatabaseExecutionChains
            {
                Steps = new List<DatabaseExecutionStep>()
            };

            // S1: Query
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.SqlServerDatabaseConnection.Id,
                ExecuteCommand = "Select * from databases Where name={{data.name}}"
            });

            // S2: Update
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.SqlServerDatabaseConnection.Id,
                ExecuteCommand = "Update databases SET \"displayName\"={{data.updatingDisplayName}} WHERE id={{$step1.id}}"
            });

            // S3: Insert            
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.SqlServerDatabaseConnection.Id,
                ExecuteCommand = "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.insertingName}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}});" +
                " Select * from databases where name={{data.insertingName}}"
            });

            // S4: Insert multiple children
            var loopDatas = new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>();
            for(int i = 0; i < 5; i++)
            {
                LetPortal.Portal.Entities.Databases.DatabaseConnection cloneChildConnection = _context.SqlServerDatabaseConnection.Copy();
                cloneChildConnection.Id = "";
                cloneChildConnection.Name += $"_{i}";
                loopDatas.Add(cloneChildConnection);
            }
            var loopDataModels = new List<LoopDataParamModel>();
            var loopDataModel = new LoopDataParamModel
            {
                Name = "data.databases.inserts",
                Parameters = new List<List<ExecuteParamModel>>()
            };
            foreach(var clone in loopDatas)
            {
                loopDataModel.Parameters.Add(new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = Guid.NewGuid().ToString() },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = clone.Name },
                            new ExecuteParamModel { Name = "data.insertingName", ReplaceValue = clone.Name },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = clone.DisplayName },
#pragma warning disable CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "currentTick()|long", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
#pragma warning restore CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = clone.ConnectionString},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = clone.DataSource},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = clone.DatabaseConnectionType }
                        });
            }
            loopDataModels.Add(loopDataModel);
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.SqlServerDatabaseConnection.Id,
                DataLoopKey = "data.databases.inserts",
                ExecuteCommand = "INSERT INTO databases(id, name, \"displayName\", \"timeSpan\", \"connectionString\", \"dataSource\", \"databaseConnectionType\") VALUES({{guid()}}, {{data.insertingName}},{{data.displayName}}, {{currentTick()|long}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}});" +
                " Select * from databases where name={{data.insertingName}}"
            });

            // S5: Delete
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.SqlServerDatabaseConnection.Id,
                ExecuteCommand = "DELETE From databases Where id={{$step3.id}}"
            });

            // Act
            string insertId = Guid.NewGuid().ToString();
            var result = await databaseService.ExecuteDynamic(
                new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>
                {
                    _context.SqlServerDatabaseConnection
                },
                executionChains,
                new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = insertId },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" },
                            new ExecuteParamModel { Name = "data.insertingName", ReplaceValue = "testdatabase123" },
                            new ExecuteParamModel { Name = "data.updatingDisplayName", ReplaceValue = "Test Database ABC" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
#pragma warning disable CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "currentTick()", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
#pragma warning restore CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = "abc"},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = "localhost"},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = "postgresql" }
                        },
                loopDataModels
                ).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion

        #region UTs for MySQL
        [Fact]
        public async Task Extract_Schema_From_Query_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExtractionDatabase mysqlExtractionDatabase =
                new MySqlExtractionDatabase(new MySqlMapper(_context.MapperOptions), new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(null, new List<IExtractionDatabase> { mysqlExtractionDatabase });
            // Act
            string warpQuery = "Select * From `apps` Where date(`createdDate`)={{queryparams.date|date}}";
            ExtractingSchemaQueryModel result = await databaseService
                                .ExtractColumnSchema(_context.MySqlDatabaseConnection, warpQuery,
                                    new List<ExecuteParamModel>
                                    {
                                        new ExecuteParamModel { Name = "queryparams.date|date", ReplaceValue = DateTime.UtcNow.ToString() }
                                    });

            // Assert
            Assert.NotEmpty(result.ColumnFields);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExecutionDatabase mysqlExecutionDatabase =
                new MySqlExecutionDatabase(
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { mysqlExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(_context.MySqlDatabaseConnection, "Select * from `databases`", null);

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Dynamic_Query_With_Params_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExecutionDatabase mysqlExecutionDatabase =
                new MySqlExecutionDatabase(
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { mysqlExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(_context.MySqlDatabaseConnection, "Select * from `databases` Where name={{data.name}}", new List<ExecuteParamModel> { new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" } });

            // Assert
            Assert.NotEmpty(result.Result);
        }

        [Fact]
        public async Task Execute_Insert_In_MySql_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExecutionDatabase mysqlExecutionDatabase =
                new MySqlExecutionDatabase(
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { mysqlExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.MySqlDatabaseConnection,
                        "INSERT INTO `databases`(id, name, displayName, timeSpan, connectionString, dataSource, databaseConnectionType) VALUES({{guid()}}, {{data.name}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}})",
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
        public async Task Execute_Update_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExecutionDatabase mysqlExecutionDatabase =
                new MySqlExecutionDatabase(
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { mysqlExecutionDatabase }, null);

            // Act
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.MySqlDatabaseConnection,
                        "Update `databases` SET `displayName`={{data.displayName}} WHERE `id`={{data.id}}",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "data.id", ReplaceValue = _context.MySqlDatabaseConnection.Id },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" }
                        });

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Delete_In_MySQL_Test()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExecutionDatabase mysqlExecutionDatabase =
                new MySqlExecutionDatabase(
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { mysqlExecutionDatabase }, null);
            string insertId = Guid.NewGuid().ToString();
            // Act
            await databaseService.ExecuteDynamic(
                    _context.MySqlDatabaseConnection,
                        "INSERT INTO `databases`(id, name, displayName, timeSpan, connectionString, dataSource, databaseConnectionType) VALUES({{guid()}}, {{data.name}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}})",
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
            LetPortal.Portal.Models.ExecuteDynamicResultModel result =
                await databaseService.ExecuteDynamic(
                    _context.MySqlDatabaseConnection,
                        "DELETE From `databases` Where id={{data.id}}",
                        new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "data.id", ReplaceValue = insertId }
                        });

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Multiple_Commands_In_MySql()
        {
            if(!_context.AllowMySQL)
            {
                Assert.True(true);
                return;
            }
            // Arrange
            MySqlExecutionDatabase mysqlExecutionDatabase =
                new MySqlExecutionDatabase(
                    new MySqlMapper(_context.MapperOptions),
                    new CSharpMapper());

            DatabaseService databaseService = new DatabaseService(new List<IExecutionDatabase> { mysqlExecutionDatabase }, null);

            var executionChains = new DatabaseExecutionChains
            {
                Steps = new List<DatabaseExecutionStep>()
            };

            // S1: Query
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MySqlDatabaseConnection.Id,
                ExecuteCommand = "Select * from `databases` Where name={{data.name}}"
            });

            // S2: Update
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MySqlDatabaseConnection.Id,
                ExecuteCommand = "Update `databases` SET `displayName`={{data.updatingDisplayName}} WHERE id={{$step1.id}}"
            });

            // S3: Insert            
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MySqlDatabaseConnection.Id,
                ExecuteCommand = "INSERT INTO `databases`(id, name, displayName, timeSpan, connectionString, dataSource, databaseConnectionType) VALUES({{guid()}}, {{data.insertingName}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}});" +
                " Select * from `databases` where name={{data.insertingName}}"
            });

            // S4: Insert multiple children
            var loopDatas = new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>();
            for(int i = 0; i < 5; i++)
            {
                LetPortal.Portal.Entities.Databases.DatabaseConnection cloneChildConnection = _context.MySqlDatabaseConnection.Copy();
                cloneChildConnection.Id = "";
                cloneChildConnection.Name += $"_{i}";
                loopDatas.Add(cloneChildConnection);
            }
            var loopDataModels = new List<LoopDataParamModel>();
            var loopDataModel = new LoopDataParamModel
            {
                Name = "data.databases.inserts",
                Parameters = new List<List<ExecuteParamModel>>()
            };
            foreach(var clone in loopDatas)
            {
                loopDataModel.Parameters.Add(new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = Guid.NewGuid().ToString() },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = clone.Name },
                            new ExecuteParamModel { Name = "data.insertingName", ReplaceValue = clone.Name },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = clone.DisplayName },
#pragma warning disable CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "currentTick()", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
#pragma warning restore CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = clone.ConnectionString},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = clone.DataSource},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = clone.DatabaseConnectionType }
                        });
            }
            loopDataModels.Add(loopDataModel);
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MySqlDatabaseConnection.Id,
                DataLoopKey = "data.databases.inserts",
                ExecuteCommand = "INSERT INTO `databases`(id, name, displayName, timeSpan, connectionString, dataSource, databaseConnectionType) VALUES({{guid()}}, {{data.insertingName}},{{data.displayName}}, {{currentTick()}}, {{data.connectionString}}, {{data.dataSource}}, {{data.databaseConnectionType}});" +
                " Select * from `databases` where name={{data.insertingName}}"
            });

            // S5: Delete
            executionChains.Steps.Add(new DatabaseExecutionStep
            {
                DatabaseConnectionId = _context.MySqlDatabaseConnection.Id,
                ExecuteCommand = "DELETE From `databases` Where id={{$step3.id}}"
            });

            // Act
            string insertId = Guid.NewGuid().ToString();
            var result = await databaseService.ExecuteDynamic(
                new List<LetPortal.Portal.Entities.Databases.DatabaseConnection>
                {
                    _context.MySqlDatabaseConnection
                },
                executionChains,
                new List<ExecuteParamModel>
                        {
                            new ExecuteParamModel { Name = "guid()", ReplaceValue = insertId },
                            new ExecuteParamModel { Name = "data.name", ReplaceValue = "testdatabase" },
                            new ExecuteParamModel { Name = "data.insertingName", ReplaceValue = "testdatabase123" },
                            new ExecuteParamModel { Name = "data.updatingDisplayName", ReplaceValue = "Test Database ABC" },
                            new ExecuteParamModel { Name = "data.displayName", ReplaceValue = "Test Database" },
#pragma warning disable CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "currentTick()", ReplaceValue = DateTime.UtcNow.Ticks.ToString() },
#pragma warning restore CA1305 // Specify IFormatProvider
                            new ExecuteParamModel { Name = "data.connectionString", ReplaceValue = "abc"},
                            new ExecuteParamModel { Name = "data.dataSource", ReplaceValue = "localhost"},
                            new ExecuteParamModel { Name = "data.databaseConnectionType", ReplaceValue = "postgresql" }
                        },
                loopDataModels
                ).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
        }

        #endregion
    }
}
