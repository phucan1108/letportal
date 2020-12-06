using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Components
{
    public class Chart : Component, ICodeGenerable
    {
        public ChartDefinitions Definitions { get; set; }

        public Portal.Entities.Shared.SharedDatabaseOptions DatabaseOptions { get; set; }

        public List<ChartFilter> ChartFilters { get; set; }

        public void TurnOffRealTime()
        {
            foreach (var option in Options)
            {
                if (option.Key == "allowrealtime")
                {
                    option.Value = "false";
                }
            }
        }

        public void SetRealTimeField(string fieldName)
        {
            foreach (var option in Options)
            {
                if (option.Key == "comparerealtimefield")
                {
                    option.Value = fieldName;
                }
            }
        }

        public void SetDataRange(string dataRange)
        {
            foreach (var option in Options)
            {
                if (option.Key == "datarange")
                {
                    option.Value = dataRange;
                }
            }
        }

        public void SetXFormatDate(string xFormatDate)
        {
            foreach (var option in Options)
            {
                if (option.Key == "xformatdate")
                {
                    option.Value = xFormatDate;
                }
            }
        }

        public CodeGenerableResult GenerateCode(string varName = null, int space = 0)
        {
            var codeResult = new CodeGenerableResult
            {
                DeletingCode = $"versionContext.DeleteData<LetPortal.Portal.Entities.Components.Chart>(\"{Id}\");"
            };

            varName ??= Name.Replace("-", "", System.StringComparison.OrdinalIgnoreCase) + "Chart";
            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine($"var {varName} = new LetPortal.Portal.Entities.Components.Chart");
            _ = stringBuilder.AppendLine($"{{");
            _ = stringBuilder.AppendLine($"    Id = \"{Id}\",");
            _ = stringBuilder.AppendLine($"    Name = \"{Name}\",");
            _ = stringBuilder.AppendLine($"    AppId = \"{AppId}\",");
            _ = stringBuilder.AppendLine($"    DisplayName = \"{DisplayName}\",");
            var layoutType = "LetPortal.Portal.Entities.SectionParts.PageSectionLayoutType." + Enum.GetName(typeof(PageSectionLayoutType), LayoutType);
            _ = stringBuilder.AppendLine($"    LayoutType = {layoutType},");
            _ = stringBuilder.AppendLine(DatabaseOptions.GenerateCode(space: 1).InsertingCode);
            _ = stringBuilder.AppendLine($"    TimeSpan = {TimeSpan},");
            if (Definitions != null)
            {
                _ = stringBuilder.AppendLine($"    Definitions = new LetPortal.Portal.Entities.Components.ChartDefinitions");
                _ = stringBuilder.AppendLine($"    {{");
                _ = stringBuilder.AppendLine($"        ChartTitle = \"{Definitions.ChartTitle}\",");
                var chartType = "ChartType." + Enum.GetName(typeof(ChartType), Definitions.ChartType);
                _ = stringBuilder.AppendLine($"        ChartType = {chartType},");
                _ = stringBuilder.AppendLine($"        MappingProjection = \"{Definitions.MappingProjection}\",");
                _ = stringBuilder.AppendLine($"    }},");
            }

            if (Options != null && Options.Any())
            {
                _ = stringBuilder.AppendLine($"    Options = new System.Collections.Generic.List<LetPortal.Portal.Entities.Pages.ShellOption>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach (var option in Options)
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
            }
            if (ChartFilters != null && ChartFilters.Any())
            {
                _ = stringBuilder.AppendLine($"    ChartFilters = new System.Collections.Generic.List<LetPortal.Portal.Entities.Components.ChartFilter>");
                _ = stringBuilder.AppendLine($"    {{");
                foreach (var filter in ChartFilters)
                {
                    _ = stringBuilder.AppendLine($"        new LetPortal.Portal.Entities.Components.ChartFilter");
                    _ = stringBuilder.AppendLine($"        {{");
                    _ = stringBuilder.AppendLine($"            Name = \"{filter.Name}\",");
                    _ = stringBuilder.AppendLine($"            DisplayName = \"{filter.DisplayName}\",");
                    _ = stringBuilder.AppendLine($"            IsMultiple = {filter.IsMultiple.ToString().ToLower()},");
                    var chartFilterType = "LetPortal.Portal.Entities.Components.FilterType." + Enum.GetName(typeof(FilterType), filter.Type);
                    _ = stringBuilder.AppendLine($"            Type = {chartFilterType}");

                    if(filter != ChartFilters.Last())
                    {
                        _ = stringBuilder.AppendLine($"        }},");
                    }
                    else
                    {
                        _ = stringBuilder.AppendLine($"        }}");
                    }                       
                }
                _ = stringBuilder.AppendLine($"    }}");
            }
            _ = stringBuilder.AppendLine($"}};");
            _ = stringBuilder.AppendLine($"versionContext.InsertData({varName});");
            codeResult.InsertingCode = stringBuilder.ToString();
            return codeResult;
        }
    }

    public class ChartDefinitions
    {
        public string ChartTitle { get; set; }

        public ChartType ChartType { get; set; }

        /// <summary>
        /// This field helps to arrange Chart structure
        /// Ex: Projection must be 'name=displayName;value=reportValue;group=groupName;'
        /// Also, we support some extra field: 'min=min;max=max;extra-code=countryCode'
        /// </summary>
        public string MappingProjection { get; set; }
    }

    public enum ChartType
    {
        VerticalBarChart,
        HorizontalBarChart,
        GroupedVerticalBarChart,
        GroupedHorizontalBarChart,
        PieChart,
        AdvancedPieChart,
        PieGrid,
        LineChart,
        AreaChart,
        Gauge,
        NumberCard
    }

    public class ChartFilter
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public FilterType Type { get; set; }

        public DatasourceOptions DatasourceOptions { get; set; }

        /// <summary>
        /// Support for NumberPicker/ DatePicker/ MonthPicker 
        /// Guide: 
        /// * For NumberPicker: [10,20,30] or [10..20] or [10..2..20] or [10-20,20-30,30-40]
        /// * For DatePicker: ['10/10/2019'] or ['10/10/2019', '10/10/3000'] or ['10/10/2019','Now']
        /// * For MonthPicker: ['10/2018'] or ['10/2018', '10/3000'] or ['10/2018', 'Now']
        /// </summary>
        public string RangeValue { get; set; }

        /// <summary>
        /// There are some default values that can be set. According to FilterType:
        /// Checkbox: true/false (not 0/1)
        /// Select: any value that matchs Datasource, allow ['a','b'] for multiple filtertype
        /// NumberPicker: 10 or [10,20,30] when it is multiple
        /// DatePicker: '12/10/2019' or ['12/9/2019','12/20/2019'] for multiple DatePicker
        /// MonthYearPicker: '12/2019' or ['9/2019','12/2020'] for multiple MonthYearPicker
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// When this mode is true, DefaultValue will be used to query data
        /// </summary>
        public bool AllowDefaultValue { get; set; }

        /// <summary>
        /// Support some filter type with Range and Select. Allow user choose multiple option
        /// Ex: 
        /// Select: Allow multiple options: ['A','B']
        /// NumberPicker: Can pick multiple number: [10,20] or [10-20,20-30]
        /// DatePicker: Can pick StartDate - EndDate: ['10/20/2018','10/20/2019']
        /// MonthPicker: Can pick StartMonthYear - EndMonthYear: ['10/2018','12/2019']
        /// Note: Only DatePicker and MonthPicker can use Now for generating current DateTime.Now
        /// </summary>
        public bool IsMultiple { get; set; }

        public bool IsHidden { get; set; }
    }

    public enum FilterType
    {
        None,
        Checkbox,
        Select,
        NumberPicker,
        DatePicker,
        MonthYearPicker
    }
}
