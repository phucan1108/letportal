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
        /// Support for NumberPicker/ DatePicker/ MonthPicker 
        /// Guide: 
        /// * For NumberPicker: [10,20,30] or [10..20] or [10..2..20] or [10-20,20-30,30-40]
        /// * For DatePicker: ['10/10/2019','Now']
        /// * For MonthPicker: ['10/2018', 'Now']
        /// </summary>
        public string RangeValue { get; set; }

        /// <summary>
        /// Support some filter type with Range and Select. Allow user choose multiple option
        /// Ex: 
        /// Select: Allow multiple options: ['A','B']
        /// NumberPicker: Can pick multiple number: [10,20] or [10-20,20-30]
        /// DatePicker: Can pick StartDate - EndDate: ['10/20/2018','10/20/2019']
        /// MonthPicker: Can pick StartMonthYear - EndMonthYear: ['10/2018','12/2019']
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
