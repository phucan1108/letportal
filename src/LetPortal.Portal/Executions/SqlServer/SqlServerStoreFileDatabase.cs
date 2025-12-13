using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerStoreFileDatabase : IStoreFileDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public async Task<byte[]> GetFileAsync(StoredFile storedFile, DatabaseOptions databaseOptions)
        {
            var fileId = ConvertUtil.DeserializeObject<DatabaseIdentifierOptions>(storedFile.FileIdentifierOptions).FileId;
            byte[] bytes = null;
            using (var sqlDbConnection = new SqlConnection(databaseOptions.ConnectionString))
            {
                sqlDbConnection.Open();
                using (var sqlCommand = new SqlCommand("SELECT * FROM uploadFiles Where id=@id", sqlDbConnection))
                {
                    sqlCommand.Parameters.Add("id", System.Data.SqlDbType.NVarChar, 450).Value = fileId;
                    using (var sqlReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (sqlReader.HasRows)
                        {
                            while (sqlReader.Read())
                            {
                                bytes = (byte[])sqlReader["File"];
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
            return await StoreFileSqlServer(file.FileName, tempFilePath, databaseOptions);
        }

        public async Task<StoredFile> StoreFileAsync(string localFilePath, DatabaseOptions databaseOptions)
        {
            return await StoreFileSqlServer(Path.GetFileName(localFilePath), localFilePath, databaseOptions);
        }

        private async Task<StoredFile> StoreFileSqlServer(string fileName, string localFilePath, DatabaseOptions databaseOptions)
        {
            using (var sqlDbConnection = new SqlConnection(databaseOptions.ConnectionString))
            {
                sqlDbConnection.Open();
                var oid = DataUtil.GenerateUniqueId();
                using (var transaction = sqlDbConnection.BeginTransaction())
                {
                    using (var sqlCommand = new SqlCommand("INSERT INTO uploadFiles ([id], [file]) Values (@id, @File)", sqlDbConnection, transaction))
                    {
                        var bytes = await File.ReadAllBytesAsync(localFilePath);
                        sqlCommand.Parameters.Add("@id", System.Data.SqlDbType.NVarChar).Value = oid;
                        sqlCommand.Parameters.Add("@File", System.Data.SqlDbType.VarBinary, bytes.Length).Value = bytes;
                        sqlCommand.ExecuteNonQuery();
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
