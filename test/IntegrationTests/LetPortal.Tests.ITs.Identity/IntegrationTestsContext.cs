using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace LetPortal.Tests.ITs.Identity
{
    public class IntegrationTestsContext : IDisposable
    {
        public DatabaseOptions MongoDatabaseOptions { get; }

        public static bool isRegistered;
        private static readonly object _lockObject = new object();

        public IntegrationTestsContext()
        {
            bool lockTaken = false;
            Monitor.Enter(_lockObject, ref lockTaken);
            try
            {
                if(!isRegistered)
                {
                    ConventionPackDefault.Register();
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

            MongoDatabaseOptions = new DatabaseOptions
            {
                ConnectionString = "mongodb://localhost:27017",
                ConnectionType = ConnectionType.MongoDB,
                Datasource = generateUniqueDatasourceName()
            };

            createSomeDummyData();
        }

        public void Dispose()
        {
            // Remove all created databases
            var mongoClient = new MongoClient(MongoDatabaseOptions.ConnectionString);
            mongoClient.DropDatabase(MongoDatabaseOptions.Datasource);
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
            var mongoClient = new MongoClient(MongoDatabaseOptions.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(MongoDatabaseOptions.Datasource);
            var roleCollection = mongoDatabase.GetCollection<Role>("roles");
            var userCollection = mongoDatabase.GetCollection<User>("users");
            var superAdminRole = new Role
            {
                Id = "5c06a15e4cc9a850bca44488",
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                DisplayName = "Super Admin",
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp,
                    new BaseClaim
                    {
                        ClaimType = "apps",
                        ClaimValue = "5c162e9005924c1c741bfd22"
                    }
                }
            };

            // Pass: @Dm1n!
            var adminAccount = new User
            {
                Id = "5ce287ee569d6f23e8504cef",
                Username = "admin",
                NormalizedUserName = "ADMIN",
                Domain = string.Empty,
                PasswordHash = "AQAAAAEAACcQAAAAEBhhMYTL5kwYqXheHSdarA/+vleSI07yGkTKNw1bb1jrTlYnBZK1CZ+zdHnqWwLLDA==",
                Email = "admin@portal.com",
                NormalizedEmail = "ADMIN@PORTAL.COM",
                IsConfirmedEmail = true,
                SecurityStamp = "7YHYVBYWLTYC4EAPVRS2SWX2IIUOZ3XM",
                AccessFailedCount = 0,
                IsLockoutEnabled = false,
                LockoutEndDate = DateTime.UtcNow,
                Roles = new List<string>
                {
                    "SuperAdmin"
                },
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessAppSelectorPage,
                    StandardClaims.Sub("5ce287ee569d6f23e8504cef"),
                    StandardClaims.UserId("5ce287ee569d6f23e8504cef"),
                    StandardClaims.Name("admin")
                }
            };
            roleCollection.InsertOne(superAdminRole);
            userCollection.InsertOne(adminAccount);
        }
    }
}
