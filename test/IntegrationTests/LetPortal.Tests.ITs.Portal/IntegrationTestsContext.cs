using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Persistences;
using MongoDB.Driver;
using System;

namespace LetPortal.Tests.ITs.Portal
{
    public class IntegrationTestsContext : IDisposable
    {
        public DatabaseConnection MongoDatabaseConenction { get; }

        public DatabaseConnection SqlServerDatabaseConnection { get; }

        public IntegrationTestsContext()
        {
            ConventionPackDefault.Register();
            MongoDbRegistry.RegisterEntities();

            MongoDatabaseConenction = new DatabaseConnection
            {
                Id = DataUtil.GenerateUniqueId(),
                DisplayName = "Test Database",
                Name = "testdatabase",
                TimeSpan = DateTime.UtcNow.Ticks,
                ConnectionString = "mongodb://localhost:27017",
                DatabaseConnectionType = "mongodb",
                DataSource = generateUniqueDatasourceName()
            };

            var sqlDatasourceName = generateUniqueDatasourceName();
            SqlServerDatabaseConnection = new DatabaseConnection
            {
                Id = DataUtil.GenerateUniqueId(),
                DisplayName = "Test Database",
                Name = "testdatabase",
                TimeSpan = DateTime.UtcNow.Ticks,
                ConnectionString = "Server=.;Database=" + sqlDatasourceName + ";User Id=sa;Password = 123456;",
                DatabaseConnectionType = "sqlserver",
                DataSource = sqlDatasourceName
            };

            createSomeDummyData();
        }

        public void Dispose()
        {
            // Remove all created databases
            var mongoClient = new MongoClient(MongoDatabaseConenction.ConnectionString);
            mongoClient.DropDatabase(MongoDatabaseConenction.DataSource);
        }

        private string generateUniqueDatasourceName()
        {
            var suppliedVars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var lengthOfName = 20;
            var datasourceName = string.Empty;
            for(int i = 0; i < lengthOfName; i++)
            {
                var randomIndx = (new Random()).Next(0, 45);
                datasourceName += suppliedVars[randomIndx];
            }

            return datasourceName;
        }

        private void createSomeDummyData()
        {
            var mongoClient = new MongoClient(MongoDatabaseConenction.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(MongoDatabaseConenction.DataSource);
            var mongoCollection = mongoDatabase.GetCollection<DatabaseConnection>("databases");

            mongoCollection.InsertOne(MongoDatabaseConenction);
        }
    }
}
