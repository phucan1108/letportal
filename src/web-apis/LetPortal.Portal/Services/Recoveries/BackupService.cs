 using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Recoveries;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.Recoveries;
using LetPortal.Portal.Options.Recoveries;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Recoveries;
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
            Task<IEnumerable<App>> collectApp = _appServiceProvider.GetAppsByIds(model.Apps);
            Task<IEnumerable<StandardComponent>> collectStandards = _standardServiceProvider.GetStandardComponentsByIds(model.Standards);
            Task<IEnumerable<Chart>> collectCharts = _chartServiceProvider.GetChartsByIds(model.Charts);
            Task<IEnumerable<DynamicList>> collectDynamicLists = _dynamicListServiceProvider.GetDynamicListsByIds(model.DynamicLists);
            Task<IEnumerable<DatabaseConnection>> collectDatabases = _databaseServiceProvider.GetDatabaseConnectionsByIds(model.Databases);
            Task<IEnumerable<Page>> collectPages = _pageServiceProvider.GetPagesByIds(model.Pages);

            await Task.WhenAll(collectApp, collectStandards, collectCharts, collectDatabases, collectDynamicLists, collectPages);

            var backupFileModel = new BackupFlatternFileModel
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = model.Name,
                Description = model.Description,
                CreatedDate = DateTime.UtcNow,
                Apps = collectApp.Result?.ToList(),
                Charts = collectCharts.Result?.ToList(),
                Creator = model.Creator,
                Databases = collectDatabases.Result?.ToList(),
                DynamicLists = collectDynamicLists.Result?.ToList(),
                Pages = collectPages.Result?.ToList(),
                StandardComponents = collectStandards.Result?.ToList()
            };

            var jsonFlattern = ConvertUtil.SerializeObject(backupFileModel, true);
            var compressString = StringUtil.CompressionString(jsonFlattern);

            // Write to file
            var fileName = DateTime.UtcNow.Ticks.ToString();
            var folderPath = !string.IsNullOrEmpty(_backupOptions.CurrentValue.FolderPath) ? _backupOptions.CurrentValue.FolderPath : Environment.CurrentDirectory;
            var fileNameTxt = fileName + ".txt";
            var fileNameZip = fileName + ".zip";
            folderPath = Path.Combine(folderPath, fileName);
            Directory.CreateDirectory(folderPath);
            using(StreamWriter sw = new StreamWriter(Path.Combine(folderPath, fileNameTxt)))
            {
                sw.Write(compressString);
            }

            // Compress to zip with encrypt password
            ZipFile.CreateFromDirectory(folderPath, fileNameZip);

            // Store it into file server
            var uploadResponse = await _fileSeviceProvider.UploadFileAsync(Path.Combine(folderPath, fileNameZip), model.Creator);

            await _backupRepository.AddAsync(new Backup
            {
                Id = backupFileModel.Id,
                Name= backupFileModel.Name,
                Description = backupFileModel.Description,
                CreatedDate = backupFileModel.CreatedDate,
                Creator = backupFileModel.Creator,
                BackupElements = new BackupElements
                {
                    Apps = model.Apps,
                    Charts = model.Charts,
                    Databases = model.Databases,
                    DynamicLists = model.DynamicLists,
                    Pages = model.Pages,
                    Standards = model.Standards
                },
                FileId = uploadResponse.FileId,
                DownloadableUrl = uploadResponse.DownloadableUrl
            });

            return new BackupResponseModel { DownloadableUrl = uploadResponse.DownloadableUrl };
        }
    }

    class BackupFlatternFileModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<App> Apps { get; set; }

        public List<StandardComponent> StandardComponents { get; set; }

        public List<DynamicList> DynamicLists { get; set; }

        public List<Chart> Charts { get; set; }

        public List<Page> Pages { get; set; }

        public List<DatabaseConnection> Databases { get; set; }
    }
}
