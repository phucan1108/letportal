using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Repositories;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public MapperOptions MapperOptions
        {
            get
            {
                return intergrationTestOptions.MapperOptions;
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
                    DataSource = GenerateUniqueDatasourceName()
                };
                var mongoOptions = intergrationTestOptions.DatabasesList.First(a => a.DatabaseConnectionType.ToEnum<ConnectionType>(true) == ConnectionType.MongoDB);
                MongoDatabaseConenction.ConnectionString = mongoOptions.ConnectionString;
            }

            if(AllowSQLServer)
            {
                var sqlDatasourceName = GenerateUniqueDatasourceName();
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
                var postgresqlDatasourceName = GenerateUniqueDatasourceName();
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
                var mysqlDatasourceName = GenerateUniqueDatasourceName();
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

            CreateSomeDummyData();
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
                postgreContext.Dispose();

                var postgreServiceContext = GetPostgreServiceContext();
                postgreServiceContext.Database.EnsureDeleted();
                postgreServiceContext.Dispose();
            }

            if(AllowSQLServer)
            {
                var sqlContext = GetSQLServerContext();
                sqlContext.Database.EnsureDeleted();
                sqlContext.Dispose();

                var sqlServiceContext = GetSQLServerServiceContext();
                sqlServiceContext.Database.EnsureDeleted();
                sqlServiceContext.Dispose();
            }

            if(AllowMySQL)
            {
                var mysqlContext = GetMySQLContext();
                mysqlContext.Database.EnsureDeleted();
                mysqlContext.Dispose();

                var mysqlServiceContext = GetMySQLServiceContext();
                mysqlServiceContext.Database.EnsureDeleted();
                mysqlServiceContext.Dispose();
            }

            GC.SuppressFinalize(this);
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

        public LetPortalServiceManagementDbContext GetPostgreServiceContext()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = PostgreSqlDatabaseConnection.ConnectionString,
                ConnectionType = ConnectionType.PostgreSQL,
                Datasource = PostgreSqlDatabaseConnection.DataSource
            };
            var letportalDbContext = new LetPortalServiceManagementDbContext(databaseOptions);
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

        public LetPortalServiceManagementDbContext GetSQLServerServiceContext()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = SqlServerDatabaseConnection.ConnectionString,
                ConnectionType = ConnectionType.SQLServer,
                Datasource = SqlServerDatabaseConnection.DataSource
            };
            var letportalDbContext = new LetPortalServiceManagementDbContext(databaseOptions);
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

        public LetPortalServiceManagementDbContext GetMySQLServiceContext()
        {
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = MySqlDatabaseConnection.ConnectionString,
                ConnectionType = ConnectionType.MySQL,
                Datasource = MySqlDatabaseConnection.DataSource
            };
            var letportalDbContext = new LetPortalServiceManagementDbContext(databaseOptions);
            return letportalDbContext;
        }

        private string GenerateUniqueDatasourceName()
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

        private void CreateSomeDummyData()
        {
            if(AllowMongoDB)
            {
                var mongoClient = new MongoClient(MongoDatabaseConenction.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(MongoDatabaseConenction.DataSource);
                var mongoCollection = mongoDatabase.GetCollection<DatabaseConnection>("databases");
                mongoCollection.InsertOne(MongoDatabaseConenction);

                var mongoAppCollection = mongoDatabase.GetCollection<App>("apps");
                mongoAppCollection.InsertOne(SampleApp());
            }

            if(AllowPostgreSQL)
            {
                var postgreContext = GetPostgreSQLContext();
                postgreContext.Database.EnsureCreated();
                postgreContext.Databases.Add(PostgreSqlDatabaseConnection);
                postgreContext.Apps.Add(SampleApp());
                postgreContext.SaveChanges();
                postgreContext.Dispose();
            }

            if(AllowSQLServer)
            {
                var sqlContext = GetSQLServerContext();
                sqlContext.Database.EnsureCreated();
                sqlContext.Databases.Add(SqlServerDatabaseConnection);
                sqlContext.Apps.Add(SampleApp());
                // Sql Server must create a table for storing file
                sqlContext.Database.ExecuteSqlCommand("Create table [dbo].[uploadFiles] ([id] [nvarchar](450) NOT NULL, [file] [varbinary](max) NULL, CONSTRAINT [PK_uploadFiles] PRIMARY KEY CLUSTERED ( [id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
                sqlContext.SaveChanges();
                sqlContext.Dispose();
            }

            if(AllowMySQL)
            {
                var mysqlContext = GetMySQLContext();
                mysqlContext.Database.EnsureCreated();
                mysqlContext.Databases.Add(MySqlDatabaseConnection);
                mysqlContext.Apps.Add(SampleApp());
                mysqlContext.Database.ExecuteSqlCommand("Create table `uploadFiles`(`id` varchar(255) NOT NULL,  `file` mediumblob NULL, PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci");
                mysqlContext.SaveChanges();
                mysqlContext.Dispose();
            }
        }

        private App SampleApp()
        {
            return new App
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "testapp",
                DisplayName = "Test App",
                Author = "Admin",
                CurrentVersionNumber = "0.0.1",
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow.AddDays(7),
                DefaultUrl = "~",
                Logo = "icon",
                TimeSpan = DateTime.UtcNow.Ticks
            };
        }
        private IEnumerable<MonitorCounter> GenerateCounters(string serviceId, string serviceName)
        {
            var countersList = new List<MonitorCounter>();
            int random = RandomUtil.NextInt(20, 50);
            for(int i = 0; i < random; i++)
            {
                int randomBeatHour = RandomUtil.NextInt(1, 5);
                int randomCpuUsage = RandomUtil.NextInt(1, 99);
                int memoryUsage = RandomUtil.NextInt(1, 99);
                int successRequest = RandomUtil.NextInt(100, 3000);
                int failRequest = RandomUtil.NextInt(100, successRequest);
                var beatDate = DateTime.UtcNow.AddHours(randomBeatHour);
                var monitorCounterId = DataUtil.GenerateUniqueId();
                countersList.Add(new MonitorCounter
                {
                    Id = monitorCounterId,
                    BeatDate = beatDate,
                    ServiceId = serviceId,
                    ServiceName = serviceName,
                    HardwareCounter = new HardwareCounter
                    {
                        Id = DataUtil.GenerateUniqueId(),
                        CpuUsage = randomCpuUsage,
                        MemoryUsed = memoryUsage,
                        MonitorCounterId = monitorCounterId,
                        ServiceId = serviceId
                    },
                    HttpCounter = new HttpCounter
                    {
                        Id = DataUtil.GenerateUniqueId(),
                        MonitorCounterId = monitorCounterId,
                        AvgDuration = 100,
                        SuccessRequests = successRequest,
                        FailedRequests = failRequest,
                        MeansureDate = beatDate,
                        TotalRequestsPerDay = 999999
                    }
                });
            }

            return countersList;
        }
    }

    internal class IntergrationTestOptions
    {
        public List<DatabaseConnection> DatabasesList { get; set; }

        public MapperOptions MapperOptions { get; set; }

        public List<ConnectionType> RunningConnectionTypes { get; set; }
    }
}
