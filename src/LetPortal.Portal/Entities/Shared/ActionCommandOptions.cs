using System;
using System.Linq;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Entities.Shared
{
    public class ActionCommandOptions : ICodeGenerable
    {
        public bool IsEnable { get; set; } = true;

        public ActionType ActionType { get; set; } = ActionType.Redirect;

        public RedirectOptions RedirectOptions { get; set; }

        public HttpServiceOptions HttpServiceOptions { get; set; }

        public DatabaseExecutionChains DbExecutionChains { get; set; }

        public WorkflowOptions WorkflowOptions { get; set; }

        public NotificationOptions NotificationOptions { get; set; }

        public ConfirmationOptions ConfirmationOptions { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult();

            var stringBuilder = new StringBuilder();
            varName = varName != null ? varName : "ActionCommandOptions";
            _ = stringBuilder.AppendLine($"{varName} = new LetPortal.Portal.Entities.Shared.ActionCommandOptions", space);
            _ = stringBuilder.AppendLine($"{{", space);
            var actionType = "LetPortal.Portal.Entities.Shared.ActionType." + Enum.GetName(typeof(ActionType), ActionType);
            _ = stringBuilder.AppendLine($"    ActionType = {actionType},", space);
            _ = stringBuilder.AppendLine($"    IsEnable = {IsEnable.ToString().ToLower()},", space);
            if (IsEnable)
            {
                //if(MapDataOptions != null)
                //{
                //    _ = stringBuilder.AppendLine(MapDataOptions.GenerateCode("MapDataOptions", space + 4).InsertingCode, space);
                //}
                if (ConfirmationOptions != null)
                {
                    _ = stringBuilder.AppendLine($"    ConfirmationOptions = new LetPortal.Portal.Entities.Shared.ConfirmationOptions", space);
                    _ = stringBuilder.AppendLine($"    {{", space);
                    _ = stringBuilder.AppendLine($"        IsEnable = {ConfirmationOptions.IsEnable.ToString().ToLower()},", space);
                    _ = stringBuilder.AppendLine($"        ConfirmationText = \"{ConfirmationOptions.ConfirmationText}\",", space);
                    _ = stringBuilder.AppendLine($"    }},", space);
                }
                if (NotificationOptions != null)
                {
                    _ = stringBuilder.AppendLine($"    NotificationOptions = new LetPortal.Portal.Entities.Shared.NotificationOptions", space);
                    _ = stringBuilder.AppendLine($"    {{", space);
                    _ = stringBuilder.AppendLine($"        CompleteMessage = \"{NotificationOptions.CompleteMessage}\",", space);
                    _ = stringBuilder.AppendLine($"        FailedMessage = \"{NotificationOptions.FailedMessage}\",", space);
                    _ = stringBuilder.AppendLine($"    }},", space);
                }
                switch (ActionType)
                {
                    case ActionType.Redirect:
                        _ = stringBuilder.AppendLine($"    RedirectOptions = new LetPortal.Portal.Entities.Shared.RedirectOptions", space);
                        _ = stringBuilder.AppendLine($"    {{", space);
                        _ = stringBuilder.AppendLine($"        IsSameDomain = {RedirectOptions.IsSameDomain.ToString().ToLower()},", space);
                        _ = stringBuilder.AppendLine($"        RedirectUrl = \"{RedirectOptions.RedirectUrl}\"", space);
                        _ = stringBuilder.AppendLine($"    }},", space);
                        break;
                    case ActionType.ExecuteDatabase:
                        _ = stringBuilder.AppendLine($"    DbExecutionChains = new LetPortal.Portal.Entities.Shared.DatabaseExecutionChains", space);
                        _ = stringBuilder.AppendLine($"    {{", space);
                        _ = stringBuilder.AppendLine($"        Steps = new List<LetPortal.Portal.Entities.Shared.DatabaseExecutionStep>", space);
                        _ = stringBuilder.AppendLine($"        {{", space);
                        foreach (var step in DbExecutionChains.Steps)
                        {
                            _ = stringBuilder.AppendLine($"            new LetPortal.Portal.Entities.Shared.DatabaseExecutionStep", space);
                            _ = stringBuilder.AppendLine($"            {{", space);
                            _ = stringBuilder.AppendLine($"                DatabaseConnectionId = \"{step.DatabaseConnectionId}\",", space);
                            _ = stringBuilder.AppendLine($"                ExecuteCommand = {StringUtil.ToLiteral(step.ExecuteCommand)},", space);
                            _ = stringBuilder.AppendLine($"                DataLoopKey = \"{step.DataLoopKey}\",", space);
                            if(step != DbExecutionChains.Steps.Last())
                            {
                                _ = stringBuilder.AppendLine($"            }},", space);
                            }
                            else
                            {
                                _ = stringBuilder.AppendLine($"            }}", space);
                            }
                        }
                        _ = stringBuilder.AppendLine($"       }}", space);
                        _ = stringBuilder.AppendLine($"    }},", space);
                        break;
                    case ActionType.CallHttpService:
                        _ = stringBuilder.AppendLine(HttpServiceOptions.GenerateCode(space: space + 1).InsertingCode);
                        break;
                    case ActionType.CallWorkflow:
                        // TODO: We will implement this later
                        break;
                }
            }            
            _ = stringBuilder.AppendLine($"}},", space);
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }
}
