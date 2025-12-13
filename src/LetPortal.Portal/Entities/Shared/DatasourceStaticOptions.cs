using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Entities.Shared
{
    public class DatasourceStaticOptions : ICodeGenerable
    {
        public string JsonResource { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();
            varName ??= "DatasourceStaticOptions";
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            _ = stringBuilder.AppendLine($"    JsonResource = {StringUtil.ToLiteral(JsonResource)}", space);
            _ = stringBuilder.AppendLine($"}},", space);
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }
}
