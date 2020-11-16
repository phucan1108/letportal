using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Core.Utils;

namespace LetPortal.Portal.Entities.SectionParts
{
    public class DynamicList : Component, ICodeGenerable
    {
        public DynamicListDatasource ListDatasource { get; set; }

        public ParamsList ParamsList { get; set; }

        public FiltersList FiltersList { get; set; }

        public ColumnsList ColumnsList { get; set; }

        public CommandsList CommandsList { get; set; }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult
            {
                DeletingCode = $"versionContext.DeleteData<LetPortal.Portal.Entities.SectionParts.DynamicList>(\"{Id}\");"
            };

            var stringBuilder = new StringBuilder();
            varName ??= Name.Replace("-", "", StringComparison.OrdinalIgnoreCase) + "List";
            _ = stringBuilder.AppendLine($"var {varName} = new LetPortal.Portal.Entities.SectionParts.DynamicList");
            _ = stringBuilder.AppendLine("{");
            _ = stringBuilder.AppendLine($"    Id = \"{Id}\",");
            _ = stringBuilder.AppendLine($"    Name = \"{Name}\",");
            _ = stringBuilder.AppendLine($"    AppId = \"{AppId}\",");
            _ = stringBuilder.AppendLine($"    DisplayName = \"{DisplayName}\",");
            _ = stringBuilder.AppendLine($"    Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>");
            _ = stringBuilder.AppendLine($"    {{");
            foreach(var option in Options)
            {
                _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Pages.ShellOption");
                _ = stringBuilder.AppendLine($"        {{");
                _ = stringBuilder.AppendLine($"            Key = \"{option.Key}\",");
                _ = stringBuilder.AppendLine($"            Value = \"{option.Value}\",");
                _ = stringBuilder.AppendLine($"            Description = \"{option.Description}\"");
                if(option != Options.Last())
                {
                    _ = stringBuilder.AppendLine($"        }},");
                }
                else
                {
                    _ = stringBuilder.AppendLine($"        }}");
                }
            }
            _ = stringBuilder.AppendLine($"    }},");
            _ = stringBuilder.AppendLine($"    ListDatasource = new LetPortal.Portal.Entities.SectionParts.DynamicListDatasource");
            _ = stringBuilder.AppendLine($"    {{");
            if(ListDatasource.SourceType == DynamicListSourceType.Database)
            {
                _ = stringBuilder.AppendLine(ListDatasource.DatabaseConnectionOptions.GenerateCode("DatabaseConnectionOptions", 2).InsertingCode);
                _ = stringBuilder.AppendLine($"        SourceType = LetPortal.Portal.Entities.SectionParts.DynamicListSourceType.Database");
            }
            else
            {
                // TODO: implement for web service datasource
            }
            _ = stringBuilder.AppendLine($"    }},");

