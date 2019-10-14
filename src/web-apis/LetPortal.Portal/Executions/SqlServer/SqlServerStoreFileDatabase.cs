using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerStoreFileDatabase : IStoreFileDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public async Task<byte[]> GetFileAsync(StoredFile storedFile, DatabaseOptions databaseOptions)
        {
            var fileId = ConvertUtil.DeserializeObject<DatabaseIdentifierOptions>(storedFile.FileIdentifierOptions).FileId;
            byte[] bytes = null;
            using(var sqlDbConnection = new SqlConnection(databaseOptions.ConnectionString))
            {
                sqlDbConnection.Open();
                using(var sqlCommand = new SqlCommand("SELECT * FROM uploadFiles", sqlDbConnection))
                {
                    using(var sqlReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while(sqlReader.NextResult())
                        {
                            bytes = (byte[])sqlReader["File"];
                            break;
                        }
                    }
                }
            }

            return bytes;
        }

        public async Task<StoredFile> StoreFileAsync(IFormFile file, string tempFilePath, DatabaseOptions databaseOptions)
        {
            using(var sqlDbConnection = new SqlConnection(databaseOptions.ConnectionString))
            {
                sqlDbConnection.Open();
                string oid = DataUtil.GenerateUniqueId();
                using(var transaction = sqlDbConnection.BeginTransaction())
                {
                    using(var sqlCommand = new SqlCommand("INSERT INTO uploadFiles (File) Values(@File)", sqlDbConnection, transaction))
                    {
                        var bytes = await File.ReadAllBytesAsync(tempFilePath);
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
