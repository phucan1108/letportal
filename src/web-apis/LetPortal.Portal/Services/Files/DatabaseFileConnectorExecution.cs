using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetPortal.Portal.Services.Files
{
    public class DatabaseFileConnectorExecution : IFileConnectorExecution
    {
        private readonly IOptionsMonitor<DatabaseStorageOptions> _databaseStorageOptions;

        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;

        private readonly IEnumerable<IStoreFileDatabase> _storeFileDatabases;

        public FileStorageType FileStorageType => FileStorageType.Database;

        public DatabaseFileConnectorExecution(
            IOptionsMonitor<DatabaseOptions> databaseOptions,
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptions,
            IEnumerable<IStoreFileDatabase> storeFileDatabases)
        {
            _databaseOptions = databaseOptions;
            _databaseStorageOptions = databaseStorageOptions;
            _storeFileDatabases = storeFileDatabases;
        }

        public async Task<byte[]> GetFileAsync(StoredFile storedFile)
        {
            var databaseOptions = _databaseOptions.CurrentValue;
            if (!_databaseStorageOptions.CurrentValue.SameAsPortal)
            {
                databaseOptions = _databaseStorageOptions.CurrentValue.DatabaseOptions;
            }

            var foundStoreFileDatabase = _storeFileDatabases.First(a => a.ConnectionType == databaseOptions.ConnectionType);

            return await foundStoreFileDatabase.GetFileAsync(storedFile, databaseOptions);
        }

        public async Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath)
        {
            var databaseOptions = _databaseOptions.CurrentValue;
            if (!_databaseStorageOptions.CurrentValue.SameAsPortal)
            {
                databaseOptions = _databaseStorageOptions.CurrentValue.DatabaseOptions;
            }

            var foundStoreFileDatabase = _storeFileDatabases.First(a => a.ConnectionType == databaseOptions.ConnectionType);
            return await foundStoreFileDatabase.StoreFileAsync(file, tempFilePath, databaseOptions);
        }

        public async Task<StoredFile> StoreFileAsync(string localFilePath)
        {
            var databaseOptions = _databaseOptions.CurrentValue;
            if (!_databaseStorageOptions.CurrentValue.SameAsPortal)
            {
                databaseOptions = _databaseStorageOptions.CurrentValue.DatabaseOptions;
            }

            var foundStoreFileDatabase = _storeFileDatabases.First(a => a.ConnectionType == databaseOptions.ConnectionType);
            return await foundStoreFileDatabase.StoreFileAsync(localFilePath, databaseOptions);
        }
    }
}
