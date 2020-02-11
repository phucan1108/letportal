using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Exceptions.Databases;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Services.Databases;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.UTs.Portal.Services
{
    public class DatabaseServiceTests
    {
        [Fact]
        public async Task Execute_Dynamic_Command_In_Mongo_Test()
        {
            // Arrange
            Mock<IExecutionDatabase> mongoExecutionDatabaseMock = new Mock<IExecutionDatabase>();
            mongoExecutionDatabaseMock.Setup(a => a.ConnectionType).Returns(Core.Persistences.ConnectionType.MongoDB);
            mongoExecutionDatabaseMock
                .Setup(a => a.Execute(It.IsAny<DatabaseConnection>(), It.IsAny<string>(), It.IsAny<IEnumerable<ExecuteParamModel>>()))
                .Returns(Task.FromResult(new ExecuteDynamicResultModel
                {
                    IsSuccess = true,
                    Result = "A"
                }));

            DatabaseService databaseService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabaseMock.Object }, null);

            // Act
            ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(new LetPortal.Portal.Entities.Databases.DatabaseConnection
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseConnectionType = "mongodb",
                DataSource = "letportal"
            }, "", new List<ExecuteParamModel>());

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Execute_Dynamic_Command_Not_Supported_DbType_Test()
        {
            // Arrange
            Mock<IExecutionDatabase> mongoExecutionDatabaseMock = new Mock<IExecutionDatabase>();
            mongoExecutionDatabaseMock.Setup(a => a.ConnectionType).Returns(Core.Persistences.ConnectionType.MongoDB);
            mongoExecutionDatabaseMock
                .Setup(a => a.Execute(It.IsAny<DatabaseConnection>(), It.IsAny<string>(), It.IsAny<IEnumerable<ExecuteParamModel>>()))
                .Returns(Task.FromResult(new ExecuteDynamicResultModel
                {
                    IsSuccess = true,
                    Result = "A"
                }));

            DatabaseService databaseService = new DatabaseService(new IExecutionDatabase[] { mongoExecutionDatabaseMock.Object }, null);

            // Act
            try
            {
                ExecuteDynamicResultModel result = await databaseService.ExecuteDynamic(new LetPortal.Portal.Entities.Databases.DatabaseConnection
                {
                    DatabaseConnectionType = "SQLServer"
                }, "", new List<ExecuteParamModel>());

                Assert.False(true);
            }
            catch(Exception ex)
            {
                if(ex is DatabaseException databaseException)
                {
                    Assert.True(databaseException.ErrorCode.Equals(DatabaseErrorCodes.NotSupportedConnectionType));
                }
                else
                {
                    Assert.False(true);
                }
            }
        }

        [Fact]
        public async Task Extract_Columns_In_Mongo_Test()
        {
            // Arrange
            Mock<IExtractionDatabase> mongoExtractionDatabaseMock = new Mock<IExtractionDatabase>();
            mongoExtractionDatabaseMock.Setup(a => a.ConnectionType).Returns(Core.Persistences.ConnectionType.MongoDB);
            mongoExtractionDatabaseMock
                .Setup(a => a.Extract(It.IsAny<DatabaseConnection>(), It.IsAny<string>(), It.IsAny<IEnumerable<ExecuteParamModel>>()))
                .Returns(Task.FromResult(new ExtractingSchemaQueryModel
                {
                    ColumnFields = new System.Collections.Generic.List<LetPortal.Portal.Models.Shared.ColumnField>
                    {
                        new LetPortal.Portal.Models.Shared.ColumnField
                        {
                            Name = "A",
                            DisplayName = "A",
                            FieldType ="a"
                        }
                    }
                }));

            DatabaseService databaseService = new DatabaseService(null, new IExtractionDatabase[] { mongoExtractionDatabaseMock.Object });

            // Act
            ExtractingSchemaQueryModel result = await databaseService.ExtractColumnSchema(new LetPortal.Portal.Entities.Databases.DatabaseConnection
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseConnectionType = "mongodb",
                DataSource = "letportal"
            }, "", null);

            // Assert
            Assert.True(result.ColumnFields.Count > 0);
        }
    }
}
