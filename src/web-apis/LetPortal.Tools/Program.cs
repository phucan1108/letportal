using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.PortalVersions;
using LetPortal.Tools;
using LetPortal.Tools.Features;
using LetPortal.Versions;
using McMaster.Extensions.CommandLineUtils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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

        [Option("-f|--force", Description = "Force run without asking")]
        public bool Force { get; set; } = false;

        [Argument(0, Name = "mode", Description = "Supports: info | install | uninstall | up | down")]
        public string Mode { get; set; }

        [Argument(1, Name = "version", Description = "Version number is needed to run with mode. Ex: 0.0.1")]
        public string VersionNumber { get; set; }

        public static async Task<int> Main(string[] args)
        {
            return await CommandLineApplication.ExecuteAsync<Program>(args);
        }

        private async Task OnExecuteAsync()
        {
            ConventionPackDefault.Register();
            MongoDbRegistry.RegisterEntities();
            var dbType = DatabseType.ToEnum<ConnectionType>(true);

            var allVersions = Scanner.GetAllVersions();

            var databaseOption = new DatabaseOptions
            {
                ConnectionString = !string.IsNullOrEmpty(Connection) ? Connection : GetDefaultConnectionString(dbType),
                ConnectionType = dbType,
                Datasource = "letportal"
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
                        var portalMongoRepository = new PortalVersionMongoRepository(mongoConnection);
                        mongoVersionContext.ConnectionType = ConnectionType.MongoDB;
                        mongoVersionContext.DatabaseOptions = databaseOption;
                        var latestVersion = portalMongoRepository.GetAsQueryable().ToList().LastOrDefault();

                        toolsContext = new ToolsContext
                        {
                            LatestVersion = latestVersion,
                            VersionContext = mongoVersionContext,
                            VersionNumber = VersionNumber,
                            Versions = allVersions,
                            PortalVersionRepository = portalMongoRepository
                        };

                        await runningCommand.RunAsync(toolsContext);
                        break;
                    case ConnectionType.PostgreSQL:
                    case ConnectionType.MySQL:
                    case ConnectionType.SQLServer:
                        var letportalContext = new LetPortalVersionDbContext(databaseOption);
                        letportalContext.Database.EnsureCreated();

                        var letportalContextForRepo = new LetPortalVersionDbContext(databaseOption);
                        var sqlEFVersionContext = new EFVersionContext(letportalContext);
                        sqlEFVersionContext.ConnectionType = dbType;
                        sqlEFVersionContext.DatabaseOptions = databaseOption;
                        var portalVersionRepository = new PortalVersionEFRepository(letportalContextForRepo);
                        var latestVersionEF = portalVersionRepository.GetAsQueryable().ToList().LastOrDefault();

                        toolsContext = new ToolsContext
                        {
                            LatestVersion = latestVersionEF,
                            VersionContext = sqlEFVersionContext,
                            VersionNumber = VersionNumber,
                            Versions = allVersions,
                            PortalVersionRepository = portalVersionRepository
                        };
                        break;
                }

                if(toolsContext != null)
                {
                    await runningCommand.RunAsync(toolsContext);
                }

                Console.WriteLine("====Done====");
            }
            else
            {
                Console.WriteLine("Oops! We don't find any matching command to execute. If you don't know how to run, please type '--help'");
            }
        }

        private IEnumerable<IFeatureCommand> GetAvailableCommands()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IFeatureCommand>(currentAssembly);
        }

        private string GetDefaultConnectionString(ConnectionType connectionType)
        {
            switch(connectionType)
            {
                case ConnectionType.MongoDB:
                    return "mongodb://localhost:27017/letportal";
                case ConnectionType.SQLServer:
                    return "Server=.;Database=letportal;User Id=sa;Password=123456;";
                case ConnectionType.PostgreSQL:
                    return "Host=localhost;Port=5432;Database=letportal;Username=postgres;Password=123456";
                case ConnectionType.MySQL:
                    return "server=localhost;uid=root;pwd=123456;database=letportal";
                default:
                    return "";
            }
        }
    }
}
