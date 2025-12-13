using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;

namespace LetPortal.Portal.Entities.Shared
{
    public class MapDataOptions : ICodeGenerable
    {
        public bool KeptAsStore { get; set; }

        /// <summary>
        /// Feature allowed: 
        /// Spread: ...data
        /// Exclude: -data.info
        /// Map: name=id; or name=data.info.username
        /// </summary>
        public string OutputMapping { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var result = new CodeGenerableResult();
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.MapDataOptions",space);
            _ = stringBuilder.AppendLine($"{{",space);
            _ = stringBuilder.AppendLine($"    KeptAsStore = {KeptAsStore.ToString().ToLower()},", space);
            _ = stringBuilder.AppendLine($"    OutputMapping = \"{OutputMapping}\"", space);
            _ = stringBuilder.AppendLine($"}}", space);
            result.InsertingCode = stringBuilder.ToString();
            return result;
        }
    }
}
