using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Entities.Shared
{
    public class ButtonOptions : ICodeGenerable
    {                              
        public ActionCommandOptions ActionCommandOptions { get; set; }

        public RouteOptions RouteOptions { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            Console.WriteLine("Current button options " + ConvertUtil.SerializeObject(this));
            var codeResult = new CodeGenerableResult();
            varName ??= "ButtonOptions";
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.ButtonOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            if(ActionCommandOptions != null)
            {
                _ = stringBuilder.AppendLine(ActionCommandOptions.GenerateCode(space: 4).InsertingCode);
            }
            if(RouteOptions != null)
            {
                Console.WriteLine("Current route options " + ConvertUtil.SerializeObject(RouteOptions));
                _ = stringBuilder.AppendLine(RouteOptions.GenerateCode("RouteOptions", space: 4).InsertingCode);
            }
            _ = stringBuilder.AppendLine($"}},", space);
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    public class ActionOptions
    {
        public bool IsEnable { get; set; }

        public ActionCommandOptions CommandOptions { get; set; }
    }

    public class RouteOptions : ICodeGenerable
    {
        public List<Route> Routes { get; set; }

        public bool IsEnable { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();
            varName ??= "RouteOptions";
            Console.WriteLine("Current route options generator" + ConvertUtil.SerializeObject(this));
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.RouteOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            _ = stringBuilder.AppendLine($"    IsEnable = {IsEnable.ToString().ToLower()},", space);
            if(Routes != null && Routes.Any())
            {
                _ = stringBuilder.AppendLine($"    Routes = new System.Collections.Generic.List<LetPortal.Portal.Entities.Shared.Route>", space);
                _ = stringBuilder.AppendLine($"    {{", space);
                foreach(var route in Routes)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Shared.Route",space);
                    _ = stringBuilder.AppendLine($"        {{", space);
                    _ = stringBuilder.AppendLine($"            RedirectUrl = \"{route.RedirectUrl}\",", space);
                    _ = stringBuilder.AppendLine($"            Condition = \"{route.Condition}\",", space);
                    _ = stringBuilder.AppendLine($"            IsSameDomain = {route.IsSameDomain.ToString().ToLower()},", space);
                    _ = stringBuilder.AppendLine($"         }},", space);
                }
                _ = stringBuilder.AppendLine($"    }},", space);
            }
            _ = stringBuilder.AppendLine($"}},", space);
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    public class Route
    {
        public string RedirectUrl { get; set; }

        public bool IsSameDomain { get; set; }

        public string Condition { get; set; }
    }

    public enum RouteType
    {
        ThroughPage,
        ThroughUrl
    }
}
