using System;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;

namespace LetPortal.Portal.Entities.Shared
{
    public class DatasourceOptions : ICodeGenerable
    {
        public DatasourceControlType Type { get; set; }

        public DatasourceStaticOptions DatasourceStaticOptions { get; set; }

        public SharedDatabaseOptions DatabaseOptions { get; set; }

        public HttpServiceOptions HttpServiceOptions { get; set; }

        public string TriggeredEvents { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();
            varName ??= "DatasourceOptions";

            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.DatasourceOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            switch (Type)
            {
                case DatasourceControlType.Database:
                    var databaseType = "LetPortal.Portal.Entities.Shared.DatasourceControlType.Database";
                    _ = stringBuilder.AppendLine($"    Type = {databaseType},", space);
                    _ = stringBuilder.AppendLine(DatabaseOptions.GenerateCode("DatabaseOptions", space + 1).InsertingCode);
                    break;
                case DatasourceControlType.StaticResource:
                    var staticDatabaseType = "LetPortal.Portal.Entities.Shared.DatasourceControlType.StaticResource";
                    _ = stringBuilder.AppendLine($"    Type = {staticDatabaseType},", space);
                    _ = stringBuilder.AppendLine(DatasourceStaticOptions.GenerateCode("DatasourceStaticOptions", space + 1).InsertingCode);
                    break;
                case DatasourceControlType.WebService:
                    var httpType = "LetPortal.Portal.Entities.Shared.DatasourceControlType.WebService";
                    _ = stringBuilder.AppendLine($"    Type = {httpType},", space);
                    _ = stringBuilder.AppendLine(HttpServiceOptions.GenerateCode("HttpServiceOptions", space + 1).InsertingCode);
                    break;
            }
            _ = stringBuilder.AppendLine($"}},", space);

            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    public class DynamicListDatasourceOptions : DatasourceOptions
    {
        public string OutputMapProjection { get; set; }
    }

    public enum DatasourceControlType
    {
        StaticResource,
        Database,
        WebService
    }
}
