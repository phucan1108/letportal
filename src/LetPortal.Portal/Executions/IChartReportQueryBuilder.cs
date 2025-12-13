using System;
using System.Collections.Generic;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions
{
    public interface IChartReportQueryBuilder
    {
        IChartReportQueryBuilder Init(
            string formattedString,
            string mappingProjection,
            Action<ChartReportQueryOptions> options = null);

        IChartReportQueryBuilder AddParameters(IEnumerable<ChartParameterValue> parameters);

        IChartReportQueryBuilder AddFilters(IEnumerable<ChartFilterValue> filterValues);

        IChartReportQueryBuilder AddRealTime(string comparedField, DateTime lastComparedDate, DateTime comparedDate);

        IChartReportQueryBuilder AddMapper(Func<string, string, object> mapperFunc);

        ChartReportQuery Build();
    }

    public class ChartReportQueryOptions
    {
        public string RealTimeWord { get; set; } = Constants.REAL_TIME_KEY;

        public string FilterWord { get; set; } = Constants.FILTER_KEY;

        public string DefaultRealTimeNull { get; set; } = "1=1";

        public string DefaultFilterNull { get; set; } = "1=1";

        public string WrapperQuery { get; set; } = "SELECT {0} FROM ({1}) s";

        public string FieldFormat { get; set; } = "\"{0}\"";

        public string ParamSign { get; set; } = "@";

        public bool AllowBoolIsInt { get; set; } = false;

        public string InOperator { get; set; } = "({0} IN ({1}))";

        public string NumberRangeCompare { get; set; } = "({0} BETWEEN {1} AND {2})";

        public string DateCompare { get; set; } = "{0}::date{1}{2}::date";

        public string MonthCompare { get; set; } = "date_part('month',{0}){1}date_part('month', timestamp {2})";

        public string YearCompare { get; set; } = "date_part('year',{0}){1}date_part('year', timestamp {2})";

    }

    public class ChartReportQuery
    {
        public string CombinedQuery { get; set; }

        public List<ChartReportParameter> DbParameters { get; set; }
    }

    public class ChartReportParameter
    {
        public string Name { get; set; }

        public string ValueType { get; set; }

        public object CastedValue { get; set; }
    }
}
