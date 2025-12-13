using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;

namespace LetPortal.Portal.Entities.Databases
{
    [EntityCollection(Name = DatabaseConstants.DatabaseCollection)]
    [Table("databases")]
    public class DatabaseConnection : BackupableEntity, ICodeGenerable
    {
        [StringLength(500)]
        public string ConnectionString { get; set; }

        [StringLength(250)]
        public string DataSource { get; set; }

        [StringLength(50)]
        public string DatabaseConnectionType { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult
            {
                DeletingCode = $"versionContext.DeleteData<LetPortal.Portal.Entities.Databases.DatabaseConnection>(\"{Id}\");"
            };

            var stringBuilder = new StringBuilder();
            varName ??= Name.Replace("-", "", System.StringComparison.OrdinalIgnoreCase) + "Connection";
            _ = stringBuilder.AppendLine($"var {varName} = new LetPortal.Portal.Entities.Databases.DatabaseConnection");
            _ = stringBuilder.AppendLine($"{{");
            _ = stringBuilder.AppendLine($"    Id = \"{Id}\",");
            _ = stringBuilder.AppendLine($"    Name = \"{Name}\",");
            _ = stringBuilder.AppendLine($"    DisplayName = \"{DisplayName}\",");
            _ = stringBuilder.AppendLine($"    ConnectionString = \"{ConnectionString}\",");
            _ = stringBuilder.AppendLine($"    DatabaseConnectionType = \"{DatabaseConnectionType}\",");
            _ = stringBuilder.AppendLine($"    DataSource = \"{DataSource}\",");
            _ = stringBuilder.AppendLine($"}};");
            _ = stringBuilder.AppendLine($"versionContext.InsertData({varName});");
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }

        public ConnectionType GetConnectionType()
        {
            return DatabaseConnectionType.ToEnum<ConnectionType>(true);
        }
    }
}
