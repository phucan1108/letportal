using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Entities.Shared
{
    public class HttpServiceOptions : ICodeGenerable
    {
        public string HttpServiceUrl { get; set; }

        public string HttpMethod { get; set; }

        public string HttpSuccessCode { get; set; }

        public string JsonBody { get; set; }

        public string OutputProjection { get; set; }

        public virtual CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();

            var stringBuilder = new StringBuilder();
            varName = varName != null ? varName : "HttpServiceOptions";
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.HttpServiceOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            _ = stringBuilder.AppendLine($"    HttpServiceUrl = \"{HttpServiceUrl}\",", space);
            _ = stringBuilder.AppendLine($"    HttpMethod = \"{HttpMethod}\",", space);
            _ = stringBuilder.AppendLine($"    HttpSuccessCode = \"{HttpSuccessCode}\",", space);
            _ = stringBuilder.AppendLine($"    JsonBody = {StringUtil.ToLiteral(JsonBody)},", space);
            _ = stringBuilder.AppendLine($"    OutputProjection = \"{OutputProjection}\",", space);
            _ = stringBuilder.AppendLine($"}},", space);

            codeResult.InsertingCode = stringBuilder.ToString();

            return codeResult;
        }
    }
}
