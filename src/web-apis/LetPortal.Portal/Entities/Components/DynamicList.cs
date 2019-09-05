using LetPortal.Portal.Entities.Shared;
using System;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.SectionParts
{
    public class DynamicList : Component
    {
        public DynamicListDatasource ListDatasource { get; set; }

        public ParamsList ParamsList { get; set; }

        public FiltersList FiltersList { get; set; }

        public ColumnsList ColumnsList { get; set; }

        public CommandsList CommandsList { get; set; }

        public void GenerateFilters()
        {
            if(ColumnsList != null && ColumnsList.ColumndDefs.Count > 0)
            {
                FiltersList = new FiltersList
                {
                    FilterFields = new List<FilterField>()
                };
                foreach(var col in ColumnsList.ColumndDefs)
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

        public DatabaseOptions DatabaseConnectionOptions { get; set; }

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
        public List<ColumndDef> ColumndDefs { get; set; } = new List<ColumndDef>();
    }

    public class ColumndDef
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
