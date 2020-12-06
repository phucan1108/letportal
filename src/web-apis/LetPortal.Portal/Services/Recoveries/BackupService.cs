using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Recoveries;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Exceptions.Recoveries;
using LetPortal.Portal.Models.Recoveries;
using LetPortal.Portal.Options.Recoveries;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.CompositeControls;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Recoveries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Portal.Services.Recoveries
{
    public class BackupService : IBackupService
    {
        public const string APP_FILE = "apps.json";
        public const string CHART_FILE = "charts.json";
        public const string DATABASE_FILE = "databases.json";
        public const string STANDARD_FILE = "standards.json";
        public const string TREE_FILE = "tree.json";
        public const string ARRAY_FILE = "array.json";
        public const string DYNAMICLIST_FILE = "dynamiclists.json";
        public const string PAGE_FILE = "pages.json";
        public const string COMPOSITE_CONTROL_FILE = "compositecontrols.json";

        private readonly IAppServiceProvider _appServiceProvider;

        private readonly IStandardServiceProvider _standardServiceProvider;

        private readonly IChartServiceProvider _chartServiceProvider;

        private readonly IDynamicListServiceProvider _dynamicListServiceProvider;

        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IPageServiceProvider _pageServiceProvider;

        private readonly IFileSeviceProvider _fileSeviceProvider;

        private readonly IBackupRepository _backupRepository;

        private readonly ICompositeControlServiceProvider _compositeControlServiceProvider;

        private readonly IOptionsMonitor<BackupOptions> _backupOptions;

        private readonly IServiceLogger<BackupService> _logger;

        public BackupService(
            IAppServiceProvider appServiceProvider,
            IStandardServiceProvider standardServiceProvider,
            IChartServiceProvider chartServiceProvider,
            IDynamicListServiceProvider dynamicListServiceProvider,
            IDatabaseServiceProvider databaseServiceProvider,
            IPageServiceProvider pageServiceProvider,
            IFileSeviceProvider fileSeviceProvider,
            IBackupRepository backupRepository,
            ICompositeControlServiceProvider compositeControlServiceProvider,
            IOptionsMonitor<BackupOptions> backupOptions,
            IServiceLogger<BackupService> logger
            )
        {
            _appServiceProvider = appServiceProvider;
            _standardServiceProvider = standardServiceProvider;
            _chartServiceProvider = chartServiceProvider;
            _dynamicListServiceProvider = dynamicListServiceProvider;
            _databaseServiceProvider = databaseServiceProvider;
            _fileSeviceProvider = fileSeviceProvider;
            _pageServiceProvider = pageServiceProvider;
            _backupRepository = backupRepository;
            _compositeControlServiceProvider = compositeControlServiceProvider;
            _backupOptions = backupOptions;
            _logger = logger;
        }

        public async Task<BackupResponseModel> CreateBackupFile(BackupRequestModel model)
        {
            Directory.CreateDirectory(_backupOptions.CurrentValue.BackupFolderPath);
            // Ensure a total number of objects less than MaximumObjects
            var appCount = model.Apps != null ? model.Apps.Count() : 0;
            var standardCount = model.Standards != null ? model.Standards.Count() : 0;
            var treeCount = model.Tree != null ? model.Tree.Count() : 0;
            var arrayCount = model.Array != null ? model.Array.Count() : 0;
            var chartCount = model.Charts != null ? model.Charts.Count() : 0;
            var dynamicCount = model.DynamicLists != null ? model.DynamicLists.Count() : 0;
            var databaseCount = model.Databases != null ? model.Databases.Count() : 0;
            var pageCount = model.Pages != null ? model.Pages.Count() : 0;
            var compositeControlCount = model.CompositeControls != null ? model.CompositeControls.Count() : 0;

            var totalBackupCount =
                appCount
                + standardCount
                + treeCount
                + arrayCount
                + chartCount
                + dynamicCount
                + databaseCount
                + pageCount
                + compositeControlCount;

            if (totalBackupCount > _backupOptions.CurrentValue.MaximumObjects)
            {
                throw new BackupException(BackupErrorCodes.ReachMaximumBackupObjects);
            }
            var collectApp = _appServiceProvider.GetAppsByIds(model.Apps);
            var collectStandards = _standardServiceProvider.GetStandardComponentsByIds(model.Standards);
            var collectTree = _standardServiceProvider.GetStandardComponentsByIds(model.Tree);
            var collectArray = _standardServiceProvider.GetStandardComponentsByIds(model.Array);
            var collectCharts = _chartServiceProvider.GetChartsByIds(model.Charts);
            var collectDynamicLists = _dynamicListServiceProvider.GetDynamicListsByIds(model.DynamicLists);
            var collectDatabases = _databaseServiceProvider.GetDatabaseConnectionsByIds(model.Databases);
            var collectPages = _pageServiceProvider.GetPagesByIds(model.Pages);
            var collectCompositeControls = _compositeControlServiceProvider.GetByIds(model.CompositeControls);

            await Task.WhenAll(
                collectApp,
                collectStandards,
                collectTree,
                collectArray,
                collectCharts,
                collectDatabases,
                collectDynamicLists,
                collectPages,
                collectCompositeControls);

            var backupFileModel = new BackupFlatternFileModel
            {
                TotalObjects = totalBackupCount,
                ChainingFiles = new List<string>()
            };

            var backup = new Backup
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = model.Name,
                Description = model.Description,
                CreatedDate = DateTime.UtcNow,
                Creator = model.Creator,
                BackupElements = new BackupElements
                {
                    Apps = model.Apps,
                    Charts = model.Charts,
                    Databases = model.Databases,
                    DynamicLists = model.DynamicLists,
                    Pages = model.Pages,
                    Standards = model.Standards,
                    Array = model.Array,
                    Tree = model.Tree,
                    CompositeControls = model.CompositeControls
                }
            };
            backupFileModel.Backup = backup;

            // Write to file
            var fileName = DateTime.UtcNow.Ticks.ToString();
            var jsonFilePath = !string.IsNullOrEmpty(_backupOptions.CurrentValue.BackupFolderPath) ? _backupOptions.CurrentValue.BackupFolderPath : Environment.CurrentDirectory;
            var jsonFileName = fileName + ".json";
            jsonFilePath = Path.Combine(jsonFilePath, fileName);
            Directory.CreateDirectory(jsonFilePath);

            if (collectApp.Result != null)
            {
                var jsonApps = ConvertUtil.SerializeObject(collectApp.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, APP_FILE)))
                {
                    sw.Write(jsonApps);
                }

                backupFileModel.ChainingFiles.Add(APP_FILE);
            }

            if (collectStandards.Result != null)
            {
                var jsonStandards = ConvertUtil.SerializeObject(collectStandards.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, STANDARD_FILE)))
                {
                    sw.Write(jsonStandards);
                }
                backupFileModel.ChainingFiles.Add(STANDARD_FILE);
            }

            if (collectTree.Result != null)
            {
                var jsonTree = ConvertUtil.SerializeObject(collectTree.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, TREE_FILE)))
                {
                    sw.Write(jsonTree);
                }
                backupFileModel.ChainingFiles.Add(TREE_FILE);
            }

            if (collectArray.Result != null)
            {
                var jsonArray = ConvertUtil.SerializeObject(collectArray.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, ARRAY_FILE)))
                {
                    sw.Write(jsonArray);
                }
                backupFileModel.ChainingFiles.Add(ARRAY_FILE);
            }

            if (collectDynamicLists.Result != null)
            {
                var jsonDynamicLists = ConvertUtil.SerializeObject(collectDynamicLists.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, DYNAMICLIST_FILE)))
                {
                    sw.Write(jsonDynamicLists);
                }
                backupFileModel.ChainingFiles.Add(DYNAMICLIST_FILE);
            }

            if (collectDatabases.Result != null)
            {
                var jsonDatabases = ConvertUtil.SerializeObject(collectDatabases.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, DATABASE_FILE)))
                {
                    sw.Write(jsonDatabases);
                }
                backupFileModel.ChainingFiles.Add(DATABASE_FILE);
            }

            if (collectCharts.Result != null)
            {
                var jsonCharts = ConvertUtil.SerializeObject(collectCharts.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, CHART_FILE)))
                {
                    sw.Write(jsonCharts);
                }
                backupFileModel.ChainingFiles.Add(CHART_FILE);
            }

            if (collectPages.Result != null)
            {
                var jsonPages = ConvertUtil.SerializeObject(collectPages.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, PAGE_FILE)))
                {
                    sw.Write(jsonPages);
                }
                backupFileModel.ChainingFiles.Add(PAGE_FILE);
            }

            if (collectCompositeControls.Result != null)
            {
                var jsonPages = ConvertUtil.SerializeObject(collectCompositeControls.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, COMPOSITE_CONTROL_FILE)))
                {
                    sw.Write(jsonPages);
                }
                backupFileModel.ChainingFiles.Add(COMPOSITE_CONTROL_FILE);
            }

            var jsonFlattern = ConvertUtil.SerializeObject(backupFileModel, true);

            using (var sw = new StreamWriter(Path.Combine(jsonFilePath, jsonFileName)))
            {
                sw.Write(jsonFlattern);
            }

            ZipFile.CreateFromDirectory(jsonFilePath, Path.Combine(_backupOptions.CurrentValue.BackupFolderPath, fileName + ".zip"));

            // Store zip file into file server, allow to create zip file when downloading
            var uploadResponse = await _fileSeviceProvider
                .UploadFileAsync(
                    Path.Combine(_backupOptions.CurrentValue.BackupFolderPath, fileName + ".zip"),
                    model.Creator,
                    true);

            backup.FileId = uploadResponse.FileId;
            backup.DownloadableUrl = uploadResponse.DownloadableUrl;
            await _backupRepository.AddAsync(backup);
            return new BackupResponseModel { DownloadableUrl = uploadResponse.DownloadableUrl };
        }

        public async Task<UploadBackupResponseModel> UploadBackupFile(IFormFile uploadFile, string uploader)
        {
            var isFileValid = await _fileSeviceProvider.ValidateFile(uploadFile);
            if (isFileValid)
            {
                Directory.CreateDirectory(_backupOptions.CurrentValue.RestoreFolderPath);
                var tempFilePath = await SaveFormFileAsync(uploadFile, _backupOptions.CurrentValue.RestoreFolderPath);
                var unzipFileName = Path.GetFileNameWithoutExtension(tempFilePath);
                var unzipFolderPath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, Path.GetFileNameWithoutExtension(tempFilePath));
                if (Directory.Exists(unzipFolderPath))
                {
                    // Delete old directory
                    Directory.Delete(unzipFolderPath, true);
                }

                Directory.CreateDirectory(unzipFolderPath);
                ZipFile.ExtractToDirectory(tempFilePath, unzipFolderPath);
                var jsonFilePath = Path.Combine(unzipFolderPath, unzipFileName + ".json");
                var jsonFound = File.ReadAllText(jsonFilePath);

                var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(jsonFound);
                // Save zip file into file service
                var storedFile = await _fileSeviceProvider.UploadFileAsync(tempFilePath, uploader, true);

                // Restore backup 
                var backup = backupFlatternModel.Backup;
                backup.Id = DataUtil.GenerateUniqueId();
                backup.FileId = storedFile.FileId;
                backup.DownloadableUrl = storedFile.DownloadableUrl;
                await _backupRepository.AddAsync(backup);

                return new UploadBackupResponseModel
                {
                    Id = backup.Id,
                    Name = backup.Name,
                    Description = backup.Description,
                    Creator = backup.Creator,
                    CreatedDate = backup.CreatedDate,
                    TotalObjects = backupFlatternModel.TotalObjects,
                    IsFileValid = true
                };
            }
            else
            {
                return new UploadBackupResponseModel { IsFileValid = false };
            }
        }

        public async Task<PreviewRestoreModel> PreviewBackup(string backupId)
        {
            var previewModel = new PreviewRestoreModel();
            var backup = await _backupRepository.GetOneAsync(backupId);

            var zipFile = await _fileSeviceProvider.DownloadFileAsync(backup.FileId);

            var fileNameWithoutExt = FileUtil.GetFileNameWithoutExt(zipFile.FileName);
            var restoreFilePath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, zipFile.FileName);
            Directory.CreateDirectory(_backupOptions.CurrentValue.RestoreFolderPath);
            using (var fileStream = File.Create(restoreFilePath))
            {
                fileStream.Write(zipFile.FileBytes, 0, zipFile.FileBytes.Length);
            }

            // Release file in memory
            zipFile.FileBytes = null;
            var folderExtractingPath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, fileNameWithoutExt);
            if (Directory.Exists(folderExtractingPath))
            {
                Directory.Delete(folderExtractingPath, true);
            }
            ZipFile.ExtractToDirectory(restoreFilePath, folderExtractingPath);

            var jsonBackupFilePath = Path.Combine(folderExtractingPath, fileNameWithoutExt + ".json");

            var jsonBackupString = File.ReadAllText(jsonBackupFilePath);
            var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(jsonBackupString);

            foreach (var chainingFile in backupFlatternModel.ChainingFiles)
            {
                switch (chainingFile)
                {
                    case APP_FILE:
                        var appFilePath = Path.Combine(folderExtractingPath, APP_FILE);
                        var appsListString = File.ReadAllText(appFilePath);
                        var appsList = ConvertUtil.DeserializeObject<IEnumerable<App>>(appsListString);
                        previewModel.Apps = await _appServiceProvider.CompareEntities(appsList);
                        break;
                    case STANDARD_FILE:
                        var standardFilePath = Path.Combine(folderExtractingPath, STANDARD_FILE);
                        var standardsString = File.ReadAllText(standardFilePath);
                        var standarsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                        previewModel.Standards = await _standardServiceProvider.CompareStandardComponent(standarsList);
                        break;
                    case TREE_FILE:
                        var treeFilePath = Path.Combine(folderExtractingPath, TREE_FILE);
                        var treeString = File.ReadAllText(treeFilePath);
                        var treeList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(treeString);
                        previewModel.Tree = await _standardServiceProvider.CompareStandardComponent(treeList);
                        break;
                    case ARRAY_FILE:
                        var arrayFilePath = Path.Combine(folderExtractingPath, ARRAY_FILE);
                        var arrayString = File.ReadAllText(arrayFilePath);
                        var arrayList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(arrayString);
                        previewModel.Array = await _standardServiceProvider.CompareStandardComponent(arrayList);
                        break;
                    case CHART_FILE:
                        var chartFilePath = Path.Combine(folderExtractingPath, CHART_FILE);
                        var chartsString = File.ReadAllText(chartFilePath);
                        var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                        previewModel.Charts = await _chartServiceProvider.CompareCharts(chartsList);
                        break;
                    case DATABASE_FILE:
                        var databaseFilePath = Path.Combine(folderExtractingPath, DATABASE_FILE);
                        var databasesString = File.ReadAllText(databaseFilePath);
                        var databasesList = ConvertUtil.DeserializeObject<IEnumerable<DatabaseConnection>>(databasesString);
                        previewModel.Databases = await _databaseServiceProvider.CompareDatabases(databasesList);
                        break;
                    case PAGE_FILE:
                        var pageFilePath = Path.Combine(folderExtractingPath, PAGE_FILE);
                        var pagesString = File.ReadAllText(pageFilePath);
                        var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                        previewModel.Pages = await _pageServiceProvider.ComparePages(pagesList);
                        break;
                    case DYNAMICLIST_FILE:
                        var dynamicListFilePath = Path.Combine(folderExtractingPath, DYNAMICLIST_FILE);
                        var dynamicListString = File.ReadAllText(dynamicListFilePath);
                        var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                        previewModel.DynamicLists = await _dynamicListServiceProvider.CompareDynamicLists(dynamicListsList);
                        break;
                    case COMPOSITE_CONTROL_FILE:
                        var compositeFilePath = Path.Combine(folderExtractingPath, COMPOSITE_CONTROL_FILE);
                        var compositeString = File.ReadAllText(compositeFilePath);
                        var compositeControlsList = ConvertUtil.DeserializeObject<IEnumerable<CompositeControl>>(compositeString);
                        previewModel.CompositeControls = await _compositeControlServiceProvider.Compare(compositeControlsList);
                        break;
                    default:
                        break;
                }
            }

            Directory.Delete(folderExtractingPath, true);
            File.Delete(restoreFilePath);

            previewModel.TotalObjects =
                (previewModel.Apps != null ? previewModel.Apps.Count() : 0) +
                (previewModel.Charts != null ? previewModel.Charts.Count() : 0) +
                (previewModel.Databases != null ? previewModel.Databases.Count() : 0) +
                (previewModel.DynamicLists != null ? previewModel.DynamicLists.Count() : 0) +
                (previewModel.Pages != null ? previewModel.Pages.Count() : 0) +
                (previewModel.Standards != null ? previewModel.Standards.Count() : 0) +
                (previewModel.Tree != null ? previewModel.Tree.Count() : 0) +
                (previewModel.Array != null ? previewModel.Array.Count() : 0) +
                (previewModel.CompositeControls != null ? previewModel.CompositeControls.Count() : 0);

            previewModel.TotalNewObjects =
                (previewModel.Apps != null ? previewModel.Apps.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Charts != null ? previewModel.Charts.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Databases != null ? previewModel.Databases.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.DynamicLists != null ? previewModel.DynamicLists.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Pages != null ? previewModel.Pages.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Standards != null ? previewModel.Standards.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Tree != null ? previewModel.Tree.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Array != null ? previewModel.Array.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.CompositeControls != null ? previewModel.CompositeControls.Count(a => a.IsTotallyNew) : 0);

            previewModel.TotalUnchangedObjects =
                (previewModel.Apps != null ? previewModel.Apps.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Charts != null ? previewModel.Charts.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Databases != null ? previewModel.Databases.Count(a => a.IsUnchanged) : 0) +
                (previewModel.DynamicLists != null ? previewModel.DynamicLists.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Pages != null ? previewModel.Pages.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Standards != null ? previewModel.Standards.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Tree != null ? previewModel.Tree.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Array != null ? previewModel.Array.Count(a => a.IsUnchanged) : 0) +
                (previewModel.CompositeControls != null ? previewModel.CompositeControls.Count(a => a.IsUnchanged) : 0);

            previewModel.TotalChangedObjects =
                previewModel.TotalObjects - previewModel.TotalNewObjects - previewModel.TotalUnchangedObjects;

            return previewModel;
        }

        public async Task RestoreBackupPoint(string backupId)
        {
            var backup = await _backupRepository.GetOneAsync(backupId);

            var zipFile = await _fileSeviceProvider.DownloadFileAsync(backup.FileId);

            var fileNameWithoutExt = FileUtil.GetFileNameWithoutExt(zipFile.FileName);
            var restoreFilePath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, zipFile.FileName);
            Directory.CreateDirectory(_backupOptions.CurrentValue.RestoreFolderPath);
            using (var fileStream = File.Create(restoreFilePath))
            {
                fileStream.Write(zipFile.FileBytes, 0, zipFile.FileBytes.Length);
            }

            // Release file in memory
            zipFile.FileBytes = null;
            var folderExtractingPath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, fileNameWithoutExt);
            if (Directory.Exists(folderExtractingPath))
            {
                Directory.Delete(folderExtractingPath, true);
            }
            ZipFile.ExtractToDirectory(restoreFilePath, folderExtractingPath);

            var jsonBackupFilePath = Path.Combine(folderExtractingPath, fileNameWithoutExt + ".json");

            var jsonBackupString = File.ReadAllText(jsonBackupFilePath);
            var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(jsonBackupString);

            foreach (var chainingFile in backupFlatternModel.ChainingFiles)
            {
                switch (chainingFile)
                {
                    case APP_FILE:
                        var appFilePath = Path.Combine(folderExtractingPath, APP_FILE);
                        var appsListString = File.ReadAllText(appFilePath);
                        var appsList = ConvertUtil.DeserializeObject<IEnumerable<App>>(appsListString);
                        await _appServiceProvider.ForceUpdateApps(appsList);
                        break;
                    case STANDARD_FILE:
                        var standardFilePath = Path.Combine(folderExtractingPath, STANDARD_FILE);
                        var standardsString = File.ReadAllText(standardFilePath);
                        var standardsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                        await _standardServiceProvider.ForceUpdateStandards(standardsList);
                        break;
                    case TREE_FILE:
                        var treeFilePath = Path.Combine(folderExtractingPath, TREE_FILE);
                        var treeString = File.ReadAllText(treeFilePath);
                        var treeList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(treeString);
                        await _standardServiceProvider.ForceUpdateStandards(treeList);
                        break;
                    case ARRAY_FILE:
                        var arrayFilePath = Path.Combine(folderExtractingPath, ARRAY_FILE);
                        var arrayString = File.ReadAllText(arrayFilePath);
                        var arrayList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(arrayString);
                        await _standardServiceProvider.ForceUpdateStandards(arrayList);
                        break;
                    case CHART_FILE:
                        var chartFilePath = Path.Combine(folderExtractingPath, CHART_FILE);
                        var chartsString = File.ReadAllText(chartFilePath);
                        var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                        await _chartServiceProvider.ForceUpdateCharts(chartsList);
                        break;
                    case DATABASE_FILE:
                        var databaseFilePath = Path.Combine(folderExtractingPath, DATABASE_FILE);
                        var databasesString = File.ReadAllText(databaseFilePath);
                        var databasesList = ConvertUtil.DeserializeObject<IEnumerable<DatabaseConnection>>(databasesString);
                        await _databaseServiceProvider.ForceUpdateDatabases(databasesList);
                        break;
                    case PAGE_FILE:
                        var pageFilePath = Path.Combine(folderExtractingPath, PAGE_FILE);
                        var pagesString = File.ReadAllText(pageFilePath);
                        var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                        await _pageServiceProvider.ForceUpdatePages(pagesList);
                        break;
                    case DYNAMICLIST_FILE:
                        var dynamicListFilePath = Path.Combine(folderExtractingPath, DYNAMICLIST_FILE);
                        var dynamicListString = File.ReadAllText(dynamicListFilePath);
                        var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                        await _dynamicListServiceProvider.ForceUpdateDynamicLists(dynamicListsList);
                        break;
                    case COMPOSITE_CONTROL_FILE:
                        var compositeFilePath = Path.Combine(folderExtractingPath, COMPOSITE_CONTROL_FILE);
                        var compositeString = File.ReadAllText(compositeFilePath);
                        var compositeList = ConvertUtil.DeserializeObject<IEnumerable<CompositeControl>>(compositeString);
                        await _compositeControlServiceProvider.ForceUpdate(compositeList);
                        break;
                    default:
                        break;
                }
            }

            Directory.Delete(folderExtractingPath, true);
            File.Delete(restoreFilePath);
        }

        private async Task<string> SaveFormFileAsync(IFormFile file, string saveFolderPath)
        {
            var tempFileName = file.FileName;
            var fullTempFilePath = Path.Combine(saveFolderPath, tempFileName);
            using (var stream = new FileStream(fullTempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullTempFilePath;
        }

        public async Task<GenerateCodeResponseModel> CreateCode(GenerateCodeRequestModel model)
        {
            var collectApp = _appServiceProvider.GetAppsByIds(model.Apps);
            var collectStandards = _standardServiceProvider.GetStandardComponentsByIds(model.Standards);
            var collectTree = _standardServiceProvider.GetStandardComponentsByIds(model.Tree);
            var collectArray = _standardServiceProvider.GetStandardComponentsByIds(model.Array);
            var collectCharts = _chartServiceProvider.GetChartsByIds(model.Charts);
            var collectDynamicLists = _dynamicListServiceProvider.GetDynamicListsByIds(model.DynamicLists);
            var collectDatabases = _databaseServiceProvider.GetDatabaseConnectionsByIds(model.Databases);
            var collectPages = _pageServiceProvider.GetPagesByIds(model.Pages);
            var collectCompositeControls = _compositeControlServiceProvider.GetByIds(model.CompositeControls);

            await Task.WhenAll(
                collectApp, 
                collectStandards, 
                collectCharts, 
                collectDatabases, 
                collectDynamicLists, 
                collectPages, 
                collectCompositeControls);

            var appCodes = new List<CodeGenerableResult>();
            if (collectApp.Result != null && collectApp.Result.Any())
            {
                foreach (var app in collectApp.Result)
                {
                    try
                    {
                        appCodes.Add(app.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for app {app.Name}");
                    }
                }
            }

            var standardCodes = new List<CodeGenerableResult>();
            if (collectStandards.Result != null && collectStandards.Result.Any())
            {
                foreach (var standard in collectStandards.Result)
                {
                    try
                    {
                        standardCodes.Add(standard.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for standard component {standard.Name}");
                    }
                }
            }

            var treeCodes = new List<CodeGenerableResult>();
            if (collectTree.Result != null && collectTree.Result.Any())
            {
                foreach (var tree in collectTree.Result)
                {
                    try
                    {
                        treeCodes.Add(tree.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for tree {tree.Name}");
                    }
                }
            }

            var arrayCodes = new List<CodeGenerableResult>();
            if (collectArray.Result != null && collectArray.Result.Any())
            {
                foreach (var array in collectArray.Result)
                {
                    try
                    {
                        arrayCodes.Add(array.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for array standard {array.Name}");
                    }
                }
            }

            var databaseCodes = new List<CodeGenerableResult>();
            if (collectDatabases.Result != null && collectDatabases.Result.Any())
            {
                foreach (var database in collectDatabases.Result)
                {
                    try
                    {
                        databaseCodes.Add(database.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for database {database.Name}");
                    }
                }
            }

            var chartCodes = new List<CodeGenerableResult>();
            if (collectCharts.Result != null && collectCharts.Result.Any())
            {
                foreach (var chart in collectCharts.Result)
                {
                    try
                    {
                        chartCodes.Add(chart.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for chart {chart.Name}");
                    }
                }
            }

            var pageCodes = new List<CodeGenerableResult>();
            if (collectPages.Result != null && collectPages.Result.Any())
            {
                foreach (var page in collectPages.Result)
                {
                    try
                    {
                        pageCodes.Add(page.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for page {page.Name}");
                    }
                }
            }

            var dynamicListCodes = new List<CodeGenerableResult>();
            if (collectDynamicLists.Result != null && collectDynamicLists.Result.Any())
            {
                foreach (var dynamicList in collectDynamicLists.Result)
                {
                    try
                    {
                        dynamicListCodes.Add(dynamicList.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for page {dynamicList.Name}");
                    }

                }
            }

            var compositeControlCodes = new List<CodeGenerableResult>();
            if (collectCompositeControls.Result != null && collectCompositeControls.Result.Any())
            {
                foreach (var control in collectCompositeControls.Result)
                {
                    try
                    {
                        compositeControlCodes.Add(control.GenerateCode());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Cannot generate code for composite code {control.Name}");
                    }
                }
            }

            var response = new GenerateCodeResponseModel();

            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine("using System;");
            _ = stringBuilder.AppendLine("using System.Threading.Tasks;");
            _ = stringBuilder.AppendLine("using System.Collections.Generic;");
            _ = stringBuilder.AppendLine("namespace CustomVersion");
            _ = stringBuilder.AppendLine("{");
            _ = stringBuilder.AppendLine($"    public class {model.FileName} : LetPortal.Portal.IPortalVersion");
            _ = stringBuilder.AppendLine($"    {{");
            _ = stringBuilder.AppendLine($"        public string VersionNumber => \"{model.VersionNumber}\";");
            _ = stringBuilder.AppendLine($"        public Task Downgrade(LetPortal.Core.Versions.IVersionContext versionContext)");
            _ = stringBuilder.AppendLine($"        {{");
            foreach (var appCode in appCodes)
            {
                _ = stringBuilder.AppendLine(appCode.DeletingCode);
            }

            foreach (var databaseCode in databaseCodes)
            {
                _ = stringBuilder.AppendLine(databaseCode.DeletingCode);
            }

            foreach (var standardCode in standardCodes)
            {
                _ = stringBuilder.AppendLine(standardCode.DeletingCode);
            }

            foreach (var treeCode in treeCodes)
            {
                _ = stringBuilder.AppendLine(treeCode.DeletingCode);
            }

            foreach (var arrayCode in arrayCodes)
            {
                _ = stringBuilder.AppendLine(arrayCode.DeletingCode);
            }

            foreach (var dynamicListCode in dynamicListCodes)
            {
                _ = stringBuilder.AppendLine(dynamicListCode.DeletingCode);
            }

            foreach (var chartCode in chartCodes)
            {
                _ = stringBuilder.AppendLine(chartCode.DeletingCode);
            }

            foreach (var pageCode in pageCodes)
            {
                _ = stringBuilder.AppendLine(pageCode.DeletingCode);
            }

            foreach (var controlCode in compositeControlCodes)
            {
                _ = stringBuilder.AppendLine(controlCode.DeletingCode);
            }

            _ = stringBuilder.AppendLine($"            return System.Threading.Tasks.Task.CompletedTask;");
            _ = stringBuilder.AppendLine($"        }}");
            _ = stringBuilder.AppendLine($"        public Task Upgrade(LetPortal.Core.Versions.IVersionContext versionContext)");
            _ = stringBuilder.AppendLine($"        {{");
            foreach (var appCode in appCodes)
            {
                _ = stringBuilder.AppendLine(appCode.InsertingCode);
            }

            foreach (var databaseCode in databaseCodes)
            {
                _ = stringBuilder.AppendLine(databaseCode.InsertingCode);
            }

            foreach (var standardCode in standardCodes)
            {
                _ = stringBuilder.AppendLine(standardCode.InsertingCode);
            }

            foreach (var treeCode in treeCodes)
            {
                _ = stringBuilder.AppendLine(treeCode.InsertingCode);
            }

            foreach (var arrayCode in arrayCodes)
            {
                _ = stringBuilder.AppendLine(arrayCode.InsertingCode);
            }

            foreach (var dynamicListCode in dynamicListCodes)
            {
                _ = stringBuilder.AppendLine(dynamicListCode.InsertingCode);
            }

            foreach (var chartCode in chartCodes)
            {
                _ = stringBuilder.AppendLine(chartCode.InsertingCode);
            }

            foreach (var pageCode in pageCodes)
            {
                _ = stringBuilder.AppendLine(pageCode.InsertingCode);
            }

            foreach (var controlCode in compositeControlCodes)
            {
                _ = stringBuilder.AppendLine(controlCode.InsertingCode);
            }
            _ = stringBuilder.AppendLine($"            return System.Threading.Tasks.Task.CompletedTask;");
            _ = stringBuilder.AppendLine($"        }}");
            _ = stringBuilder.AppendLine($"    }}");
            _ = stringBuilder.AppendLine("}");
            response.Content = stringBuilder.ToString();
            response.FileName = model.FileName + ".cs";
            return response;
        }
    }

    public class BackupFlatternFileModel
    {
        public Backup Backup { get; set; }

        public int TotalObjects { get; set; }

        public List<string> ChainingFiles { get; set; }
    }
}
