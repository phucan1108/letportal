using System.Text;
using System.Text.RegularExpressions;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Entities.Shared
{
    public class SharedDatabaseOptions: ICodeGenerable
    {
        public string DatabaseConnectionId { get; set; }

        public string EntityName { get; set; }

        public string Query { get; set; }

        public virtual CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();
            varName = varName != null ? varName : "DatabaseOptions";
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            _ = stringBuilder.AppendLine($"    DatabaseConnectionId = \"{DatabaseConnectionId}\",", space);
            _ = stringBuilder.AppendLine($"    Query = {StringUtil.ToLiteral(Query)},", space);
            _ = stringBuilder.AppendLine($"}},", space);
            codeResult.InsertingCode = stringBuilder.ToString();

            return codeResult;
        }
    }
}
