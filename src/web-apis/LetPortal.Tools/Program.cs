using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Identity.Repositories;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Repositories;
using LetPortal.Tools;
using LetPortal.Tools.Features;
using LetPortal.Versions;
using McMaster.Extensions.CommandLineUtils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LET.Tools.Installation
{
    [Command(Name = "LetPortal CLI")]
    internal class Program
    {
        [Option("-c|--connection", Description = "A connection string to indicate the database")]
        public string Connection { get; set; }

        [Option("-db|--db-type", Description = "Database type, support specific parameter: mongodb | sqlserver")]
        public string DatabseType { get; set; } = "mongodb";

        [Option("-f|--file", Description = "File config path. Ex: C:\\tools.json. Default: a config file of installed tools")]
        public string FilePath { get; set; }

        [Option("-p|--patch", Description = "Patches folder path. Ex: C:\\Patches. Available only for portal app")]
        public string PatchesFolder { get; set; }

        [Argument(0, Name = "app", Description = "Supports: portal | identity")]
        public string App { get; set; }

        [Argument(1, Name = "mode", Description = "Supports: info | install | uninstall | up | down")]
        public string Mode { get; set; }

        [Argument(2, Name = "version", Description = "Version number is needed to run with mode. Ex: 0.0.1")]
        public string VersionNumber { get; set; }

        public static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("--------------------++++LETPORTAL CLI++++-----------------------");
                Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");                
                Console.WriteLine("");
                await CommandLineApplication.ExecuteAsync<Program>(args);
                Console.ReadLine();
                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Oops, something went wrong. Exception stack: " + ex.ToString());
                Console.ReadLine();
                return 0;
            }
            
        }

        private async Task OnExecuteAsync()
        {  
            var toolsOption = GetToolsOptions(FilePath);
            ConventionPackDefault.Register();
            MongoDbRegistry.RegisterEntities();
            var dbType = DatabseType.ToEnum<ConnectionType>(true);
            var databaseOption = new DatabaseOptions
            {
                ConnectionString = !string.IsNullOrEmpty(Connection) ? Connection : GetDefaultConnectionString(dbType, App),
                ConnectionType = dbType
            };

            var runningCommand = GetAvailableCommands().FirstOrDefault(a => a.CommandName.ToLower() == Mode.ToLower());

            if(runningCommand != null)
            {
                ToolsContext toolsContext = null;
                switch(dbType)
                {
                    case ConnectionType.MongoDB:
                        var mongoConnection = new MongoConnection(databaseOption);
                        var mongoVersionContext = new MongoVersionContext(databaseOption);
                        var versionMongoRepository = new VersionMongoRepository(mongoConnection);
                        mongoVersionContext.ConnectionType = ConnectionType.MongoDB;
                        mongoVersionContext.DatabaseOptions = databaseOption;
                        mongoVersionContext.ServiceManagementOptions = toolsOption.StoringConnections.ServiceManagementConnection;
                        mongoVersionContext.IdentityDbOptions = toolsOption.StoringConnections.IdentityConnection;
                        var latestVersion = versionMongoRepository.GetAsQueryable().ToList().LastOrDefault();

                        IEnumerable<IVersion> allVersions = Enumerable.Empty<IVersion>();
                        if(IsPortal())
                        {
                            allVersions = Scanner.GetAllPortalVersions();
                        }
                        else if(IsIdentity())
                        {
                            allVersions = Scanner.GetAllIdentityVersions();
                        }

                        toolsContext = new ToolsContext
                        {
                            LatestVersion = latestVersion,
                            VersionContext = mongoVersionContext,
                            VersionNumber = VersionNumber,
                            Versions = allVersions,
                            VersionRepository = versionMongoRepository,
                            PatchesFolder = PatchesFolder,
                            AllowPatch = !string.IsNullOrEmpty(PatchesFolder)
                        };
                        break;
                    case ConnectionType.PostgreSQL:
                    case ConnectionType.MySQL:
                    case ConnectionType.SQLServer:

                        if(IsPortal())
                        {
                            var letportalContext = new LetPortalDbContext(databaseOption);
                            letportalContext.Database.EnsureCreated();

                            var letportalContextForRepo = new LetPortalDbContext(databaseOption);
                            var sqlEFVersionContext = new EFVersionContext(letportalContext)
                            {
                                ConnectionType = dbType,
                                DatabaseOptions = databaseOption
                            };
                            var portalVersionRepository = new VersionEFRepository(letportalContextForRepo);
                            var latestVersionEF = portalVersionRepository.GetAsQueryable().ToList().LastOrDefault();
                            sqlEFVersionContext.ServiceManagementOptions = toolsOption.StoringConnections.ServiceManagementConnection;
                            sqlEFVersionContext.IdentityDbOptions = toolsOption.StoringConnections.IdentityConnection;

                            IEnumerable<IVersion> allSQLVersions = Scanner.GetAllIdentityVersions();
                            toolsContext = new ToolsContext
                            {
                                LatestVersion = latestVersionEF,
                                VersionContext = sqlEFVersionContext,
                                VersionNumber = VersionNumber,
                                Versions = allSQLVersions,
                                VersionRepository = portalVersionRepository,
                                AllowPatch = !string.IsNullOrEmpty(PatchesFolder)
                            };
                        }
                        else if(IsIdentity())
                        {
                            var letIdentityContext = new LetPortalIdentityDbContext(databaseOption);
                            letIdentityContext.Database.EnsureCreated();

                            var letportalContextForRepo = new LetPortalIdentityDbContext(databaseOption);
                            var sqlEFVersionContext = new EFVersionContext(letportalContextForRepo)
                            {
                                ConnectionType = dbType,
                                DatabaseOptions = databaseOption
                            };
                            var portalVersionRepository = new VersionEFRepository(letportalContextForRepo);
                            var latestVersionEF = portalVersionRepository.GetAsQueryable().ToList().LastOrDefault();
                            sqlEFVersionContext.ServiceManagementOptions = toolsOption.StoringConnections.ServiceManagementConnection;
                            sqlEFVersionContext.IdentityDbOptions = toolsOption.StoringConnections.IdentityConnection;

                            IEnumerable<IVersion> allSQLVersions = Scanner.GetAllIdentityVersions();

                            toolsContext = new ToolsContext
                            {
                                LatestVersion = latestVersionEF,
                                VersionContext = sqlEFVersionContext,
                                VersionNumber = VersionNumber,
                                Versions = allSQLVersions,
                                VersionRepository = portalVersionRepository,
                                AllowPatch = !string.IsNullOrEmpty(PatchesFolder)
                            };
                        }

                        break;
                }

                if(toolsContext != null)
                {
                    await runningCommand.RunAsync(toolsContext);
                }

                Console.WriteLine("-----------------------++++++DONE++++++-------------------------");
            }
            else
            {
                Console.WriteLine("Oops! We don't find any matching command to execute. If you don't know how to run, please type '--help'");
            }

            Console.ReadLine();
        }

        private bool IsPortal()
        {
            return App == "portal";
        }

        private bool IsIdentity()
        {
            return App == "identity";
        }

        private ToolsOptions GetToolsOptions(string filePath)
        {
            string fullPath;
            if(!string.IsNullOrEmpty(filePath))
            {
                fullPath = Path.GetFullPath(filePath);
            }
            else
            {
                fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "tools.json");
            }
            var foundText = File.ReadAllText(fullPath);
            return ConvertUtil.DeserializeObject<ToolsOptions>(foundText);
        }

        private IEnumerable<IFeatureCommand> GetAvailableCommands()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IFeatureCommand>(currentAssembly);
        }

        private string GetDefaultConnectionString(ConnectionType connectionType, string app)
        {
            switch(connectionType)
            {
                case ConnectionType.MongoDB:
                    return string.Format("mongodb://localhost:27017/{0}", app == "portal" ? "letportal" : "letportalidentity");
                case ConnectionType.SQLServer:
                    return string.Format("Server=.;Database={0};User Id=sa;Password=123456;", app == "portal" ? "letportal" : "letportalidentity");
                case ConnectionType.PostgreSQL:
                    return string.Format("Host=localhost;Port=5432;Database={0};Username=postgres;Password=123456", app == "portal" ? "letportal" : "letportalidentity");                    
                case ConnectionType.MySQL:
                    return string.Format("server=localhost;uid=root;pwd=123456;database={0}", app == "portal" ? "letportal" : "letportalidentity");                    
                default:
                    return "";
            }
        }
    }
}
