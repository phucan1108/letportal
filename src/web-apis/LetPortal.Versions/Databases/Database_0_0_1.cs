using LetPortal.Core.Versions;
using LetPortal.Portal.Entities.Databases;

namespace LetPortal.Versions.Databases
{
    internal class Database_0_0_1 : IVersion
    {
        public string VersionNumber => "0.0.1";

        public void Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.CoreDatabaseId);
        }

        public void Upgrade(IVersionContext versionContext)
        {
            var databaseManagement = new DatabaseConnection
            {
                Id = Constants.CoreDatabaseId,
                ConnectionString = "mongodb://localhost:27017",
                DatabaseConnectionType = "mongodb",
                DataSource = "letportal",
                Name = "Database Management"
            };

            versionContext.InsertData(databaseManagement);
        }
    }

}
