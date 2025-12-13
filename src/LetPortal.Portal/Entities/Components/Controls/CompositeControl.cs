using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Entities.SectionParts.Controls;

namespace LetPortal.Portal.Entities.Components.Controls
{
    [EntityCollection(Name = "compositecontrols")]
    [Table("compositecontrols")]
    public class CompositeControl : Entity, ICodeGenerable
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string AppId { get; set; }

        public List<PageControl> Controls { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult()
            {
                DeletingCode = $"versionContext.DeleteData<LetPortal.Portal.Entities.Components.Controls.CompositeControl>(\"{Id}\");"
            };
            var stringBuilder = new StringBuilder();

            varName ??= Name.Replace("-", "", System.StringComparison.OrdinalIgnoreCase) + "Standard";

            _ = stringBuilder.AppendLine($"var {varName} = new LetPortal.Portal.Entities.Components.Controls.CompositeControl");
            _ = stringBuilder.AppendLine($"{{");
            _ = stringBuilder.AppendLine($"    Id = \"{Id}\",");
            _ = stringBuilder.AppendLine($"    Name = \"{Name}\",");
            _ = stringBuilder.AppendLine($"    DisplayName = \"{DisplayName}\",");
            _ = stringBuilder.AppendLine($"    AppId = \"{AppId}\",");
            if (Controls != null && Controls.Any())
            {
                _ = stringBuilder.AppendLine($"    Controls = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControl>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach (var control in Controls)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.SectionParts.Controls.PageControl");
                    _ = stringBuilder.AppendLine($"        {{");
                    _ = stringBuilder.AppendLine($"            Name = \"{control.Name}\",");
                    var controlType = "LetPortal.Portal.Entities.SectionParts.Controls.ControlType." + Enum.GetName(typeof(ControlType), control.Type);
                    _ = stringBuilder.AppendLine($"            Type = {controlType},");
                    _ = stringBuilder.AppendLine($"            Order = {control.Order},");
                    _ = stringBuilder.AppendLine($"            CompositeControlId = \"{control.CompositeControlId}\",");
                    // Control Options
                    _ = stringBuilder.AppendLine($"            Options = new List<LetPortal.Portal.Entities.Pages.ShellOption>");
                    _ = stringBuilder.AppendLine($"            {{");
                    foreach (var option in control.Options)
                    {
                        _ = stringBuilder.AppendLine($"                new LetPortal.Portal.Entities.Pages.ShellOption");
                        _ = stringBuilder.AppendLine($"                {{");
                        _ = stringBuilder.AppendLine($"                    Key = \"{option.Key}\",");
                        _ = stringBuilder.AppendLine($"                    Value = \"{option.Value}\",");
                        _ = stringBuilder.AppendLine($"                    Description = \"{option.Description}\",");
                        _ = stringBuilder.AppendLine($"                }},");
                    }
                    _ = stringBuilder.AppendLine($"            }},");

                    // Control Datasource
                    if (control.DatasourceOptions != null && (control.Type == ControlType.Select || control.Type == ControlType.Radio || control.Type == ControlType.AutoComplete))
                    {
                        _ = stringBuilder.AppendLine(control.DatasourceOptions.GenerateCode().InsertingCode, 4);
                    }
                    // Control Validators
                    _ = stringBuilder.AppendLine($"            Validators = new List<LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator>");
                    _ = stringBuilder.AppendLine($"            {{");
                    foreach (var validator in control.Validators)
                    {
                        _ = stringBuilder.AppendLine($"                new LetPortal.Portal.Entities.SectionParts.Controls.PageControlValidator");
                        _ = stringBuilder.AppendLine($"                {{");
                        var validatorType = "LetPortal.Portal.Entities.SectionParts.Controls.ValidatorType." + Enum.GetName(typeof(ValidatorType), validator.ValidatorType);
                        _ = stringBuilder.AppendLine($"                    ValidatorType = {validatorType},");
                        _ = stringBuilder.AppendLine($"                    IsActive = {validator.IsActive.ToString().ToLower()},");
                        _ = stringBuilder.AppendLine($"                    ValidatorMessage = \"{validator.ValidatorMessage}\",");
                        _ = stringBuilder.AppendLine($"                    ValidatorOption = \"{validator.ValidatorOption}\",");
                        _ = stringBuilder.AppendLine($"                }},");
                    }
                    _ = stringBuilder.AppendLine($"            }},");

                    // Control Async Validators
                    if (control.AsyncValidators != null && control.AsyncValidators.Any())
                    {
                        _ = stringBuilder.AppendLine($"            AsyncValidators = new List<LetPortal.Portal.Entities.Components.Controls.PageControlAsyncValidator>");
                        _ = stringBuilder.AppendLine($"            {{");
                        foreach (var validator in control.AsyncValidators)
                        {
                            _ = stringBuilder.AppendLine($"                new LetPortal.Portal.Entities.Components.Controls.PageControlAsyncValidator");
                            _ = stringBuilder.AppendLine($"                {{");
                            _ = stringBuilder.AppendLine($"                    ValidatorName = \"{validator.ValidatorName}\",");
                            _ = stringBuilder.AppendLine($"                    IsActive = {validator.IsActive.ToString().ToLower()},");
                            _ = stringBuilder.AppendLine($"                    ValidatorMessage = \"{validator.ValidatorMessage}\",");
                            _ = stringBuilder.AppendLine($"                    AsyncValidatorOptions = new LetPortal.Portal.Entities.Components.Controls.ControlAsyncValidatorOptions");
                            _ = stringBuilder.AppendLine($"                    {{");
                            _ = stringBuilder.AppendLine($"                        EvaluatedExpression = \"{validator.AsyncValidatorOptions.EvaluatedExpression}\",");
                            if (validator.AsyncValidatorOptions.ValidatorType == Components.Controls.AsyncValidatorType.DatabaseValidator)
                            {
                                _ = stringBuilder.AppendLine(validator.AsyncValidatorOptions.DatabaseOptions.GenerateCode(space: 6).InsertingCode);
                                _ = stringBuilder.AppendLine($"                        ValidatorType = LetPortal.Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator,");
                            }
                            else if (validator.AsyncValidatorOptions.ValidatorType == Components.Controls.AsyncValidatorType.HttpValidator)
                            {
                                _ = stringBuilder.AppendLine(validator.AsyncValidatorOptions.HttpServiceOptions.GenerateCode(space: 6).InsertingCode);
                                _ = stringBuilder.AppendLine($"                        ValidatorType = LetPortal.Portal.Entities.Components.Controls.AsyncValidatorType.HttpValidator,");
                            }
                            _ = stringBuilder.AppendLine($"                    }}");
                            _ = stringBuilder.AppendLine($"                }},");
                        }
                        _ = stringBuilder.AppendLine($"            }},");
                    }

                    // Control Event
                    if (control.PageControlEvents != null && control.PageControlEvents.Any())
                    {
                        _ = stringBuilder.AppendLine($"            PageControlEvents = new List<LetPortal.Portal.Entities.Components.Controls.PageControlEvent>");
                        _ = stringBuilder.AppendLine($"            {{");
                        foreach (var controlEvent in control.PageControlEvents)
                        {
                            _ = stringBuilder.AppendLine(controlEvent.GenerateCode().InsertingCode, 3);
                        }
                        _ = stringBuilder.AppendLine($"            }},");
                    }
                    if (control != Controls.Last())
                    {
                        _ = stringBuilder.AppendLine($"        }},");
                    }
                    else
                    {
                        _ = stringBuilder.AppendLine($"        }}");
                    }
                }

                _ = stringBuilder.AppendLine($"    }},");
            }
            _ = stringBuilder.AppendLine($"}};");
            _ = stringBuilder.AppendLine($"versionContext.InsertData({varName});");
            codeResult.InsertingCode = stringBuilder.ToString();

            return codeResult;
        }
    }
}
