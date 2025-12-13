using System.Threading.Tasks;
using LetPortal.Core.Versions;
using LetPortal.Portal;

namespace LetPortal.Versions.Files
{
    public class File_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.1.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            if (versionContext.ConnectionType == Core.Persistences.ConnectionType.SQLServer)
            {
                versionContext.ExecuteRaw("DROP TABLE IF EXISTS [dbo].[uploadFiles]");
            }
            else if (versionContext.ConnectionType == Core.Persistences.ConnectionType.PostgreSQL)
            {

            }
            else if (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL)
            {
                versionContext.ExecuteRaw("Drop table if exists `uploadFiles`");
            }

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            if (versionContext.ConnectionType == Core.Persistences.ConnectionType.SQLServer)
            {
                versionContext.ExecuteRaw("Create table [dbo].[uploadFiles] ([id] [nvarchar](450) NOT NULL, [file] [varbinary](max) NULL, CONSTRAINT [PK_uploadFiles] PRIMARY KEY CLUSTERED ( [id] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
            }
            else if (versionContext.ConnectionType == Core.Persistences.ConnectionType.PostgreSQL)
            {

            }
            else if (versionContext.ConnectionType == Core.Persistences.ConnectionType.MySQL)
            {
                versionContext.ExecuteRaw("Create table `uploadFiles`(`id` varchar(255) NOT NULL,  `file` mediumblob NULL, PRIMARY KEY(`id`)) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci");
            }

            return Task.CompletedTask;
        }
    }
}