            if(ColumnsList != null && ColumnsList.ColumnDefs != null && ColumnsList.ColumnDefs.Any())
            {
                _ = stringBuilder.AppendLine($"    ColumnsList = new LetPortal.Portal.Entities.SectionParts.ColumnsList");
                _ = stringBuilder.AppendLine($"    {{");
                _ = stringBuilder.AppendLine($"        ColumnDefs = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.ColumnDef>");
                _ = stringBuilder.AppendLine($"        {{");
                foreach (var colDef in ColumnsList.ColumnDefs)
                {
                    _ = stringBuilder.AppendLine($"            new LetPortal.Portal.Entities.SectionParts.ColumnDef");
                    _ = stringBuilder.AppendLine($"            {{");
                    _ = stringBuilder.AppendLine($"                Name = \"{colDef.Name}\",");
                    _ = stringBuilder.AppendLine($"                DisplayName = \"{colDef.DisplayName}\",");
                    _ = stringBuilder.AppendLine($"                IsHidden = {colDef.IsHidden.ToString().ToLower()},");
                    _ = stringBuilder.AppendLine($"                DisplayFormat = \"{colDef.DisplayFormat}\",");
                    _ = stringBuilder.AppendLine($"                AllowSort = {colDef.AllowSort.ToString().ToLower()},");
                    _ = stringBuilder.AppendLine($"                HtmlFunction = \"{colDef.HtmlFunction}\",");
                    _ = stringBuilder.AppendLine($"                DisplayFormatAsHtml = {colDef.DisplayFormatAsHtml.ToString().ToLower()},");
                    _ = stringBuilder.AppendLine($"                Order = {colDef.Order},");
                    if(colDef.SearchOptions != null)
                    {
                        _ = stringBuilder.AppendLine($"                SearchOptions = new LetPortal.Portal.Entities.SectionParts.SearchOptions");
                        _ = stringBuilder.AppendLine($"                {{");
                        _ = stringBuilder.AppendLine($"                    AllowInAdvancedMode = {colDef.SearchOptions.AllowInAdvancedMode.ToString().ToLower()},");
                        _ = stringBuilder.AppendLine($"                    AllowTextSearch = {colDef.SearchOptions.AllowInAdvancedMode.ToString().ToLower()},");
                        var fieldValueTypeStr = "LetPortal.Portal.Entities.SectionParts.FieldValueType." + Enum.GetName(typeof(FieldValueType), colDef.SearchOptions.FieldValueType);
                        _ = stringBuilder.AppendLine($"                    FieldValueType = {fieldValueTypeStr}");
                        _ = stringBuilder.AppendLine($"                }},");
                    }

                    if(colDef.DatasourceOptions != null)
                    {
                        _ = stringBuilder.AppendLine($"                DatasourceOptions = new LetPortal.Portal.Entities.Shared.DynamicListDatasourceOptions");
                        _ = stringBuilder.AppendLine($"                {{");
                        var typeStr = "LetPortal.Portal.Entities.Shared.DatasourceControlType." + Enum.GetName(typeof(DatasourceControlType), colDef.DatasourceOptions.Type);
                        _ = stringBuilder.AppendLine($"                    Type = {typeStr},");
                        _ = stringBuilder.AppendLine($"                    TriggeredEvents = \"{colDef.DatasourceOptions.TriggeredEvents}\",");
                        _ = stringBuilder.AppendLine($"                    OutputMapProjection = \"{colDef.DatasourceOptions.OutputMapProjection}\",");
                        switch (colDef.DatasourceOptions.Type)
                        {
                            case DatasourceControlType.StaticResource:
                                _ = stringBuilder.AppendLine($"                   DatasourceStaticOptions = new LetPortal.Portal.Entities.Shared.DatasourceStaticOptions");
                                _ = stringBuilder.AppendLine($"                   {{");
                                _ = stringBuilder.AppendLine($"                        JsonResource = {StringUtil.ToLiteral(colDef.DatasourceOptions.DatasourceStaticOptions.JsonResource)}");
                                _ = stringBuilder.AppendLine($"                   }},");
                                break;
                            case DatasourceControlType.Database:
                                _ = stringBuilder.AppendLine(colDef.DatasourceOptions.DatabaseOptions.GenerateCode("DatabaseOptions", space: 4).InsertingCode);
                                break;
                            case DatasourceControlType.WebService:
                                // TODO: We will implement this later
                                break;
                        }
                        _ = stringBuilder.AppendLine($"                }},");
                    }
                    
                    if(colDef != ColumnsList.ColumnDefs.Last())
                    {
                        _ = stringBuilder.AppendLine($"            }},");
                    }
                    else
                    {
                        _ = stringBuilder.AppendLine($"            }}");
                    }
                    
                }
                _ = stringBuilder.AppendLine($"        }}");
                _ = stringBuilder.AppendLine($"    }},");
            }

