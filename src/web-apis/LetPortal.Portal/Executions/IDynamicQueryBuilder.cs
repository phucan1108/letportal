using System;
using System.Collections.Generic;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;

namespace LetPortal.Portal.Executions
{
    public interface IDynamicQueryBuilder
    {
        IDynamicQueryBuilder Init(string query, List<FilledParameter> parameters, Action<DynamicQueryBuilderOptions> action = null);

        IDynamicQueryBuilder AddTextSearch(string text, IEnumerable<string> searchFields);

        IDynamicQueryBuilder AddFilter(List<FilterGroup> groups);

        IDynamicQueryBuilder AddSort(List<SortableField> sorts);

        IDynamicQueryBuilder AddPagination(int currentPage, int numberPerPage);

        DynamicQuery Build();
    }

    public class DynamicQueryBuilderOptions
    {
        public string ParamSign { get; set; } = "@";

        public string ContainsOperatorFormat = " LIKE '%' || {0} || '%'";

        public string DateCompareFormat = "{0}::date{1}{2}::date";

        public string OrderByFormat = " ORDER BY {0}";

        public string FieldFormat { get; set; } = "\"{0}\"";

        public string SearchWord { get; set; } = Constants.SEARCH_KEY;

        public string FilterWord { get; set; } = Constants.FILTER_LIST_KEY;

        public string OrderWord { get; set; } = Constants.ORDER_KEY;

        public string CurrentPageWord { get; set; } = Constants.CURRENT_PAGE_KEY;

        public string StartRowWord { get; set; } = Constants.PAGE_START_KEY;

        public string NumberPageWord { get; set; } = Constants.PAGE_NUM_KEY;

        public string DefaultSearchNull { get; set; } = "1=1";

        public string DefaultFilterNull { get; set; } = "1=1";

        public string DefaultOrderNull { get; set; } = "1 asc";

        public int DefaultCurrentPage { get; set; } = 0;

        public int DefaultNumberPage { get; set; } = 10;

        public int DefaultStartRow { get; set; } = 0;

        public string PaginationFormat { get; set; } = "Limit {0} offset {1}";
    }

    public class DynamicQuery
    {
        public string CombinedQuery { get; set; }

        public string CombinedTotalQuery { get; set; }

        public List<DynamicQueryParameter> Parameters { get; set; }
    }

    public class DynamicQueryParameter
    {
        public string Name { get; set; }

        public FieldValueType ValueType { get; set; }

        public string ReplaceValueType { get; set; }

        public bool IsReplacedValue { get; set; }

        public string Value { get; set; }
    }
}
