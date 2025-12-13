using System;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Components.Controls
{
    public class PageControlEvent : ICodeGenerable
    {
        public string EventName { get; set; }

        public EventActionType EventActionType { get; set; }

        public EventHttpServiceOptions EventHttpServiceOptions { get; set; }

        public EventDatabaseOptions EventDatabaseOptions { get; set; }

        public TriggerEventOptions TriggerEventOptions { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();

            var stringBuilder = new StringBuilder();

            _ = stringBuilder.AppendLine($"new LetPortal.Portal.Entities.Components.Controls.PageControlEvent", space);
            _ = stringBuilder.AppendLine("{", space);
            _ = stringBuilder.AppendLine($"    EventName = \"{EventName}\",", space);
            var eventActionType = "LetPortal.Portal.Entities.Components.Controls.EventActionType." + Enum.GetName(typeof(EventActionType), EventActionType);
            _ = stringBuilder.AppendLine($"    EventActionType = {eventActionType},", space);
            _ = stringBuilder.AppendLine(EventHttpServiceOptions.GenerateCode().InsertingCode, space);
            _ = stringBuilder.AppendLine(EventDatabaseOptions.GenerateCode().InsertingCode, space);
            _ = stringBuilder.AppendLine(TriggerEventOptions.GenerateCode().InsertingCode, space);
            _ = stringBuilder.AppendLine("}", space);
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    public class TriggerEventOptions : ICodeGenerable
    {
        public string[] EventsList { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();

            var stringBuilder = new StringBuilder();

            varName = varName != null ? varName : "TriggerEventOptions";
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Components.Controls.TriggerEventOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            if (EventsList != null && EventsList.Length > 0)
            {
                _ = stringBuilder.AppendLine($"    EventsList = new string[]", space);
                _ = stringBuilder.AppendLine("     {", space);
                foreach (var eventList in EventsList)
                {
                    _ = stringBuilder.AppendLine($"         \"{eventList}\",", space);
                }
                _ = stringBuilder.AppendLine("     },", space);
            }
            _ = stringBuilder.AppendLine($"}}", space);
            codeResult.InsertingCode = stringBuilder.ToString();

            return codeResult;
        }
    }

    public class EventHttpServiceOptions : HttpServiceOptions
    {
        public string[] BoundData { get; set; }

        public override CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();

            var stringBuilder = new StringBuilder();
            varName = varName != null ? varName : "EventHttpServiceOptions";
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Components.Controls.EventHttpServiceOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            _ = stringBuilder.AppendLine($"    HttpServiceUrl = \"{HttpServiceUrl}\",", space);
            _ = stringBuilder.AppendLine($"    HttpMethod = \"{HttpMethod}\",", space);
            _ = stringBuilder.AppendLine($"    HttpSuccessCode = \"{HttpSuccessCode}\",", space);
            _ = stringBuilder.AppendLine($"    JsonBody = {StringUtil.ToLiteral(JsonBody)},", space);
            _ = stringBuilder.AppendLine($"    OutputProjection = \"{OutputProjection}\",", space);
            if(BoundData != null && BoundData.Length > 0)
            {
                _ = stringBuilder.AppendLine($"    BoundData = new string[]", space);
                _ = stringBuilder.AppendLine("     {", space);
                foreach(var bound in BoundData)
                {
                    _ = stringBuilder.AppendLine($"         \"{bound}\",", space);
                }                                                                        
                _ = stringBuilder.AppendLine("     },", space);
            }
            _ = stringBuilder.AppendLine($"}},", space);

            codeResult.InsertingCode = stringBuilder.ToString();

            return codeResult;
        }
    }

    public class EventDatabaseOptions: SharedDatabaseOptions
    {
        public string OutputProjection { get; set; }

        public string[] BoundData { get; set; }

        public override CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();
            varName = varName != null ? varName : "EventDatabaseOptions";
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Components.Controls.EventDatabaseOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            _ = stringBuilder.AppendLine($"    DatabaseConnectionId = \"{DatabaseConnectionId}\",", space);
            _ = stringBuilder.AppendLine($"    Query = {StringUtil.ToLiteral(Query)},", space);
            _ = stringBuilder.AppendLine($"    OutputProjection = \"{OutputProjection}\",", space);
            if (BoundData != null && BoundData.Length > 0)
            {
                _ = stringBuilder.AppendLine($"    BoundData = new string[]", space);
                _ = stringBuilder.AppendLine("     {", space);
                foreach (var bound in BoundData)
                {
                    _ = stringBuilder.AppendLine($"         \"{bound}\",", space);
                }
                _ = stringBuilder.AppendLine("     },", space);
            }
            _ = stringBuilder.AppendLine($"}},", space);
            codeResult.InsertingCode = stringBuilder.ToString();

            return codeResult;
        }
    }

    public enum EventActionType
    {
        TriggerEvent,
        QueryDatabase,
        WebService
    }
}
