using System.IO;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlStoreFileDatabase : IStoreFileDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public async Task<byte[]> GetFileAsync(StoredFile storedFile, DatabaseOptions databaseOptions)
        {
            var fileId = ConvertUtil.DeserializeObject<DatabaseIdentifierOptions>(storedFile.FileIdentifierOptions).FileId;
            byte[] bytes = null;
            using (var mysqlDbConnection = new MySqlConnection(databaseOptions.ConnectionString))
            {
                mysqlDbConnection.Open();
                using (var mysqlCommand = new MySqlCommand("SELECT * FROM uploadFiles Where id=@id", mysqlDbConnection))
                {
                    mysqlCommand.Parameters.Add("id", MySqlDbType.VarChar, 450).Value = fileId;
                    using (var mysqlReader = await mysqlCommand.ExecuteReaderAsync())
                    {
                        if (mysqlReader.HasRows)
                        {
                            while (mysqlReader.Read())
                            {
                                bytes = (byte[])mysqlReader["File"];
                                break;
                            }
                        }
                    }
                }
            }

            return bytes;
        }

        public async Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreFileMySQL(tempFilePath, databaseOptions);
        }

        public async Task<StoredFile> StoreFileAsync(string localFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreFileMySQL(localFilePath, databaseOptions);
        }

        private async Task<StoredFile> StoreFileMySQL(string localFilePath, DatabaseOptions databaseOptions)
        {
            using (var mysqlDbConnection = new MySqlConnection(databaseOptions.ConnectionString))
            {
                mysqlDbConnection.Open();
                var oid = DataUtil.GenerateUniqueId();
                using (var transaction = mysqlDbConnection.BeginTransaction())
                {
                    using (var mysqlCommand = new MySqlCommand("INSERT INTO uploadFiles (id, file) Values (@id, @File)", mysqlDbConnection, transaction))
                    {
                        var bytes = await File.ReadAllBytesAsync(localFilePath);
                        mysqlCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = oid;
                        mysqlCommand.Parameters.Add("@File", MySqlDbType.MediumBlob, bytes.Length).Value = bytes;
                        mysqlCommand.ExecuteNonQuery();
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

        class DatabaseIdentifierOptions
        {
            public string FileId { get; set; }
        }
    }
}
