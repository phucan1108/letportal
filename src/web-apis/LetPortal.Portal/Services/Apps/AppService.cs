using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Localizations;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Apps;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Services.Apps
{
    public class AppService : IAppService
    {
        public const string APP_FILE = "app.json";
        public const string CHART_FILE = "charts.json";
        public const string DATABASE_FILE = "databases.json";
        public const string STANDARD_FILE = "standards.json";
        public const string DYNAMICLIST_FILE = "dynamiclists.json";
        public const string PAGE_FILE = "pages.json";
        public const string LOCALE_FILE = "locales.json";

        private readonly IStandardServiceProvider _standardServiceProvider;

        private readonly IChartServiceProvider _chartServiceProvider;

        private readonly IDynamicListServiceProvider _dynamicListServiceProvider;

        private readonly IPageServiceProvider _pageServiceProvider;

        private readonly IFileSeviceProvider _fileSeviceProvider;

        private readonly IAppRepository _appRepository;

        private readonly ILocalizationProvider _localizationProvider;

        public AppService(
            IStandardServiceProvider standardServiceProvider,
            IChartServiceProvider chartServiceProvider,
            IDynamicListServiceProvider dynamicListServiceProvider,
            IPageServiceProvider pageServiceProvider,
            IFileSeviceProvider fileSeviceProvider,
            ILocalizationProvider localizationProvider,
            IAppRepository appRepository
            )
        {
            _standardServiceProvider = standardServiceProvider;
            _chartServiceProvider = chartServiceProvider;
            _dynamicListServiceProvider = dynamicListServiceProvider;
            _fileSeviceProvider = fileSeviceProvider;
            _pageServiceProvider = pageServiceProvider;
            _appRepository = appRepository;
            _localizationProvider = localizationProvider;
        }

        public async Task Install(string uploadFileId, InstallWay installWay = InstallWay.Merge)
        {
            var zipFile = await _fileSeviceProvider.DownloadFileAsync(uploadFileId);
            var fileNameWithoutExt = FileUtil.GetFileNameWithoutExt(zipFile.FileName);
            var restoreFolderPath = Path.Combine(Environment.CurrentDirectory, "Temp", "InstallationPackages");
            Directory.CreateDirectory(restoreFolderPath);
            var restoreFilePath = Path.Combine(restoreFolderPath, zipFile.FileName);
            using (var fileStream = File.Create(restoreFilePath))
            {
                fileStream.Write(zipFile.FileBytes, 0, zipFile.FileBytes.Length);
            }
            // Release file in memory
            zipFile.FileBytes = null;
            var folderExtractingPath = Path.Combine(restoreFolderPath, fileNameWithoutExt);
            if (Directory.Exists(folderExtractingPath))
            {
                Directory.Delete(folderExtractingPath, true);
            }
            ZipFile.ExtractToDirectory(restoreFilePath, folderExtractingPath);

            var jsonAppPackageFilePath = Path.Combine(folderExtractingPath, fileNameWithoutExt + ".json");

            var jsonAppPackageString = File.ReadAllText(jsonAppPackageFilePath);
            var appFlatternModel = ConvertUtil.DeserializeObject<AppPackageFlatternModel>(jsonAppPackageString);

            var isExist = await _appRepository.IsExistAsync(a => a.Id == appFlatternModel.App.Id);

            switch (installWay)
            {
                case InstallWay.Merge:
                    break;
                case InstallWay.Wipe:
                    if (isExist)
                    {
                        // Wipe all data
                        await _standardServiceProvider.DeleteAllByAppIdAsync(appFlatternModel.App.Id);
                        await _dynamicListServiceProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                        await _chartServiceProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                        await _pageServiceProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                        await _localizationProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                    }
                    break;
            }

            foreach (var chainingFile in appFlatternModel.ChainingFiles)
            {
                switch (chainingFile)
                {
                    case STANDARD_FILE:
                        var standardFilePath = Path.Combine(folderExtractingPath, STANDARD_FILE);
                        var standardsString = File.ReadAllText(standardFilePath);
                        var standardsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                        await _standardServiceProvider.ForceUpdateStandards(standardsList);
                        break;
                    case DYNAMICLIST_FILE:
                        var dynamicListFilePath = Path.Combine(folderExtractingPath, DYNAMICLIST_FILE);
                        var dynamicListString = File.ReadAllText(dynamicListFilePath);
                        var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                        await _dynamicListServiceProvider.ForceUpdateDynamicLists(dynamicListsList);
                        break;
                    case CHART_FILE:
                        var chartFilePath = Path.Combine(folderExtractingPath, CHART_FILE);
                        var chartsString = File.ReadAllText(chartFilePath);
                        var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                        await _chartServiceProvider.ForceUpdateCharts(chartsList);
                        break;
                    case PAGE_FILE:
                        var pageFilePath = Path.Combine(folderExtractingPath, PAGE_FILE);
                        var pagesString = File.ReadAllText(pageFilePath);
                        var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                        await _pageServiceProvider.ForceUpdatePages(pagesList);
                        break;
                    case LOCALE_FILE:
                        var localeFilePath = Path.Combine(folderExtractingPath, LOCALE_FILE);
                        var localesString = File.ReadAllText(localeFilePath);
                        var localesList = ConvertUtil.DeserializeObject<IEnumerable<Localization>>(localesString);
                        await _localizationProvider.ForceUpdateLocalizations(localesList);
                        break;
                }
            }

            await _appRepository.ForceUpdateAsync(appFlatternModel.App.Id, appFlatternModel.App);

            Directory.Delete(folderExtractingPath, true);
            File.Delete(restoreFilePath);
        }

        public async Task<PackageResponseModel> Package(PackageRequestModel package)
        {
            var app = await _appRepository.GetOneAsync(package.AppId);

            var collectLocales = _localizationProvider.GetByAppId(app.Id);
            var collectStandards = _standardServiceProvider.GetByAppId(app.Id);
            var collectCharts = _chartServiceProvider.GetByAppId(app.Id);
            var collectDynamicLists = _dynamicListServiceProvider.GetByAppId(app.Id);
            var collectPages = _pageServiceProvider.GetByAppId(app.Id);
            await Task.WhenAll(collectLocales, collectStandards, collectCharts, collectDynamicLists, collectPages);

            var appPackpageFlattern = new AppPackageFlatternModel
            {
                App = app,
                Description = package.Description,
                PackagedDate = DateTime.UtcNow,
                Creator = package.Creator,
                TotalCharts = collectCharts.Result?.Count(),
                TotalDynamicLists = collectDynamicLists.Result?.Count(),
                TotalStandards = collectStandards.Result?.Count(),
                TotalLocales = collectLocales.Result?.Count(),
                TotalPages = collectPages.Result?.Count(),
                ChainingFiles = new List<string>()
            };

            var fileName = DateTime.UtcNow.Ticks.ToString();
            var folderName = app.Name + "_" + app.CurrentVersionNumber + "_" + fileName;
            var jsonFileName = folderName + ".json";
            var jsonFilePath = Path.Combine(Environment.CurrentDirectory, "Temp", "Packages", folderName);
            if (Directory.Exists(jsonFilePath))
            {
                Directory.Delete(jsonFilePath, true);
            }
            Directory.CreateDirectory(jsonFilePath);

            if (collectStandards.Result != null)
            {
                var jsonStandards = ConvertUtil.SerializeObject(collectStandards.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, STANDARD_FILE)))
                {
                    sw.Write(jsonStandards);
                }

                appPackpageFlattern.ChainingFiles.Add(STANDARD_FILE);
            }

            if (collectCharts.Result != null)
            {
                var jsonCharts = ConvertUtil.SerializeObject(collectCharts.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, CHART_FILE)))
                {
                    sw.Write(jsonCharts);
                }

                appPackpageFlattern.ChainingFiles.Add(CHART_FILE);
            }

            if (collectDynamicLists.Result != null)
            {
                var jsonDynamicLists = ConvertUtil.SerializeObject(collectDynamicLists.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, DYNAMICLIST_FILE)))
                {
                    sw.Write(jsonDynamicLists);
                }

                appPackpageFlattern.ChainingFiles.Add(DYNAMICLIST_FILE);
            }

            if (collectPages.Result != null)
            {
                var jsonPages = ConvertUtil.SerializeObject(collectPages.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, PAGE_FILE)))
                {
                    sw.Write(jsonPages);
                }

                appPackpageFlattern.ChainingFiles.Add(PAGE_FILE);
            }

            if (collectLocales.Result != null)
            {
                var jsonLocales = ConvertUtil.SerializeObject(collectLocales.Result, true);
                using (var sw = new StreamWriter(
                    Path.Combine(jsonFilePath, LOCALE_FILE)))
                {
                    sw.Write(jsonLocales);
                }

                appPackpageFlattern.ChainingFiles.Add(LOCALE_FILE);
            }

            var jsonFlattern = ConvertUtil.SerializeObject(appPackpageFlattern, true);

            using (var sw = new StreamWriter(Path.Combine(jsonFilePath, jsonFileName)))
            {
                sw.Write(jsonFlattern);
            }

            ZipFile.CreateFromDirectory(jsonFilePath, Path.Combine(Environment.CurrentDirectory, "Temp", "Packages", folderName + ".zip"));

            // Store zip file into file server, allow to create zip file when downloading
            var uploadResponse = await _fileSeviceProvider
                .UploadFileAsync(
                    Path.Combine(Environment.CurrentDirectory, "Temp", "Packages", folderName + ".zip"),
                    package.Creator,
                    true);

            return new PackageResponseModel { DownloadableUrl = uploadResponse.DownloadableUrl };
        }

        public async Task<PreviewPackageModel> Preview(string appId)
        {
            var app = await _appRepository.GetOneAsync(appId);
            var preview = new PreviewPackageModel
            {
                App = app
            };
            if (app != null)
            {
                var standards = await _standardServiceProvider.GetByAppId(appId);
                var charts = await _chartServiceProvider.GetByAppId(appId);
                var dynamicLists = await _dynamicListServiceProvider.GetByAppId(appId);
                var pages = await _pageServiceProvider.GetByAppId(appId);
                var locales = await _localizationProvider.GetByAppId(appId);

                preview.Standards = standards != null ? standards.Select(a => a.DisplayName) : Enumerable.Empty<string>();
                preview.Charts = charts != null ? charts.Select(a => a.DisplayName) : Enumerable.Empty<string>();
                preview.DynamicLists = dynamicLists != null ? dynamicLists.Select(a => a.DisplayName) : Enumerable.Empty<string>();
                preview.Pages = pages != null ? pages.Select(a => a.DisplayName) : Enumerable.Empty<string>();
                preview.Locales = locales != null ? locales.Select(a => a.LocaleId) : Enumerable.Empty<string>();
            }

            return preview;
        }

        public async Task<UnpackResponseModel> Unpack(IFormFile uploadFile, string uploader)
        {
            var unpackResponse = new UnpackResponseModel { };

            var isFileValid = await _fileSeviceProvider.ValidateFile(uploadFile);
            if (isFileValid)
            {
                var storedFilePath = Path.Combine(Environment.CurrentDirectory, "Temp", "Unpackages");
                Directory.CreateDirectory(storedFilePath);
                var tempFilePath = await SaveFormFileAsync(uploadFile, storedFilePath);
                var unzipFileName = Path.GetFileNameWithoutExtension(tempFilePath);
                var unzipFolderPath = Path.Combine(storedFilePath, Path.GetFileNameWithoutExtension(tempFilePath));
                if (Directory.Exists(unzipFolderPath))
                {
                    // Delete old directory
                    Directory.Delete(unzipFolderPath, true);
                }

                Directory.CreateDirectory(unzipFolderPath);
                ZipFile.ExtractToDirectory(tempFilePath, unzipFolderPath);
                var jsonFilePath = Path.Combine(unzipFolderPath, unzipFileName + ".json");
                var jsonFound = File.ReadAllText(jsonFilePath);

                var appFlatternModel = ConvertUtil.DeserializeObject<AppPackageFlatternModel>(jsonFound);

                var app = await _appRepository.GetOneAsync(appFlatternModel.App.Id);
                unpackResponse.IsExistedId = app != null;
                unpackResponse.IsExistedName = await _appRepository.IsExistAsync(a => a.Name == appFlatternModel.App.Name);
                // Save zip file into file service
                var storedFile = await _fileSeviceProvider.UploadFileAsync(tempFilePath, uploader, true);

                unpackResponse.App = appFlatternModel.App;
                unpackResponse.UploadFileId = storedFile.FileId;
                unpackResponse.Description = appFlatternModel.Description;
                unpackResponse.PackagedDate = appFlatternModel.PackagedDate;
                unpackResponse.Creator = appFlatternModel.Creator;

                unpackResponse.TotalStandards = appFlatternModel.TotalStandards.GetValueOrDefault();
                unpackResponse.TotalDynamicLists = appFlatternModel.TotalDynamicLists.GetValueOrDefault();
                unpackResponse.TotalCharts = appFlatternModel.TotalCharts.GetValueOrDefault();
                unpackResponse.TotalPages = appFlatternModel.TotalPages.GetValueOrDefault();
                unpackResponse.TotalLocales = appFlatternModel.TotalLocales.GetValueOrDefault();

                foreach (var chainingFile in appFlatternModel.ChainingFiles)
                {
                    switch (chainingFile)
                    {
                        case STANDARD_FILE:
                            var standardFilePath = Path.Combine(unzipFolderPath, STANDARD_FILE);
                            var standardsString = File.ReadAllText(standardFilePath);
                            var standardsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                            var standardStates = new List<ComponentInstallState>();
                            foreach (var standard in standardsList)
                            {
                                standardStates.Add(new ComponentInstallState
                                {
                                    Name = standard.DisplayName,
                                    IsExisted = await _standardServiceProvider.CheckStandardExist(a => a.Name == standard.Name || a.Id == standard.Id)
                                });
                            }
                            unpackResponse.Standards = standardStates;
                            break;
                        case DYNAMICLIST_FILE:
                            var dynamicListFilePath = Path.Combine(unzipFolderPath, DYNAMICLIST_FILE);
                            var dynamicListString = File.ReadAllText(dynamicListFilePath);
                            var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                            var dynamicListStates = new List<ComponentInstallState>();
                            foreach (var dynamicList in dynamicListsList)
                            {
                                dynamicListStates.Add(new ComponentInstallState
                                {
                                    Name = dynamicList.DisplayName,
                                    IsExisted = await _dynamicListServiceProvider.CheckDynamicListExist(a => a.Name == dynamicList.Name || a.Id == dynamicList.Id)
                                });
                            }
                            unpackResponse.DynamicLists = dynamicListStates;
                            break;
                        case CHART_FILE:
                            var chartFilePath = Path.Combine(unzipFolderPath, CHART_FILE);
                            var chartsString = File.ReadAllText(chartFilePath);
                            var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                            var chartsListStates = new List<ComponentInstallState>();
                            foreach (var chart in chartsList)
                            {
                                chartsListStates.Add(new ComponentInstallState
                                {
                                    Name = chart.DisplayName,
                                    IsExisted = await _chartServiceProvider.CheckChartExist(a => a.Name == chart.Name || a.Id == chart.Id)
                                });
                            }
                            unpackResponse.Charts = chartsListStates;
                            break;
                        case PAGE_FILE:
                            var pageFilePath = Path.Combine(unzipFolderPath, PAGE_FILE);
                            var pagesString = File.ReadAllText(pageFilePath);
                            var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                            var pagesListStates = new List<ComponentInstallState>();
                            foreach (var page in pagesList)
                            {
                                pagesListStates.Add(new ComponentInstallState
                                {
                                    Name = page.DisplayName,
                                    IsExisted = await _pageServiceProvider.CheckPageExist(a => a.Name == page.Name || a.Id == page.Id)
                                });
                            }
                            unpackResponse.Pages = pagesListStates;
                            break;
                        case LOCALE_FILE:
                            var localeFilePath = Path.Combine(unzipFolderPath, LOCALE_FILE);
                            var localesString = File.ReadAllText(localeFilePath);
                            var localesList = ConvertUtil.DeserializeObject<IEnumerable<Localization>>(localesString);
                            var localesListStates = new List<ComponentInstallState>();
                            foreach (var locale in localesList)
                            {
                                localesListStates.Add(new ComponentInstallState
                                {
                                    Name = locale.LocaleId,
                                    IsExisted = await _localizationProvider.CheckLocaleExist(a => a.LocaleId == locale.LocaleId && a.AppId == locale.AppId)
                                });
                            }
                            unpackResponse.Locales = localesListStates;
                            break;
                    }
                }
            }

            return unpackResponse;
        }

        #region Support for CLI
        public async Task<string> Package(string appName, string saveFolderPath)
        {
            var apps = await _appRepository.GetAllAsync(a => a.Name == appName);
            var app = apps.FirstOrDefault();
            if (app != null)
            {
                var collectLocales = _localizationProvider.GetByAppId(app.Id);
                var collectStandards = _standardServiceProvider.GetByAppId(app.Id);
                var collectCharts = _chartServiceProvider.GetByAppId(app.Id);
                var collectDynamicLists = _dynamicListServiceProvider.GetByAppId(app.Id);
                var collectPages = _pageServiceProvider.GetByAppId(app.Id);
                await Task.WhenAll(collectLocales, collectStandards, collectCharts, collectDynamicLists, collectPages);

                var appPackpageFlattern = new AppPackageFlatternModel
                {
                    App = app,
                    Description = "Generated by CLI",
                    PackagedDate = DateTime.UtcNow,
                    Creator = "CLI",
                    TotalCharts = collectCharts.Result?.Count(),
                    TotalDynamicLists = collectDynamicLists.Result?.Count(),
                    TotalStandards = collectStandards.Result?.Count(),
                    TotalLocales = collectLocales.Result?.Count(),
                    TotalPages = collectPages.Result?.Count(),
                    ChainingFiles = new List<string>()
                };

                var fileName = DateTime.UtcNow.Ticks.ToString();
                var folderName = app.Name + "_" + app.CurrentVersionNumber + "_" + fileName;
                var jsonFileName = folderName + ".json";
                var jsonFilePath = Path.Combine(Environment.CurrentDirectory, "Temp", "Packages", folderName);
                if (Directory.Exists(jsonFilePath))
                {
                    Directory.Delete(jsonFilePath, true);
                }
                Directory.CreateDirectory(jsonFilePath);

                if (collectStandards.Result != null)
                {
                    var jsonStandards = ConvertUtil.SerializeObject(collectStandards.Result, true);
                    using (var sw = new StreamWriter(
                        Path.Combine(jsonFilePath, STANDARD_FILE)))
                    {
                        sw.Write(jsonStandards);
                    }

                    appPackpageFlattern.ChainingFiles.Add(STANDARD_FILE);
                }

                if (collectCharts.Result != null)
                {
                    var jsonCharts = ConvertUtil.SerializeObject(collectCharts.Result, true);
                    using (var sw = new StreamWriter(
                        Path.Combine(jsonFilePath, CHART_FILE)))
                    {
                        sw.Write(jsonCharts);
                    }

                    appPackpageFlattern.ChainingFiles.Add(CHART_FILE);
                }

                if (collectDynamicLists.Result != null)
                {
                    var jsonDynamicLists = ConvertUtil.SerializeObject(collectDynamicLists.Result, true);
                    using (var sw = new StreamWriter(
                        Path.Combine(jsonFilePath, DYNAMICLIST_FILE)))
                    {
                        sw.Write(jsonDynamicLists);
                    }

                    appPackpageFlattern.ChainingFiles.Add(DYNAMICLIST_FILE);
                }

                if (collectPages.Result != null)
                {
                    var jsonPages = ConvertUtil.SerializeObject(collectPages.Result, true);
                    using (var sw = new StreamWriter(
                        Path.Combine(jsonFilePath, PAGE_FILE)))
                    {
                        sw.Write(jsonPages);
                    }

                    appPackpageFlattern.ChainingFiles.Add(PAGE_FILE);
                }

                if (collectLocales.Result != null)
                {
                    var jsonLocales = ConvertUtil.SerializeObject(collectLocales.Result, true);
                    using (var sw = new StreamWriter(
                        Path.Combine(jsonFilePath, LOCALE_FILE)))
                    {
                        sw.Write(jsonLocales);
                    }

                    appPackpageFlattern.ChainingFiles.Add(LOCALE_FILE);
                }

                var jsonFlattern = ConvertUtil.SerializeObject(appPackpageFlattern, true);

                using (var sw = new StreamWriter(Path.Combine(jsonFilePath, jsonFileName)))
                {
                    sw.Write(jsonFlattern);
                }

                var savePath = Path.Combine(saveFolderPath, folderName + ".zip");
                ZipFile.CreateFromDirectory(jsonFilePath, savePath);

                return Path.GetFullPath(savePath);
            }

            throw new Exception("There are no matching app, please try again.");
        }

        public async Task Unpack(string zipFilePath, InstallWay installWay = InstallWay.Merge)
        {
            if (File.Exists(Path.GetFullPath(zipFilePath)))
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(zipFilePath);
                var restoreFolderPath = Path.Combine(Environment.CurrentDirectory, "Temp", "InstallationPackages");
                var restoreFilePath = Path.Combine(restoreFolderPath, fileNameWithoutExt);
                Directory.CreateDirectory(restoreFilePath);
                ZipFile.ExtractToDirectory(Path.GetFullPath(zipFilePath), restoreFilePath);

                var jsonAppPackageFilePath = Path.Combine(restoreFilePath, fileNameWithoutExt + ".json");

                var jsonAppPackageString = File.ReadAllText(jsonAppPackageFilePath);
                var appFlatternModel = ConvertUtil.DeserializeObject<AppPackageFlatternModel>(jsonAppPackageString);

                var isExist = await _appRepository.IsExistAsync(a => a.Id == appFlatternModel.App.Id);

                switch (installWay)
                {
                    case InstallWay.Merge:
                        break;
                    case InstallWay.Wipe:
                        if (isExist)
                        {
                            // Wipe all data
                            await _standardServiceProvider.DeleteAllByAppIdAsync(appFlatternModel.App.Id);
                            await _dynamicListServiceProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                            await _chartServiceProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                            await _pageServiceProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                            await _localizationProvider.DeleteByAppIdAsync(appFlatternModel.App.Id);
                        }
                        break;
                }

                foreach (var chainingFile in appFlatternModel.ChainingFiles)
                {
                    switch (chainingFile)
                    {
                        case STANDARD_FILE:
                            var standardFilePath = Path.Combine(restoreFilePath, STANDARD_FILE);
                            var standardsString = File.ReadAllText(standardFilePath);
                            var standardsList = ConvertUtil.DeserializeObject<IEnumerable<StandardComponent>>(standardsString);
                            await _standardServiceProvider.ForceUpdateStandards(standardsList);
                            break;
                        case DYNAMICLIST_FILE:
                            var dynamicListFilePath = Path.Combine(restoreFilePath, DYNAMICLIST_FILE);
                            var dynamicListString = File.ReadAllText(dynamicListFilePath);
                            var dynamicListsList = ConvertUtil.DeserializeObject<IEnumerable<DynamicList>>(dynamicListString);
                            await _dynamicListServiceProvider.ForceUpdateDynamicLists(dynamicListsList);
                            break;
                        case CHART_FILE:
                            var chartFilePath = Path.Combine(restoreFilePath, CHART_FILE);
                            var chartsString = File.ReadAllText(chartFilePath);
                            var chartsList = ConvertUtil.DeserializeObject<IEnumerable<Chart>>(chartsString);
                            await _chartServiceProvider.ForceUpdateCharts(chartsList);
                            break;
                        case PAGE_FILE:
                            var pageFilePath = Path.Combine(restoreFilePath, PAGE_FILE);
                            var pagesString = File.ReadAllText(pageFilePath);
                            var pagesList = ConvertUtil.DeserializeObject<IEnumerable<Page>>(pagesString);
                            await _pageServiceProvider.ForceUpdatePages(pagesList);
                            break;
                        case LOCALE_FILE:
                            var localeFilePath = Path.Combine(restoreFilePath, LOCALE_FILE);
                            var localesString = File.ReadAllText(localeFilePath);
                            var localesList = ConvertUtil.DeserializeObject<IEnumerable<Localization>>(localesString);
                            await _localizationProvider.ForceUpdateLocalizations(localesList);
                            break;
                    }
                }

                await _appRepository.ForceUpdateAsync(appFlatternModel.App.Id, appFlatternModel.App);

                Directory.Delete(restoreFilePath, true);
            }
        }

        #endregion

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
    }

    public class AppPackageFlatternModel
    {
        public string Description { get; set; }

        public string Creator { get; set; }

        public DateTime PackagedDate { get; set; }

        public App App { get; set; }

        public int? TotalStandards { get; set; }

        public int? TotalCharts { get; set; }

        public int? TotalDynamicLists { get; set; }

        public int? TotalPages { get; set; }

        public int? TotalLocales { get; set; }

        public List<string> ChainingFiles { get; set; }
    }
}
