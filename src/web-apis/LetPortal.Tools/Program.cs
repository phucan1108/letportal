using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
using Microsoft.Extensions.DependencyInjection;
using LetPortal.Portal;

namespace LET.Tools.Installation
{
    [Command(Name = "LetPortal CLI")]
    internal class Program
    {
        [Option("-c|--connection", Description = "A connection string to indicate the database")]
        public string Connection { get; set; }

        [Option("-db|--db-type", Description = "Database type, support specific parameter: mongodb | sqlserver")]
        public string DatabseType { get; set; } = "mongodb";

        [Option("-f|--file", Description = "File path, support for unpack. Ex: C:\\coreapp.zip")]
        public string FilePath { get; set; }

        [Option("-cf|--config-file", Description = "File config path. Ex: C:\\tools.json. Default: a config file of installed tools")]
        public string ConfigFilePath { get; set; }

        [Option("-p|--patch", Description = "Patches folder path. Ex: C:\\Patches. Available only for portal app")]
        public string PatchesFolder { get; set; }

        [Option("-n|--name", Description = "App name, used to generate zip package. Ex: coreapp")]
        public string Name { get; set; }

        [Option("-m|--mode", Description = "Unpack mode. Supports: wipe | merge")]
        public string UnpackMode { get; set; }

        [Option("-o|--output", Description = "Folder path for saving file")]
        public string Output { get; set; }

        [Argument(0, Name = "app", Description = "Supports: portal | identity")]
        public string App { get; set; }

        [Argument(1, Name = "command", Description = "Supports: info | install | uninstall | up | down | pack | unpack | patch")]
        public string Command { get; set; }

        [Argument(2, Name = "version", Description = "Version number is mandatory to run with some commands. Ex: 0.0.1")]
        public string VersionNumber { get; set; }

