using System.Collections.Generic;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Portal.Models.DynamicLists
{
    public class DynamicListFetchDataModel
    {
        public string DynamicListId { get; set; }

        public string TextSearch { get; set; }

        public FilledParameterOptions FilledParameterOptions { get; set; }

        public FilterGroupOptions FilterGroupOptions { get; set; }

        public PaginationOptions PaginationOptions { get; set; } = new PaginationOptions { PageNumber = 0, PageSize = 10 }; // Set default value when nullable

        public SortOptions SortOptions { get; set; }
    }

    public class SortOptions
    {
        public List<SortableField> SortableFields { get; set; }
    }

    public class SortableField
    {
        public string FieldName { get; set; }

        public SortType SortType { get; set; }
    }

    public enum SortType
    {
        Asc,
        Desc
    }

    public class PaginationOptions
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public bool NeedTotalItems { get; set; } = false;
    }

    public class FilledParameterOptions
    {
        public List<FilledParameter> FilledParameters { get; set; }
    }

    public class FilledParameter
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class FilterGroupOptions
    {
        public List<FilterGroup> FilterGroups { get; set; }
    }

    public class FilterGroup
    {
        public List<FilterOption> FilterOptions { get; set; }

        public FilterChainOperator FilterChainOperator { get; set; }
    }

    public class FilterOption
    {
        public string FieldName { get; set; }

        public FilterOperator FilterOperator { get; set; }

        public dynamic FieldValue { get; set; }

        public FieldValueType FilterValueType { get; set; }

        public FilterChainOperator FilterChainOperator { get; set; }
    }

    public enum FilterOperator
    {
        Equal,
        Contains,
        Lesser,
        Greater,
        Less,
        Great
    }

    public enum FilterChainOperator
    {
        None,
        And,
        Or
    }
}
