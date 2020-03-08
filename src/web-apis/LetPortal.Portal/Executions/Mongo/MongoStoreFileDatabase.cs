using System.IO;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoStoreFileDatabase : IStoreFileDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        public async Task<byte[]> GetFileAsync(StoredFile storedFile, DatabaseOptions databaseOptions)
        {
            var fileId = ConvertUtil.DeserializeObject<DatabaseIdentifierOptions>(storedFile.FileIdentifierOptions).FileId;
            var bucket = GetMongoBucket(databaseOptions);
            return await bucket.DownloadAsBytesAsync(ObjectId.Parse(fileId));
        }

        public async Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreInMongo(file.FileName, tempFilePath, databaseOptions);
        }

        public async Task<StoredFile> StoreFileAsync(string localFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreInMongo(Path.GetFileName(localFilePath), localFilePath, databaseOptions);
        }

        private async Task<StoredFile> StoreInMongo(string fileName, string localFilePath, DatabaseOptions databaseOptions)
        {
            var bucket = GetMongoBucket(databaseOptions);
            var fileId = ObjectId.Empty;
            using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                fileId = await bucket.UploadFromStreamAsync(fileName, fileStream);
            }
            return new StoredFile
            {
                FileIdentifierOptions = ConvertUtil.SerializeObject(new DatabaseIdentifierOptions
                {
                    FileId = fileId.ToString()
                }),
                UseServerHost = true
            };
        }

        private GridFSBucket GetMongoBucket(DatabaseOptions databaseOptions)
        {
            var mongoDatabase = new MongoClient(databaseOptions.ConnectionString).GetDatabase(databaseOptions.Datasource);
            var bucket = new GridFSBucket(mongoDatabase, new GridFSBucketOptions
            {
                BucketName = "files"
            });

            return bucket;
        }
    }
    class DatabaseIdentifierOptions
    {
        public string FileId { get; set; }
    }
}
