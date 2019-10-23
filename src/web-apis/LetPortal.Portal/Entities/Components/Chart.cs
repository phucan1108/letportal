using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Components
{
    public class Chart : Component
    { 
        public ChartDefinitions Definitions { get; set; }

        public DatabaseOptions DatabaseOptions { get; set; }

        public List<ChartFilter> ChartFilters { get; set; }
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
        Gauge        
    }

    public class ChartFilter
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public FilterType Type { get; set; }

        public DatasourceOptions DatasourceOptions { get; set; }

        /// <summary>
        /// Only support for Range Type. 
        /// Ex: NumberRange = '[1000,2000,3000]', DateRange = '[10/10/2019,12/10/2019]', MonthYearRange = '[1/1990,NOW]'
        /// HourRange='[0,24]'
        /// </summary>
        public string RangeValue { get; set; }

        public bool IsHidden { get; set; }
    }

    public enum FilterType
    {
        Checkbox,
        Select,
        NumberRange,
        DatePicker,
        DateRange,
        MonthYearPicker,
        MonthYearRange,
        HourPicker,
        HourRange
    }
}