        public static async Task<int> Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("****************************************************************");
                Console.WriteLine("                             *****                              ");
                Console.WriteLine("                         **** * * ****                          ");
                Console.WriteLine("                     ****     * *     ****                      ");
                Console.WriteLine("                 ****         * *         ****                  ");
                Console.WriteLine("             ****             * *             ****              ");
                Console.WriteLine("         ****           ****  * *  * * * * * *    ****          ");
                Console.WriteLine("     ****               *  *  * *  *           *      ****      ");
                Console.WriteLine(" ****                   *  *  * *  *  * * * *   *         ****  ");
                Console.WriteLine("  ****                  *  *  * *  *  * * * *   *        ****   ");
                Console.WriteLine("   ****                 *  *  * *  *           *        ****    ");
                Console.WriteLine("    ****                *  *  * *  *  *  * * *         ****     ");
                Console.WriteLine("     ****               *  *  * *  *  *               ****      ");
                Console.WriteLine("      ****              *  *  * *  *  *              ****       ");
                Console.WriteLine("       ****             *  *  * *  *  *             ****        ");
                Console.WriteLine("        ****     * * *  *  *  * *  *  *            ****         ");
                Console.WriteLine("         ****    * * * * * *  * *  ****           ****          ");
                Console.WriteLine("          ****                * *                ****           ");
                Console.WriteLine("             ****             * *             ****              ");
                Console.WriteLine("                ****          * *          ****                 ");
                Console.WriteLine("                   ****       * *       ****                    ");
                Console.WriteLine("                      ****    * *    ****                       ");
                Console.WriteLine("                         ****     ****                          ");
                Console.WriteLine("                             *****                              ");
                Console.WriteLine("****************************************************************");
                Console.WriteLine("");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("--------------------++++LETPORTAL CLI++++-----------------------");
                Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                Console.WriteLine("");
                await CommandLineApplication.ExecuteAsync<Program>(args);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oops, something went wrong. Exception stack: " + ex.ToString());
                return 0;
            }

        }

        private async Task OnExecuteAsync()
        {                  
            var services = new ServiceCollection();
            var toolsOption = GetToolsOptions(ConfigFilePath);
            ConventionPackDefault.Register();
            MongoDbRegistry.RegisterEntities();
            var dbType = DatabseType.ToEnum<ConnectionType>(true);
            var databaseOption = new DatabaseOptions
            {
                ConnectionString = !string.IsNullOrEmpty(Connection) 
                    ? Connection : GetDefaultConnectionString(dbType, App, toolsOption),
                ConnectionType = dbType
            };

            var runningCommand = GetAvailableCommands().FirstOrDefault(a => a.CommandName.ToLower() == Command.ToLower());

            if (runningCommand != null)
            {
                ToolsContext toolsContext = null;
                var storingConnections = toolsOption.GetByDatabaseType(dbType);
                switch (dbType)
                {
                    case ConnectionType.MongoDB:
                        var mongoConnection = new MongoConnection(databaseOption);
                        var mongoVersionContext = new MongoVersionContext(databaseOption);
                        var versionMongoRepository = new VersionMongoRepository(mongoConnection);
                        mongoVersionContext.ConnectionType = ConnectionType.MongoDB;
                        mongoVersionContext.DatabaseOptions = databaseOption;
                        mongoVersionContext.ServiceManagementOptions = storingConnections.ServiceManagementConnection;
                        mongoVersionContext.IdentityDbOptions = storingConnections.IdentityConnection;
                        var latestVersion = versionMongoRepository.GetAsQueryable().ToList().LastOrDefault();

                        var allVersions = Enumerable.Empty<IVersion>();
                        if (IsPortal())
                        {
                            allVersions = Scanner.GetAllPortalVersions();
                            RegisterServicesForPortal(services, databaseOption);
                        }
                        else if (IsIdentity())
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
                            AllowPatch = !string.IsNullOrEmpty(PatchesFolder),
                            Services = services.BuildServiceProvider(),
                            Arguments = CombineArguments()
                        };
                        break;
                    case ConnectionType.PostgreSQL:
                    case ConnectionType.MySQL:
                    case ConnectionType.SQLServer:

                        if (IsPortal())
                        {
#pragma warning disable CA2000 // Dispose objects before losing scope
                            var letportalContext = new PortalDbContext(databaseOption);
#pragma warning restore CA2000 // Dispose objects before losing scope
                            letportalContext.Database.EnsureCreated();

#pragma warning disable CA2000 // Dispose objects before losing scope
                            var letportalContextForRepo = new PortalDbContext(databaseOption);
#pragma warning restore CA2000 // Dispose objects before losing scope
                            var sqlEFVersionContext = new EFVersionContext(letportalContext)
                            {
                                ConnectionType = dbType,
                                DatabaseOptions = databaseOption
                            };
                            var portalVersionRepository = new VersionEFRepository(letportalContextForRepo);
                            var latestVersionEF = portalVersionRepository.GetAsQueryable().ToList().LastOrDefault();
                            sqlEFVersionContext.ServiceManagementOptions = storingConnections.ServiceManagementConnection;
                            sqlEFVersionContext.IdentityDbOptions = storingConnections.IdentityConnection;

                            IEnumerable<IVersion> allSQLVersions = Scanner.GetAllPortalVersions();
                            RegisterServicesForPortal(services, databaseOption);
                            toolsContext = new ToolsContext
                            {
                                LatestVersion = latestVersionEF,
                                VersionContext = sqlEFVersionContext,
                                VersionNumber = VersionNumber,
                                Versions = allSQLVersions,
                                VersionRepository = portalVersionRepository,
                                Services = services.BuildServiceProvider(),
                                AllowPatch = !string.IsNullOrEmpty(PatchesFolder),
                                Arguments = CombineArguments()
                            };
                        }
                        else if (IsIdentity())
                        {
#pragma warning disable CA2000 // Dispose objects before losing scope
                            var letIdentityContext = new IdentityDbContext(databaseOption);
#pragma warning restore CA2000 // Dispose objects before losing scope
                            letIdentityContext.Database.EnsureCreated();

#pragma warning disable CA2000 // Dispose objects before losing scope
                            var letportalContextForRepo = new IdentityDbContext(databaseOption);
#pragma warning restore CA2000 // Dispose objects before losing scope
                            var sqlEFVersionContext = new EFVersionContext(letportalContextForRepo)
                            {
                                ConnectionType = dbType,
                                DatabaseOptions = databaseOption
                            };
                            var portalVersionRepository = new VersionEFRepository(letportalContextForRepo);
                            var latestVersionEF = portalVersionRepository.GetAsQueryable().ToList().LastOrDefault();
                            sqlEFVersionContext.ServiceManagementOptions = storingConnections.ServiceManagementConnection;
                            sqlEFVersionContext.IdentityDbOptions = storingConnections.IdentityConnection;

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

                if (toolsContext != null)
                {
                    Console.WriteLine("");
                    await runningCommand.RunAsync(toolsContext);
                    toolsContext.Dispose();
                }

                Console.WriteLine("-----------------------++++++DONE++++++-------------------------");
            }
            else
            {
                Console.WriteLine("Oops! We don't find any matching command to execute. If you don't know how to run, please type '--help'");
            }

            Console.ReadLine();
        }

        private void RegisterServicesForPortal(IServiceCollection services, DatabaseOptions databaseOptions)
        {
            if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                services.AddTransient(a => new MongoConnection(databaseOptions));
            }

            PortalExtensions.RegisterRepos(services, databaseOptions, true);
            PortalExtensions.RegisterProviders(services);
            PortalExtensions.RegisterServices(services);
        }

        private RootArgument CombineArguments()
        {
            return new RootArgument
            {
                App = App,
                Command = Command,
                Connection = Connection,
                DatabseType = DatabseType,
                FilePath = FilePath,
                Name = Name,
                PatchesFolder = PatchesFolder,
                UnpackMode = UnpackMode,
                VersionNumber = VersionNumber,
                Output = Output
            };
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
            if (!string.IsNullOrEmpty(filePath))
            {
                fullPath = Path.GetFullPath(filePath);
            }
            else
            {
                fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "tools.json");
            }
            Console.WriteLine(">>> Config file path: {0}", fullPath);
            var foundText = File.ReadAllText(fullPath);
            return ConvertUtil.DeserializeObject<ToolsOptions>(foundText);
        }

        private IEnumerable<IFeatureCommand> GetAvailableCommands()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IFeatureCommand>(currentAssembly);
        }

        private static string GetDefaultConnectionString(ConnectionType connectionType, string app, ToolsOptions toolsOptions)
        {
            switch (connectionType)
            {
                case ConnectionType.MongoDB:     
                    if(app == "portal")
                    {
                        return toolsOptions.MongoStoringConnections.PortalConnection.ConnectionString;
                    }
                    else if (app == "identity")
                    {
                        return toolsOptions.MongoStoringConnections.IdentityConnection.ConnectionString;
                    }
                    else
                    {
                        return null;
                    }                    
                case ConnectionType.SQLServer:
                    if (app == "portal")
                    {
                        return toolsOptions.SqlServerStoringConnections.PortalConnection.ConnectionString;
                    }
                    else if (app == "identity")
                    {
                        return toolsOptions.SqlServerStoringConnections.IdentityConnection.ConnectionString;
                    }
                    else
                    {
                        return null;
                    }                    
                case ConnectionType.PostgreSQL:
                    if (app == "portal")
                    {
                        return toolsOptions.PostgreSqlStoringConnections.PortalConnection.ConnectionString;
                    }
                    else if (app == "identity")
                    {
                        return toolsOptions.PostgreSqlStoringConnections.IdentityConnection.ConnectionString;
                    }
                    else
                    {
                        return null;
                    }
                case ConnectionType.MySQL:
                    if (app == "portal")
                    {
                        return toolsOptions.MySqlStoringConnections.PortalConnection.ConnectionString;
                    }
                    else if (app == "identity")
                    {
                        return toolsOptions.MySqlStoringConnections.IdentityConnection.ConnectionString;
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }
    }
}
