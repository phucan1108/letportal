using System;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Databases;

namespace LetPortal.Versions.Databases
{
    public class Database_0_0_1 : IPortalVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.CoreDatabaseId);
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseOptions = versionContext.DatabaseOptions as DatabaseOptions;
            var databaseManagement = new DatabaseConnection
            {
                Id = Constants.CoreDatabaseId,
                ConnectionString = databaseOptions.ConnectionString,
                DatabaseConnectionType = Enum.GetName(typeof(ConnectionType), databaseOptions.ConnectionType).ToLower(),
                DataSource = databaseOptions.Datasource,
                Name = "Database Management"
            };

            versionContext.InsertData(databaseManagement);
        }
    }

}
