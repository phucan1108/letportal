using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Tools;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Identity;
using LetPortal.Identity.Repositories;
using LetPortal.Portal;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Repositories;
using LetPortal.Tools;
using LetPortal.Versions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

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

        [Option("-pn|--patch-name", Description = "Support only which command `letportal {app} install -pn {patch-name}`")]
        public string PatchName { get; set; }

        [Argument(0, Name = "app", Description = "Supports: portal | identity")]
        public string App { get; set; }

        [Argument(1, Name = "command", Description = "Supports: info | install | uninstall | up | down | pack | unpack | patch")]
        public string Command { get; set; }

        [Argument(2, Name = "version", Description = "Version number is mandatory to run with some commands. Ex: 0.0.1")]
        public string VersionNumber { get; set; }

        private IEnumerable<IToolPlugin> toolPlugins;
        private IEnumerable<IPortalPatchFeatureTool> portalPatches;
        private IEnumerable<IIdentityPatchFeatureTool> identityPatches;

        private IConfiguration configuration;

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
            var toolsOption = GetToolsOptions(ConfigFilePath);
            LoadPlugins(toolsOption.RequiredPlugins);
            LoadPatchs(toolsOption.RequiredPatchs);
            toolPlugins = GetInstalledPlugins(toolsOption.RequiredPlugins);
            portalPatches = GetInstalledPortalPatchs(toolsOption.RequiredPatchs);
            identityPatches = GetInstalledIdentityPatchs(toolsOption.RequiredPatchs);
            bool isRunningPlugin = !IsPortal() && !IsIdentity();

            // Check if run with mode patch
            bool isPatchRunning = false;
            
            if(!isRunningPlugin && !string.IsNullOrEmpty(PatchName) && Command != "install")
            {
                throw new NotSupportedException("Sorry, we only support -pn with install command.");
            }
            else if(!isRunningPlugin && !string.IsNullOrEmpty(PatchName) && Command == "install")
            {
                isPatchRunning = true;
            }

            
            IToolPlugin toolPlugin = isRunningPlugin ? toolPlugins.First(a => a.AppName == App) : null;            
            var services = new ServiceCollection();
            ConventionPackDefault.Register();
            MongoDbRegistry.RegisterEntities();
            var dbType = DatabseType.ToEnum<ConnectionType>(true);
            var databaseOption = 
                new DatabaseOptions
                {
                    ConnectionString = !string.IsNullOrEmpty(Connection)
                        ? Connection 
                            : (isRunningPlugin ? 
                                    toolPlugin.LoadDefaultDatabase().ConnectionString 
                                    : GetDefaultConnectionString(dbType, App, toolsOption)),
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
                        var patchMongoRepository = new PatchVersionMongoRepository(mongoConnection);
                        mongoVersionContext.ConnectionType = ConnectionType.MongoDB;
                        mongoVersionContext.DatabaseOptions = databaseOption;
                        mongoVersionContext.PortalDatabaseOptions = toolsOption.MongoStoringConnections.PortalConnection;
                        mongoVersionContext.ServiceManagementOptions = storingConnections.ServiceManagementConnection;
                        mongoVersionContext.IdentityDbOptions = storingConnections.IdentityConnection;
                        var latestVersion = await versionMongoRepository.GetLastestVersion(App);

                        var allVersions = Enumerable.Empty<IVersion>();
                        IServiceProvider serviceProvider = null;
                        PatchVersion latestPatchVersion = null;
                        IPortalPatchFeatureTool portalPatchFeatureTool = null;
                        IIdentityPatchFeatureTool identityPatchFeatureTool = null;
                        if (IsPortal())
                        {
                            RegisterServicesForPortal(services, databaseOption);
                            if (isPatchRunning)
                            {
                                latestPatchVersion = await patchMongoRepository.GetLatestAsync(App, PatchName);
                                portalPatchFeatureTool = portalPatches.First(a => a.PatchName == PatchName);
                                portalPatchFeatureTool.RegisterServices(services, configuration);
                            }
                            serviceProvider = services.BuildServiceProvider();
                            allVersions = Scanner.GetAllPortalVersions(serviceProvider);
                        }
                        else if (IsIdentity())
                        {
                            RegisterServicesForIdentity(services, databaseOption);
                            if (isPatchRunning)
                            {
                                latestPatchVersion = await patchMongoRepository.GetLatestAsync(App, PatchName);
                                identityPatchFeatureTool = identityPatches.First(a => a.PatchName == PatchName);
                                identityPatchFeatureTool.RegisterServices(services, configuration);
                            }
                            serviceProvider = services.BuildServiceProvider();
                            allVersions = Scanner.GetAllIdentityVersions(serviceProvider);
                        }
                        else
                        {
                            var foundPlugin = toolPlugins.First(a => a.AppName.Equals(App, StringComparison.OrdinalIgnoreCase));
                            foundPlugin.RegisterServices(services, configuration);
                            serviceProvider = services.BuildServiceProvider();
                            allVersions = foundPlugin.GetAllVersions(serviceProvider);
                        }
                        toolsContext = new ToolsContext
                        {
                            LatestVersion = latestVersion,
                            LatestPatchVersion = latestPatchVersion,
                            VersionContext = mongoVersionContext,
                            VersionNumber = VersionNumber,
                            Versions = allVersions,
                            VersionRepository = versionMongoRepository,
                            PatchesFolder = PatchesFolder,
                            AllowPatch = !string.IsNullOrEmpty(PatchesFolder),
                            Services = serviceProvider,
                            PatchVersionRepository = patchMongoRepository,
                            CurrentPatchPortal = portalPatchFeatureTool,
                            CurrentIdentityPortal = identityPatchFeatureTool,
                            Arguments = CombineArguments()
                        };

                        break;
                    case ConnectionType.PostgreSQL:
                    case ConnectionType.MySQL:
                    case ConnectionType.SQLServer:

#pragma warning disable CA2000 // Dispose objects before losing scope
                        var letportalContext = new SaturnFullContext(databaseOption);
#pragma warning restore CA2000 // Dispose objects before losing scope
                        letportalContext.Database.EnsureCreated();

#pragma warning disable CA2000 // Dispose objects before losing scope
                        var letportalContextForRepo = new SaturnFullContext(databaseOption);
#pragma warning restore CA2000 // Dispose objects before losing scope
                        var sqlEFVersionContext = new EFVersionContext(letportalContext)
                        {
                            ConnectionType = dbType,
                            DatabaseOptions = databaseOption
                        };
                        var patchEFRepository = new PatchVersionEFRepository(letportalContextForRepo);
                        var portalVersionRepository = new VersionEFRepository(letportalContextForRepo);
                        var latestVersionEF = await portalVersionRepository.GetLastestVersion(App);
                        sqlEFVersionContext.ServiceManagementOptions = storingConnections.ServiceManagementConnection;
                        sqlEFVersionContext.IdentityDbOptions = storingConnections.IdentityConnection;
                        sqlEFVersionContext.PortalDatabaseOptions = storingConnections.PortalConnection;
                        var sqlAllVersions = Enumerable.Empty<IVersion>();
                        IServiceProvider sqlServiceProvider = null;
                        PatchVersion latestEFPatchVersion = null;
                        IPortalPatchFeatureTool portalEFPatchFeatureTool = null;
                        IIdentityPatchFeatureTool identityEFPatchFeatureTool = null;

                        if (IsPortal())
                        {
                            RegisterServicesForPortal(services, databaseOption);
                            if (isPatchRunning)
                            {
                                latestEFPatchVersion = await patchEFRepository.GetLatestAsync(App, PatchName);
                                portalEFPatchFeatureTool = portalPatches.First(a => a.PatchName == PatchName);
                                portalEFPatchFeatureTool.RegisterServices(services, configuration);
                            }
                            sqlServiceProvider = services.BuildServiceProvider();
                            sqlAllVersions = Scanner.GetAllPortalVersions(sqlServiceProvider);                              
                        }
                        else if (IsIdentity())
                        {  
                            RegisterServicesForIdentity(services, databaseOption);
                            if (isPatchRunning)
                            {
                                latestEFPatchVersion = await patchEFRepository.GetLatestAsync(App, PatchName);
                                identityEFPatchFeatureTool = identityPatches.First(a => a.PatchName == PatchName);
                                identityEFPatchFeatureTool.RegisterServices(services, configuration);
                            }
                            sqlServiceProvider = services.BuildServiceProvider();
                            sqlAllVersions = Scanner.GetAllIdentityVersions(sqlServiceProvider);
                            
                        }
                        else
                        {
                            var foundPlugin = toolPlugins.First(a => a.AppName.Equals(App, StringComparison.OrdinalIgnoreCase));
                            foundPlugin.RegisterServices(services, configuration);
                            sqlServiceProvider = services.BuildServiceProvider();
                            sqlAllVersions = foundPlugin.GetAllVersions(sqlServiceProvider);
                        }
                        toolsContext = new ToolsContext
                        {
                            LatestVersion = latestVersionEF,
                            VersionContext = sqlEFVersionContext,
                            VersionNumber = VersionNumber,
                            Versions = sqlAllVersions,
                            VersionRepository = portalVersionRepository,
                            AllowPatch = !string.IsNullOrEmpty(PatchesFolder),
                            Services = sqlServiceProvider,
                            Arguments = CombineArguments(),
                            PatchesFolder = PatchesFolder,
                            PatchVersionRepository = patchEFRepository,
                            CurrentPatchPortal = portalEFPatchFeatureTool,
                            CurrentIdentityPortal = identityEFPatchFeatureTool
                        };

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
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                services.AddTransient(a => new MongoConnection(databaseOptions));
            }
            else
            {
                services.AddTransient(a => databaseOptions);
            }

            PortalExtensions.RegisterRepos(services, databaseOptions, true);
            PortalExtensions.RegisterProviders(services);
            PortalExtensions.RegisterServices(services);
        }

        private void RegisterServicesForIdentity(IServiceCollection services, DatabaseOptions databaseOptions)
        {
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                services.AddTransient(a => new MongoConnection(databaseOptions));
            }
            else
            {
                services.AddTransient(a => databaseOptions);
            }

            IdentityExtensions.RegisterRepos(services, databaseOptions);
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
                Output = Output ,
                PatchName = PatchName
            };
        }

        private IEnumerable<IToolPlugin> GetInstalledPlugins(IEnumerable<string> requiredPlugins)
        {
            var matchedAssemblies = AssemblyLoadContext.Default
                                .Assemblies
                                .Where(a => requiredPlugins.Contains(a.GetName().Name)).ToList();

            var matchedTypes = matchedAssemblies.SelectMany(b => b.GetTypes().Where(c => c.GetInterface(typeof(IToolPlugin).Name) != null)).ToList();
            return matchedTypes.Select(d => Activator.CreateInstance(d) as IToolPlugin);
        }

        private void LoadPlugins(IEnumerable<string> requiredPlugins)
        {
            foreach(var pluginPath in requiredPlugins)
            {
                try
                {
                    var curretnExecutedDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(curretnExecutedDirectory, pluginPath + ".dll"));
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Can't load plugin in the path {0}", pluginPath);
                    Console.WriteLine(ex.ToString());
                }                   
            }            
        }

        private void LoadPatchs(IEnumerable<string> requiredPatchs)
        {
            foreach (var patchPath in requiredPatchs)
            {
                try
                {
                    var curretnExecutedDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(curretnExecutedDirectory, patchPath + ".dll"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can't load patch in the path {0}", patchPath);
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private IEnumerable<IPortalPatchFeatureTool> GetInstalledPortalPatchs(IEnumerable<string> requiredPatchs)
        {
            var matchedAssemblies = AssemblyLoadContext.Default
                                .Assemblies
                                .Where(a => requiredPatchs.Contains(a.GetName().Name)).ToList();

            var matchedTypes = matchedAssemblies.SelectMany(b => b.GetTypes().Where(c => c.GetInterface(typeof(IPortalPatchFeatureTool).Name) != null)).ToList();
            return matchedTypes.Select(d => Activator.CreateInstance(d) as IPortalPatchFeatureTool);
        }

        private IEnumerable<IIdentityPatchFeatureTool> GetInstalledIdentityPatchs(IEnumerable<string> requiredPatchs)
        {
            var matchedAssemblies = AssemblyLoadContext.Default
                                .Assemblies
                                .Where(a => requiredPatchs.Contains(a.GetName().Name)).ToList();

            var matchedTypes = matchedAssemblies.SelectMany(b => b.GetTypes().Where(c => c.GetInterface(typeof(IIdentityPatchFeatureTool).Name) != null)).ToList();
            return matchedTypes.Select(d => Activator.CreateInstance(d) as IIdentityPatchFeatureTool);
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
            var configurationBuilder = new ConfigurationBuilder();
            configuration = configurationBuilder.AddJsonFile(fullPath).Build();
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
                    if (app == "portal")
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
