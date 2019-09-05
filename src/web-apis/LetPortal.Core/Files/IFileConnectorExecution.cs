using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LetPortal.Core.Files
{
    public interface IFileConnectorExecution
    {
        FileStorageType FileStorageType { get; }

        /// <summary>
        /// This method allows transfer IFormFile into a right place e.g disk, fpt server, database
        /// </summary>
        /// <param name="storedMedia">Contains IFormFiles</param>
        /// <returns>Stored File Id or file path</returns>
        Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath);

        /// <summary>
        /// This method helps to retrieve a file from Connector
        /// </summary>
        /// <param name="fileId">File Id or file path</param>
        /// <returns></returns>
        Task<byte[]> GetFileAsync(StoredFile storedFile);
    }
}
