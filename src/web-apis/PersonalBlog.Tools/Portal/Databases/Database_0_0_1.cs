using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;
using LetPortal.Portal.Entities.Databases;
using PersonalBlog.Tools;

namespace TaliBeauty.Tools.Portal.Databases
{
    public class Database_0_0_1 : IPortalVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<DatabaseConnection>(Constants.DATABASE_CMS_ID);
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var cmsDatabase = new DatabaseConnection
            {
                Id = Constants.DATABASE_CMS_ID,
                DatabaseConnectionType = "mongodb",
                ConnectionString = "mongodb://localhost:27017",
                DataSource = "cms",
                DisplayName = "Personal Blog CMS",
                Name = "cms"
            };

            versionContext.InsertData(cmsDatabase);
            return Task.CompletedTask;
        }
    }
}
