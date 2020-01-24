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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Versions.Patches
{
    public class PatchProcessor : IPatchProcessor
    {
        public async Task<IEnumerable<string>> Proceed(
            string folderPath,
            DatabaseOptions databaseOptions)
        {
            // Ensure this folder is exist
            if(!Directory.Exists(folderPath))
            {
                return Enumerable.Empty<string>();
            }
            var allZipFiles = Directory.GetFiles(folderPath, "*.zip");
            var allRunningFiles = new List<string>();
            if(allZipFiles != null && allZipFiles.Length > 0)
            {
                if(Directory.Exists("Temp"))
                {
                    Directory.Delete("Temp", true);
                }

                Directory.CreateDirectory("Temp");

                foreach(var file in allZipFiles)
                {
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                    var extractFolder = Path.Combine("Temp", fileNameWithoutExt);
                    ZipFile.ExtractToDirectory(file, extractFolder);

                    var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(File.ReadAllText(Path.Combine(extractFolder, fileNameWithoutExt + ".json")));

                    foreach(var chainingFile in backupFlatternModel.ChainingFiles)
                    {
                        switch(chainingFile)
                        {
                            case BackupService.APP_FILE:
                                var appFilePath = Path.Combine(extractFolder, BackupService.APP_FILE);
                                var appsListString = File.ReadAllText(appFilePath);
                                var appsList = ConvertUtil.DeserializeObject<IEnumerable<App>>(appsListString);
                                IAppRepository appRepository;
                                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
                                    appRepository = new AppMongoRepository(new MongoConnection(databaseOptions));
                                }
                                else
                                {
                                    appRepository = new AppEFRepository(new Portal.Repositories.LetPortalDbContext(databaseOptions));
                                }

                                var appServiceProvider = new InternalAppServiceProvider(appRepository);
                                await appServiceProvider.ForceUpdateApps(appsList);
                                break;
                            case BackupService.STANDARD_FILE:
                                var standardFilePath = Path.Combine(extractFolder, BackupService.STANDARD_FILE);
                                var standardsString = File.ReadAllText(standardFilePath);
                                var standardsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                                IStandardRepository standardRepository;
                                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
                                    standardRepository = new StandardMongoRepository(new MongoConnection(databaseOptions));
                                }
                                else
                                {
                                    standardRepository = new StandardEFRepository(new Portal.Repositories.LetPortalDbContext(databaseOptions));
                                }
                                var standardServiceProvider = new InternalStandardServiceProvider(standardRepository);
                                await standardServiceProvider.ForceUpdateStandards(standardsList);
                                break;
                            case BackupService.CHART_FILE:
                                var chartFilePath = Path.Combine(extractFolder, BackupService.CHART_FILE);
                                var chartsString = File.ReadAllText(chartFilePath);
                                var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                                IChartRepository chartRepository;
                                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
                                    chartRepository = new ChartMongoRepository(new MongoConnection(databaseOptions));
                                }
                                else
                                {
                                    chartRepository = new ChartEFRepository(new Portal.Repositories.LetPortalDbContext(databaseOptions));
                                }
                                var chartServiceProvider = new InternalChartServiceProvider(chartRepository);
                                await chartServiceProvider.ForceUpdateCharts(chartsList);
                                break;
                            case BackupService.DATABASE_FILE:
                                var databaseFilePath = Path.Combine(extractFolder, BackupService.DATABASE_FILE);
                                var databasesString = File.ReadAllText(databaseFilePath);
                                var databasesList = ConvertUtil.DeserializeObject<IEnumerable<DatabaseConnection>>(databasesString);
                                IDatabaseRepository databaseRepository;
                                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
                                    databaseRepository = new DatabaseMongoRepository(new MongoConnection(databaseOptions));
                                }
                                else
                                {
                                    databaseRepository = new DatabaseEFRepository(new LetPortalDbContext(databaseOptions));
                                }
                                var databaseServiceProvider = new InternalDatabaseServiceProvider(new DatabaseService(null, null), databaseRepository);
                                await databaseServiceProvider.ForceUpdateDatabases(databasesList);
                                break;
                            case BackupService.PAGE_FILE:
                                var pageFilePath = Path.Combine(extractFolder, BackupService.PAGE_FILE);
                                var pagesString = File.ReadAllText(pageFilePath);
                                var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                                IPageRepository pageRepository;
                                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
                                    pageRepository = new PageMongoRepository(new MongoConnection(databaseOptions));
                                }
                                else
                                {
                                    pageRepository = new PageEFRepository(new LetPortalDbContext(databaseOptions));
                                }
                                var pageServiceProvider = new InternalPageServiceProvider(pageRepository);
                                await pageServiceProvider.ForceUpdatePages(pagesList);
                                break;
                            case BackupService.DYNAMICLIST_FILE:
                                var dynamicListFilePath = Path.Combine(extractFolder, BackupService.DYNAMICLIST_FILE);
                                var dynamicListString = File.ReadAllText(dynamicListFilePath);
                                var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                                IDynamicListRepository dynamicListRepository;
                                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                                {
                                    dynamicListRepository = new DynamicListMongoRepository(new MongoConnection(databaseOptions));
                                }
                                else
                                {
                                    dynamicListRepository = new DynamicListEFRepository(new LetPortalDbContext(databaseOptions));
                                }
                                var dynamicListServiceProvider = new InternalDynamicListServiceProvider(dynamicListRepository);
                                await dynamicListServiceProvider.ForceUpdateDynamicLists(dynamicListsList);
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
