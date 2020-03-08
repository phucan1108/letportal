using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Executions
{
    public interface IStoreFileDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath, DatabaseOptions databaseOptions);

        Task<StoredFile> StoreFileAsync(string localFilePath, DatabaseOptions databaseOptions);

        Task<byte[]> GetFileAsync(StoredFile storedFile, DatabaseOptions databaseOptions);
    }
}
