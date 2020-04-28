using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Repositories.Pages;
using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Recoveries;

namespace LetPortal.Versions.Patches
{
    public class PatchProcessor : IPatchProcessor
    {
        public async Task<IEnumerable<string>> Proceed(
            string folderPath,
            DatabaseOptions databaseOptions)
        {
            // Ensure this folder is exist
            if (!Directory.Exists(folderPath))
            {
                return Enumerable.Empty<string>();
            }
            var allZipFiles = Directory.GetFiles(folderPath, "*.zip");
            var allRunningFiles = new List<string>();
            if (allZipFiles != null && allZipFiles.Length > 0)
            {
                if (Directory.Exists("Temp"))
                {
                    Directory.Delete("Temp", true);
                }

                Directory.CreateDirectory("Temp");

                foreach (var file in allZipFiles)
                {
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                    var extractFolder = Path.Combine("Temp", fileNameWithoutExt);
                    ZipFile.ExtractToDirectory(file, extractFolder);

                    var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(File.ReadAllText(Path.Combine(extractFolder, fileNameWithoutExt + ".json")));

                    foreach (var chainingFile in backupFlatternModel.ChainingFiles)
                    {
                        switch (chainingFile)
                        {
                            case BackupService.APP_FILE:
                                var appFilePath = Path.Combine(extractFolder, BackupService.APP_FILE);
                                var appsListString = File.ReadAllText(appFilePath);
                                var appsList = ConvertUtil.DeserializeObject<IEnumerable<App>>(appsListString);
                                IAppRepository appRepository;
                                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    appRepository = new AppMongoRepository(new MongoConnection(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                else
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    appRepository = new AppEFRepository(new Portal.Repositories.PortalDbContext(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }

                                var appServiceProvider = new InternalAppServiceProvider(appRepository);
                                await appServiceProvider.ForceUpdateApps(appsList);
                                appServiceProvider.Dispose();
                                break;
                            case BackupService.STANDARD_FILE:
                                var standardFilePath = Path.Combine(extractFolder, BackupService.STANDARD_FILE);
                                var standardsString = File.ReadAllText(standardFilePath);
                                var standardsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                                IStandardRepository standardRepository;
                                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    standardRepository = new StandardMongoRepository(new MongoConnection(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                else
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    standardRepository = new StandardEFRepository(new Portal.Repositories.PortalDbContext(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                var standardServiceProvider = new InternalStandardServiceProvider(standardRepository);
                                await standardServiceProvider.ForceUpdateStandards(standardsList);
                                standardServiceProvider.Dispose();
                                break;
                            case BackupService.CHART_FILE:
                                var chartFilePath = Path.Combine(extractFolder, BackupService.CHART_FILE);
                                var chartsString = File.ReadAllText(chartFilePath);
                                var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                                IChartRepository chartRepository;
                                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    chartRepository = new ChartMongoRepository(new MongoConnection(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                else
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    chartRepository = new ChartEFRepository(new Portal.Repositories.PortalDbContext(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                var chartServiceProvider = new InternalChartServiceProvider(chartRepository);
                                await chartServiceProvider.ForceUpdateCharts(chartsList);
                                chartServiceProvider.Dispose();
                                break;
                            case BackupService.DATABASE_FILE:
                                var databaseFilePath = Path.Combine(extractFolder, BackupService.DATABASE_FILE);
                                var databasesString = File.ReadAllText(databaseFilePath);
                                var databasesList = ConvertUtil.DeserializeObject<IEnumerable<DatabaseConnection>>(databasesString);
                                IDatabaseRepository databaseRepository;
                                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    databaseRepository = new DatabaseMongoRepository(new MongoConnection(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                else
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    databaseRepository = new DatabaseEFRepository(new PortalDbContext(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                var databaseServiceProvider = new InternalDatabaseServiceProvider(new DatabaseService(null, null), databaseRepository);
                                await databaseServiceProvider.ForceUpdateDatabases(databasesList);
                                databaseServiceProvider.Dispose();
                                break;
                            case BackupService.PAGE_FILE:
                                var pageFilePath = Path.Combine(extractFolder, BackupService.PAGE_FILE);
                                var pagesString = File.ReadAllText(pageFilePath);
                                var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                                IPageRepository pageRepository;
                                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    pageRepository = new PageMongoRepository(new MongoConnection(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                else
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    pageRepository = new PageEFRepository(new PortalDbContext(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                var pageServiceProvider = new InternalPageServiceProvider(pageRepository, null, null, null);
                                await pageServiceProvider.ForceUpdatePages(pagesList);
                                pageServiceProvider.Dispose();
                                break;
                            case BackupService.DYNAMICLIST_FILE:
                                var dynamicListFilePath = Path.Combine(extractFolder, BackupService.DYNAMICLIST_FILE);
                                var dynamicListString = File.ReadAllText(dynamicListFilePath);
                                var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                                IDynamicListRepository dynamicListRepository;
                                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    dynamicListRepository = new DynamicListMongoRepository(new MongoConnection(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                else
                                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                                    dynamicListRepository = new DynamicListEFRepository(new PortalDbContext(databaseOptions));
#pragma warning restore CA2000 // Dispose objects before losing scope
                                }
                                var dynamicListServiceProvider = new InternalDynamicListServiceProvider(dynamicListRepository);
                                await dynamicListServiceProvider.ForceUpdateDynamicLists(dynamicListsList);
                                dynamicListServiceProvider.Dispose();
                                break;
                            default:
                                break;
                        }
                    }

                    allRunningFiles.Add(fileNameWithoutExt + ".zip");
                }
            }

            return allRunningFiles;
        }
    }
}
