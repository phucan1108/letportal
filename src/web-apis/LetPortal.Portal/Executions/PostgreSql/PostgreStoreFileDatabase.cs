using System.IO;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreStoreFileDatabase : IStoreFileDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public async Task<byte[]> GetFileAsync(StoredFile storedFile, DatabaseOptions databaseOptions)
        {
            var fileId = ConvertUtil.DeserializeObject<DatabaseIdentifierOptions>(storedFile.FileIdentifierOptions).FileId;
            byte[] bytes;
            using (var postgreDbConnection = new NpgsqlConnection(databaseOptions.ConnectionString))
            {
                postgreDbConnection.Open();
                var manager = new NpgsqlLargeObjectManager(postgreDbConnection);
                // Reading and writing Large Objects requires the use of a transaction
                using (var transaction = postgreDbConnection.BeginTransaction())
                {
                    // Open the file for reading and writing
                    using (var stream = manager.OpenRead(fileId))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            bytes = memoryStream.GetBuffer();
                            memoryStream.Close();
                        }
                    }
                }
            }

            return bytes;
        }

        public async Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreFilePostgreSQL(file.FileName, tempFilePath, databaseOptions);
        }

        public async Task<StoredFile> StoreFileAsync(string localFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreFilePostgreSQL(Path.GetFileName(localFilePath), localFilePath, databaseOptions);
        }

        private async Task<StoredFile> StoreFilePostgreSQL(string fileName, string localFilePath, DatabaseOptions databaseOptions)
        {
            using (var postgreDbConnection = new NpgsqlConnection(databaseOptions.ConnectionString))
            {
                postgreDbConnection.Open();
                var manager = new NpgsqlLargeObjectManager(postgreDbConnection);
                var oid = manager.Create();
                // Reading and writing Large Objects requires the use of a transaction
                using (var transaction = postgreDbConnection.BeginTransaction())
                {
                    // Open the file for reading and writing
                    using (var stream = manager.OpenReadWrite(oid))
                    {
                        using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            await fileStream.CopyToAsync(stream);
                        }
                    }
                    // Save the changes to the object
                    transaction.Commit();
                }

                return new StoredFile
                {
                    FileIdentifierOptions = ConvertUtil.SerializeObject(new DatabaseIdentifierOptions
                    {
                        FileId = oid
                    }),
                    UseServerHost = true
                };
            }
        }
    }

    class DatabaseIdentifierOptions
    {
        public uint FileId { get; set; }
    }
}
