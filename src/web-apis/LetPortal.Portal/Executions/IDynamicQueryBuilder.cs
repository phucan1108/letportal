using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using System;
using System.Collections.Generic;

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

        public string OrderByFormat = " ORDER BY {0}";

        public string FieldFormat { get; set; } = "\"{0}\"";

        public string SearchWord { get; set; } = "{{SEARCH}}";

        public string FilterWord { get; set; } = "{{FILTER}}";

        public string OrderWord { get; set; } = "{{ORDER}}";

        public string CurrentPageWord { get; set; } = "{{PAGECURRENT}}";

        public string StartRowWord { get; set; } = "{{PAGESTART}}";

        public string NumberPageWord { get; set; } = "{{PAGENUM}}";

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

        public List<DynamicQueryParameter> Parameters { get; set; }
    }

    public class DynamicQueryParameter
    {
        public string Name { get; set; }

        public FieldValueType ValueType { get; set; }

        public string Value { get; set; }
    }
}
