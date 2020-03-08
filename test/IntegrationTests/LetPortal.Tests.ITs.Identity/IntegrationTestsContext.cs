using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Repositories.Identity;
using LetPortal.Identity.Stores;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;

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
                Datasource = GenerateUniqueDatasourceName()
            };

            CreateSomeDummyData();
        }

        public RoleStore GetRoleStore()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            RoleStore roleStore = new RoleStore(roleRepository: GetRoleMongoRepository());
#pragma warning restore CA2000 // Dispose objects before losing scope

            return roleStore;
        }

        public UserStore GetUserStore()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            UserStore userStore = new UserStore(userRepository: GetUserMongoRepository(), roleRepository: GetRoleMongoRepository());
#pragma warning restore CA2000 // Dispose objects before losing scope
            return userStore;
        }

        public User GenerateUser()
        {
            string username = GenerateUniqueUserName();
            return new User
            {
                Id = DataUtil.GenerateUniqueId(),
                Username = username,
                NormalizedUserName = username.ToUpper(),
                Domain = string.Empty,
                PasswordHash = "AQAAAAEAACcQAAAAEBhhMYTL5kwYqXheHSdarA/+vleSI07yGkTKNw1bb1jrTlYnBZK1CZ+zdHnqWwLLDA==",
                Email = username + "@portal.com",
                NormalizedEmail = username.ToUpper() + "@PORTAL.COM",
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
                    StandardClaims.Name(username)
                }
            };
        }

        public Role GenerateRole()
        {
            string roleName = GenerateUniqueRoleName();
            return new Role
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                DisplayName = roleName,
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp("5c162e9005924c1c741bfdc2")
                }
            };
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    // Remove all created databases
                    MongoClient mongoClient = new MongoClient(MongoDatabaseOptions.ConnectionString);
                    mongoClient.DropDatabase(MongoDatabaseOptions.Datasource);
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private string GenerateUniqueRoleName()
        {
            char[] suppliedVars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int lengthOfName = 20;
            string role = string.Empty;
            for(int i = 0; i < lengthOfName; i++)
            {
                int randomIndx = (new Random()).Next(0, 45);
                role += suppliedVars[randomIndx];
            }

            return role;
        }

        private string GenerateUniqueUserName()
        {
            char[] suppliedVars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int lengthOfName = 20;
            string username = string.Empty;
            for(int i = 0; i < lengthOfName; i++)
            {
                int randomIndx = (new Random()).Next(0, 45);
                username += suppliedVars[randomIndx];
            }

            return username;
        }

        private UserMongoRepository GetUserMongoRepository()
        {
            DatabaseOptions databaseOptions = MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            UserMongoRepository userMongoRepository = new UserMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            return userMongoRepository;
        }

        private RoleMongoRepository GetRoleMongoRepository()
        {
            DatabaseOptions databaseOptions = MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            RoleMongoRepository roleMongoRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            return roleMongoRepository;
        }

        private string GenerateUniqueDatasourceName(string startWith = "letportal_")
        {
            char[] suppliedVars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int lengthOfName = 15;
            string datasourceName = string.Empty;
            for(int i = 0; i < lengthOfName; i++)
            {
                int randomIndx = (new Random()).Next(0, 45);
                datasourceName += suppliedVars[randomIndx];
            }

            return startWith + datasourceName;
        }

        private void CreateSomeDummyData()
        {
            MongoClient mongoClient = new MongoClient(MongoDatabaseOptions.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(MongoDatabaseOptions.Datasource);
            IMongoCollection<Role> roleCollection = mongoDatabase.GetCollection<Role>("roles");
            IMongoCollection<User> userCollection = mongoDatabase.GetCollection<User>("users");
            Role superAdminRole = new Role
            {
                Id = "5c06a15e4cc9a850bca44488",
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                DisplayName = "Super Admin",
                Claims = new List<BaseClaim>
                {
                    StandardClaims.AccessCoreApp("5c162e9005924c1c741bfdc2")
                }
            };

            // Pass: @Dm1n!
            User adminAccount = new User
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
