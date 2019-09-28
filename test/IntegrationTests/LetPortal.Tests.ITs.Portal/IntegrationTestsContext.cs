using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LetPortal.Tests.ITs.Portal
{
    public class IntegrationTestsContext : IDisposable
    {
        public DatabaseConnection MongoDatabaseConenction { get; }

        public DatabaseConnection SqlServerDatabaseConnection { get; }

        public DatabaseConnection PostgreSqlDatabaseConnection { get; }

        public DatabaseConnection MySqlDatabaseConnection { get; }

        public bool AllowMongoDB
        {
            get
            {
                return intergrationTestOptions.RunningConnectionTypes.Any(a => a == ConnectionType.MongoDB);
            }
        }

        public bool AllowPostgreSQL
        {
            get
            {
                return intergrationTestOptions.RunningConnectionTypes.Any(a => a == ConnectionType.PostgreSQL);
            }
        }

        public bool AllowSQLServer
        {
            get
            {
                var allow = intergrationTestOptions.RunningConnectionTypes.Any(a => a == ConnectionType.SQLServer);
                return allow;
            }
        }

        public bool AllowMySQL
        {
            get
            {
                return intergrationTestOptions.RunningConnectionTypes.Any(a => a == ConnectionType.MySQL);
            }
        }

        public static bool isRegistered;

        private static readonly object _lockObject = new object();

        private static IntergrationTestOptions intergrationTestOptions;

        public IntegrationTestsContext()
        {
            bool lockTaken = false;
            Monitor.Enter(_lockObject, ref lockTaken);
            try
            {
                if(!isRegistered)
                {
                    var jObject = JObject.Parse(File.ReadAllText(@"Artifacts\settings.json"));
                    intergrationTestOptions = jObject.ToObject<IntergrationTestOptions>();

                    if(AllowMongoDB)
                    {
                        ConventionPackDefault.Register();
                        MongoDbRegistry.RegisterEntities();
                    }

                    isRegistered = true;
                }
            }
            finally
            {
                if(lockTaken)
                {
                    Monitor.Exit(_lockObject);
                }
            }

            if(AllowMongoDB)
            {    
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
                var mongoOptions = intergrationTestOptions.DatabasesList.First(a => a.DatabaseConnectionType.ToEnum<ConnectionType>(true) == ConnectionType.MongoDB);
                MongoDatabaseConenction.ConnectionString = mongoOptions.ConnectionString;                
            }

            if(AllowSQLServer)
            {
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
                var sqlOptions = intergrationTestOptions.DatabasesList.First(a => a.DatabaseConnectionType.ToEnum<ConnectionType>(true) == ConnectionType.SQLServer);
                SqlServerDatabaseConnection.ConnectionString = string.Format(sqlOptions.ConnectionString, sqlDatasourceName);
            }

            if(AllowPostgreSQL)
            {
                var postgresqlDatasourceName = generateUniqueDatasourceName();
                PostgreSqlDatabaseConnection = new DatabaseConnection
                {
                    Id = DataUtil.GenerateUniqueId(),
                    DisplayName = "Test Database",
                    Name = "testdatabase",
                    TimeSpan = DateTime.UtcNow.Ticks,
                    ConnectionString = "Host=localhost;Port=5432;Database=" + postgresqlDatasourceName + ";Username=postgres;Password=123456",
                    DatabaseConnectionType = "postgresql",
                    DataSource = postgresqlDatasourceName
                };
                var postgreOptions = intergrationTestOptions.DatabasesList.First(a => a.DatabaseConnectionType.ToEnum<ConnectionType>(true) == ConnectionType.PostgreSQL);
                PostgreSqlDatabaseConnection.ConnectionString = string.Format(postgreOptions.ConnectionString, postgresqlDatasourceName);
            }

            if(AllowMySQL)
            {
                var mysqlDatasourceName = generateUniqueDatasourceName();
                MySqlDatabaseConnection = new DatabaseConnection
                {
                    Id = DataUtil.GenerateUniqueId(),
                    DisplayName = "Test Database",
                    Name = "testdatabase",
                    TimeSpan = DateTime.UtcNow.Ticks,
                    ConnectionString = "Host=localhost;Port=5432;Database=" + mysqlDatasourceName + ";Username=postgres;Password=123456",
                    DatabaseConnectionType = "mysql",
                    DataSource = mysqlDatasourceName
                };
                var mysqlOptions = intergrationTestOptions.DatabasesList.First(a => a.DatabaseConnectionType.ToEnum<ConnectionType>(true) == ConnectionType.MySQL);
                MySqlDatabaseConnection.ConnectionString = string.Format(mysqlOptions.ConnectionString, mysqlDatasourceName);
            }
            
            createSomeDummyData();
        }

        public void Dispose()
        {

            // Remove all created databases
            if(AllowMongoDB)
            {
                var mongoClient = new MongoClient(MongoDatabaseConenction.ConnectionString);
                mongoClient.DropDatabase(MongoDatabaseConenction.DataSource);
            }

            if(AllowPostgreSQL)
            {
                var postgreContext = GetPostgreSQLContext();
                postgreContext.Database.EnsureDeleted();
            }

            if(AllowSQLServer)
            {
                var sqlContext = GetSQLServerContext();
                sqlContext.Database.EnsureDeleted();
            }

            if(AllowMySQL)
            {
                var mysqlContext = GetMySQLContext();
                mysqlContext.Database.EnsureDeleted();
            }
        }

        public MongoConnection GetMongoConnection()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = MongoDatabaseConenction.ConnectionString,
                ConnectionType = ConnectionType.MongoDB,
                Datasource = MongoDatabaseConenction.DataSource
            };
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);

            return new MongoConnection(databaseOptionsMock.CurrentValue);
        }

        public LetPortalDbContext GetPostgreSQLContext()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = PostgreSqlDatabaseConnection.ConnectionString,
                ConnectionType = ConnectionType.PostgreSQL,
                Datasource = PostgreSqlDatabaseConnection.DataSource
            };
            var letportalDbContext = new LetPortalDbContext(databaseOptions);
            return letportalDbContext;
        }

        public LetPortalDbContext GetSQLServerContext()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = SqlServerDatabaseConnection.ConnectionString,
                ConnectionType = ConnectionType.SQLServer,
                Datasource = SqlServerDatabaseConnection.DataSource
            };
            var letportalDbContext = new LetPortalDbContext(databaseOptions);
            return letportalDbContext;
        }

        public LetPortalDbContext GetMySQLContext()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = MySqlDatabaseConnection.ConnectionString,
                ConnectionType = ConnectionType.MySQL,
                Datasource = MySqlDatabaseConnection.DataSource
            };
            var letportalDbContext = new LetPortalDbContext(databaseOptions);
            return letportalDbContext;
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
            if(AllowMongoDB)
            {
                var mongoClient = new MongoClient(MongoDatabaseConenction.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(MongoDatabaseConenction.DataSource);
                var mongoCollection = mongoDatabase.GetCollection<DatabaseConnection>("databases");

                mongoCollection.InsertOne(MongoDatabaseConenction);
            }

            if(AllowPostgreSQL)
            {
                var postgreContext = GetPostgreSQLContext();
                postgreContext.Database.EnsureCreated();
                postgreContext.Databases.Add(PostgreSqlDatabaseConnection);
                postgreContext.SaveChanges();
            }

            if(AllowSQLServer)
            {
                var sqlContext = GetSQLServerContext();
                sqlContext.Database.EnsureCreated();
                sqlContext.Databases.Add(SqlServerDatabaseConnection);
                sqlContext.SaveChanges();
            }

            if(AllowMySQL)
            {
                var mysqlContext = GetMySQLContext();
                mysqlContext.Database.EnsureCreated();
                mysqlContext.Databases.Add(MySqlDatabaseConnection);
                mysqlContext.SaveChanges();
            }
        }
    }

    internal class IntergrationTestOptions
    {
        public List<DatabaseConnection> DatabasesList { get; set; }

        public List<ConnectionType> RunningConnectionTypes { get; set; }
    }
}