            if (CommandsList != null && CommandsList.CommandButtonsInList.Any())
            {
                _ = stringBuilder.AppendLine($"    CommandsList = new LetPortal.Portal.Entities.SectionParts.CommandsList");
                _ = stringBuilder.AppendLine($"    {{");
                _ = stringBuilder.AppendLine($"        CommandButtonsInList = new System.Collections.Generic.List<LetPortal.Portal.Entities.SectionParts.CommandButtonInList>");
                _ = stringBuilder.AppendLine($"        {{");
                foreach(var command in CommandsList.CommandButtonsInList)
                {
                    _ = stringBuilder.AppendLine($"            new LetPortal.Portal.Entities.SectionParts.CommandButtonInList");
                    _ = stringBuilder.AppendLine($"            {{");
                    _ = stringBuilder.AppendLine($"                Name = \"{command.Name}\",");
                    _ = stringBuilder.AppendLine($"                DisplayName = \"{command.DisplayName}\",");
                    _ = stringBuilder.AppendLine($"                Color = \"{command.Color}\",");
                    _ = stringBuilder.AppendLine($"                Icon = \"{command.Icon}\",");
                    _ = stringBuilder.AppendLine($"                AllowRefreshList = {command.AllowRefreshList.ToString().ToLower()},");
                    var commandPosition = "LetPortal.Portal.Entities.SectionParts.CommandPositionType." + Enum.GetName(typeof(CommandPositionType), command.CommandPositionType);
                    _ = stringBuilder.AppendLine($"                CommandPositionType = {commandPosition},");
                    _ = stringBuilder.AppendLine($"                Order = {command.Order},");
                    _ = stringBuilder.AppendLine(command.ActionCommandOptions.GenerateCode(space: 4).InsertingCode);
                    if(command != CommandsList.CommandButtonsInList.Last())
                    {
                        _ = stringBuilder.AppendLine($"            }},");
                    }
                    else
                    {
                        _ = stringBuilder.AppendLine($"            }}");
                    }
                }
                _ = stringBuilder.AppendLine($"        }}");
                _ = stringBuilder.AppendLine($"    }},");
            }
            _ = stringBuilder.AppendLine("};");

            _ = stringBuilder.AppendLine($"versionContext.InsertData({varName});");
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }

        public void GenerateFilters()
        {
            if (ColumnsList != null && ColumnsList.ColumnDefs.Count > 0)
            {
                FiltersList = new FiltersList
                {
                    FilterFields = new List<FilterField>()
                };
                foreach (var col in ColumnsList.ColumnDefs)
                {
                    FiltersList.FilterFields.Add(new FilterField
                    {
                        Name = col.Name,
                        AllowInAdvancedMode = col.SearchOptions.AllowInAdvancedMode,
                        AllowTextSearch = col.SearchOptions.AllowTextSearch,
                        FieldValueType = col.SearchOptions.FieldValueType
                    });
                }
            }
        }
    }

    public class DynamicListDatasource
    {
        public DynamicListSourceType SourceType { get; set; }

        public Shared.SharedDatabaseOptions DatabaseConnectionOptions { get; set; }

        public HttpServiceOptions HttpServiceOptions { get; set; }
    }

    public enum DynamicListSourceType
    {
        Database,
        HttpService
    }

    public enum FieldValueType
    {
        Text,
        Select,
        Checkbox,
        Slide,
        DatePicker,
        Number
    }

    public class ParamsList
    {
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }

    public class Parameter
    {
        public string Name { get; set; }
    }

    public class FiltersList
    {
        public List<FilterField> FilterFields { get; set; } = new List<FilterField>();
    }

    public class FilterField
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public FieldValueType FieldValueType { get; set; }

        public bool IsHidden { get; set; }

        public bool AllowTextSearch { get; set; }

        public bool AllowInAdvancedMode { get; set; }
    }

    public class ColumnsList
    {
        public List<ColumnDef> ColumnDefs { get; set; } = new List<ColumnDef>();
    }

    public class ColumnDef
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string DisplayFormat { get; set; }

        public string HtmlFunction { get; set; }

        public bool DisplayFormatAsHtml { get; set; }

        public bool AllowSort { get; set; }

        public SearchOptions SearchOptions { get; set; }

        /// <summary>
        /// Special property: Only use with FilterValueType = Select
        /// </summary>
        public DynamicListDatasourceOptions DatasourceOptions { get; set; }

        public bool IsHidden { get; set; }

        public int Order { get; set; }
    }

    public class SearchOptions
    {
        public FieldValueType FieldValueType { get; set; }

        public bool AllowTextSearch { get; set; }

        public bool AllowInAdvancedMode { get; set; }
    }

    public class CommandsList
    {
        public List<CommandButtonInList> CommandButtonsInList { get; set; } = new List<CommandButtonInList>();
    }

    public class CommandButtonInList
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Color { get; set; }

        public CommandPositionType CommandPositionType { get; set; }

        public ActionCommandOptions ActionCommandOptions { get; set; }

        public bool AllowRefreshList { get; set; }

        public int Order { get; set; }
    }

    public enum CommandPositionType
    {
        InList,
        OutList
    }
}
