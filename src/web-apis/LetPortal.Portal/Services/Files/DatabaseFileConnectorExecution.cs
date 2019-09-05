using LetPortal.Core.Files;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Options.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Files
{
    public class DatabaseFileConnectorExecution : IFileConnectorExecution
    {
        private readonly IOptionsMonitor<DatabaseStorageOptions> _databaseStorageOptions;

        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;

        public FileStorageType FileStorageType => FileStorageType.Database;

        public DatabaseFileConnectorExecution(
            IOptionsMonitor<DatabaseOptions> databaseOptions,
            IOptionsMonitor<DatabaseStorageOptions> databaseStorageOptions)
        {
            _databaseOptions = databaseOptions;
            _databaseStorageOptions = databaseStorageOptions;
        }

        public async Task<byte[]> GetFileAsync(StoredFile storedFile)
        {
            var fileId = ConvertUtil.DeserializeObject<DatabaseIdentifierOptions>(storedFile.FileIdentifierOptions).FileId;
            if(_databaseStorageOptions.CurrentValue.SameAsPortal)
            {
                if(_databaseOptions.CurrentValue.ConnectionType == ConnectionType.MongoDB)
                {
                    var bucket = GetMongoBucket(new DatabaseStorageOptions { DatabaseOptions = _databaseOptions.CurrentValue });
                    return await bucket.DownloadAsBytesAsync(ObjectId.Parse(fileId));
                }

                return null;
            }
            else
            {
                if(_databaseStorageOptions.CurrentValue.DatabaseOptions.ConnectionType == ConnectionType.MongoDB)
                {
                    var bucket = GetMongoBucket(_databaseStorageOptions.CurrentValue);
                    return await bucket.DownloadAsBytesAsync(ObjectId.Parse(fileId));
                }

                return null;
            }
        }

        public async Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath)
        {
            if(_databaseStorageOptions.CurrentValue.SameAsPortal)
            {
                if(_databaseOptions.CurrentValue.ConnectionType == ConnectionType.MongoDB)
                {                       
                    var bucket = GetMongoBucket(new DatabaseStorageOptions
                    {
                        DatabaseOptions = _databaseOptions.CurrentValue
                    });
                    ObjectId fileId = ObjectId.Empty;
                    using(var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fileId = await bucket.UploadFromStreamAsync(file.FileName, fileStream);
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

                return null;
            }
            else
            {
                var bucket = GetMongoBucket(_databaseStorageOptions.CurrentValue);

                ObjectId fileId = ObjectId.Empty;
                using(var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fileId = await bucket.UploadFromStreamAsync(file.FileName, fileStream);
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
        }

        private GridFSBucket GetMongoBucket(DatabaseStorageOptions databaseStorageOptions)
        {
            var mongoDatabase = new MongoClient(databaseStorageOptions.DatabaseOptions.ConnectionString).GetDatabase(databaseStorageOptions.DatabaseOptions.Datasource);
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
