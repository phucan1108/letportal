using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Recoveries;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Exceptions.Recoveries;
using LetPortal.Portal.Models.Recoveries;
using LetPortal.Portal.Options.Recoveries;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Recoveries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Recoveries
{
    public class BackupService : IBackupService
    {
        public const string APP_FILE = "apps.json";
        public const string CHART_FILE = "charts.json";
        public const string DATABASE_FILE = "databases.json";
        public const string STANDARD_FILE = "standards.json";
        public const string DYNAMICLIST_FILE = "dynamiclists.json";
        public const string PAGE_FILE = "pages.json";

        private readonly IAppServiceProvider _appServiceProvider;

        private readonly IStandardServiceProvider _standardServiceProvider;

        private readonly IChartServiceProvider _chartServiceProvider;

        private readonly IDynamicListServiceProvider _dynamicListServiceProvider;

        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IPageServiceProvider _pageServiceProvider;

        private readonly IFileSeviceProvider _fileSeviceProvider;

        private readonly IBackupRepository _backupRepository;

        private readonly IOptionsMonitor<BackupOptions> _backupOptions;

        public BackupService(
            IAppServiceProvider appServiceProvider,
            IStandardServiceProvider standardServiceProvider,
            IChartServiceProvider chartServiceProvider,
            IDynamicListServiceProvider dynamicListServiceProvider,
            IDatabaseServiceProvider databaseServiceProvider,
            IPageServiceProvider pageServiceProvider,
            IFileSeviceProvider fileSeviceProvider,
            IBackupRepository backupRepository,
            IOptionsMonitor<BackupOptions> backupOptions
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
            _backupOptions = backupOptions;
        }

        public async Task<BackupResponseModel> CreateBackupFile(BackupRequestModel model)
        {
            Directory.CreateDirectory(_backupOptions.CurrentValue.BackupFolderPath);
            // Ensure a total number of objects less than MaximumObjects
            var appCount = model.Apps != null ? model.Apps.Count() : 0;
            var standardCount = model.Standards != null ? model.Standards.Count() : 0;
            var chartCount = model.Charts != null ? model.Charts.Count() : 0;
            var dynamicCount = model.DynamicLists != null ? model.DynamicLists.Count() : 0;
            var databaseCount = model.Databases != null ? model.Databases.Count() : 0;
            var pageCount = model.Pages != null ? model.Pages.Count() : 0;

            var totalBackupCount = appCount + standardCount + chartCount + dynamicCount + databaseCount + pageCount;

            if(totalBackupCount > _backupOptions.CurrentValue.MaximumObjects)
            {
                throw new BackupException(BackupErrorCodes.ReachMaximumBackupObjects);
            }
            Task<IEnumerable<App>> collectApp = _appServiceProvider.GetAppsByIds(model.Apps);
            Task<IEnumerable<StandardComponent>> collectStandards = _standardServiceProvider.GetStandardComponentsByIds(model.Standards);
            Task<IEnumerable<Chart>> collectCharts = _chartServiceProvider.GetChartsByIds(model.Charts);
            Task<IEnumerable<DynamicList>> collectDynamicLists = _dynamicListServiceProvider.GetDynamicListsByIds(model.DynamicLists);
            Task<IEnumerable<DatabaseConnection>> collectDatabases = _databaseServiceProvider.GetDatabaseConnectionsByIds(model.Databases);
            Task<IEnumerable<Page>> collectPages = _pageServiceProvider.GetPagesByIds(model.Pages);

            await Task.WhenAll(collectApp, collectStandards, collectCharts, collectDatabases, collectDynamicLists, collectPages);

            var backupFileModel = new BackupFlatternFileModel
            {
                TotalObjects = totalBackupCount
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
                    Standards = model.Standards
                }
            };
            backupFileModel.Backup = backup;

            // Write to file
            var fileName = DateTime.UtcNow.Ticks.ToString();
            var jsonFilePath = !string.IsNullOrEmpty(_backupOptions.CurrentValue.BackupFolderPath) ? _backupOptions.CurrentValue.BackupFolderPath : Environment.CurrentDirectory;
            var jsonFileName = fileName + ".json";
            jsonFilePath = Path.Combine(jsonFilePath, fileName);
            Directory.CreateDirectory(jsonFilePath);

            if(collectApp.Result != null)
            {
                var jsonApps = ConvertUtil.SerializeObject(collectApp.Result, true);
                using(StreamWriter sw = new StreamWriter(
                    Path.Combine(jsonFilePath, APP_FILE)))
                {
                    sw.Write(jsonApps);
                }

                backupFileModel.ChainingFiles.Add(APP_FILE);
            }

            if(collectStandards.Result != null)
            {
                var jsonStandards = ConvertUtil.SerializeObject(collectStandards.Result, true);
                using(StreamWriter sw = new StreamWriter(
                    Path.Combine(jsonFilePath, STANDARD_FILE)))
                {
                    sw.Write(jsonStandards);
                }
                backupFileModel.ChainingFiles.Add(STANDARD_FILE);
            }

            if(collectDynamicLists.Result != null)
            {
                var jsonDynamicLists = ConvertUtil.SerializeObject(collectDynamicLists.Result, true);
                using(StreamWriter sw = new StreamWriter(
                    Path.Combine(jsonFilePath, DYNAMICLIST_FILE)))
                {
                    sw.Write(jsonDynamicLists);
                }
                backupFileModel.ChainingFiles.Add(DYNAMICLIST_FILE);
            }

            if(collectDatabases.Result != null)
            {
                var jsonDatabases = ConvertUtil.SerializeObject(collectDatabases.Result, true);
                using(StreamWriter sw = new StreamWriter(
                    Path.Combine(jsonFilePath, DATABASE_FILE)))
                {
                    sw.Write(jsonDatabases);
                }
                backupFileModel.ChainingFiles.Add(DATABASE_FILE);
            }

            if(collectCharts.Result != null)
            {
                var jsonCharts = ConvertUtil.SerializeObject(collectCharts.Result, true);
                using(StreamWriter sw = new StreamWriter(
                    Path.Combine(jsonFilePath, CHART_FILE)))
                {
                    sw.Write(jsonCharts);
                }
                backupFileModel.ChainingFiles.Add(CHART_FILE);
            }

            if(collectPages.Result != null)
            {
                var jsonPages = ConvertUtil.SerializeObject(collectPages.Result, true);
                using(StreamWriter sw = new StreamWriter(
                    Path.Combine(jsonFilePath, PAGE_FILE)))
                {
                    sw.Write(jsonPages);
                }
                backupFileModel.ChainingFiles.Add(PAGE_FILE);
            }

            var jsonFlattern = ConvertUtil.SerializeObject(backupFileModel, true);

            using(StreamWriter sw = new StreamWriter(Path.Combine(jsonFilePath, jsonFileName)))
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
            if(isFileValid)
            {
                Directory.CreateDirectory(_backupOptions.CurrentValue.RestoreFolderPath);
                var tempFilePath = await SaveFormFileAsync(uploadFile, _backupOptions.CurrentValue.RestoreFolderPath);
                var unzipFileName = Path.GetFileNameWithoutExtension(tempFilePath);
                var unzipFolderPath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, Path.GetFileNameWithoutExtension(tempFilePath));
                if(Directory.Exists(unzipFolderPath))
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

            var fileNameWithoutExt = zipFile.FileName.Split(".")[0];
            var restoreFilePath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, zipFile.FileName);
            using(var fileStream = File.Create(restoreFilePath))
            {
                fileStream.Write(zipFile.FileBytes, 0, zipFile.FileBytes.Length);
            }

            // Release file in memory
            zipFile.FileBytes = null;
            var folderExtractingPath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, fileNameWithoutExt);
            if(Directory.Exists(folderExtractingPath))
            {
                Directory.Delete(folderExtractingPath, true);
            }
            ZipFile.ExtractToDirectory(restoreFilePath, folderExtractingPath);

            var jsonBackupFilePath = Path.Combine(folderExtractingPath, fileNameWithoutExt + ".json");

            var jsonBackupString = File.ReadAllText(jsonBackupFilePath);
            var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(jsonBackupString);

            foreach(var chainingFile in backupFlatternModel.ChainingFiles)
            {
                switch(chainingFile)
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
                (previewModel.Standards != null ? previewModel.Standards.Count() : 0);

            previewModel.TotalNewObjects =
                (previewModel.Apps != null ? previewModel.Apps.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Charts != null ? previewModel.Charts.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Databases != null ? previewModel.Databases.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.DynamicLists != null ? previewModel.DynamicLists.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Pages != null ? previewModel.Pages.Count(a => a.IsTotallyNew) : 0) +
                (previewModel.Standards != null ? previewModel.Standards.Count(a => a.IsTotallyNew) : 0);

            previewModel.TotalUnchangedObjects =
                (previewModel.Apps != null ? previewModel.Apps.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Charts != null ? previewModel.Charts.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Databases != null ? previewModel.Databases.Count(a => a.IsUnchanged) : 0) +
                (previewModel.DynamicLists != null ? previewModel.DynamicLists.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Pages != null ? previewModel.Pages.Count(a => a.IsUnchanged) : 0) +
                (previewModel.Standards != null ? previewModel.Standards.Count(a => a.IsUnchanged) : 0);

            previewModel.TotalChangedObjects =
                previewModel.TotalObjects - previewModel.TotalNewObjects - previewModel.TotalUnchangedObjects;

            return previewModel;
        }

        public async Task RestoreBackupPoint(string backupId)
        {
            var backup = await _backupRepository.GetOneAsync(backupId);

            var zipFile = await _fileSeviceProvider.DownloadFileAsync(backup.FileId);

            var fileNameWithoutExt = zipFile.FileName.Split(".")[0];
            var restoreFilePath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, zipFile.FileName);
            using(var fileStream = File.Create(restoreFilePath))
            {
                fileStream.Write(zipFile.FileBytes, 0, zipFile.FileBytes.Length);
            }

            // Release file in memory
            zipFile.FileBytes = null;
            var folderExtractingPath = Path.Combine(_backupOptions.CurrentValue.RestoreFolderPath, fileNameWithoutExt);
            if(Directory.Exists(folderExtractingPath))
            {
                Directory.Delete(folderExtractingPath, true);
            }
            ZipFile.ExtractToDirectory(restoreFilePath, folderExtractingPath);

            var jsonBackupFilePath = Path.Combine(folderExtractingPath, fileNameWithoutExt + ".json");

            var jsonBackupString = File.ReadAllText(jsonBackupFilePath);
            var backupFlatternModel = ConvertUtil.DeserializeObject<BackupFlatternFileModel>(jsonBackupString);

            foreach(var chainingFile in backupFlatternModel.ChainingFiles)
            {
                switch(chainingFile)
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
            using(var stream = new FileStream(fullTempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullTempFilePath;
        }
    }

    public class BackupFlatternFileModel
    {
        public Backup Backup { get; set; }

        public int TotalObjects { get; set; }

        public List<string> ChainingFiles { get; }
    }
}
