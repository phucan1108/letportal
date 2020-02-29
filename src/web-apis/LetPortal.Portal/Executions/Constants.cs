namespace LetPortal.Portal.Executions
{
    class Constants
    {
        // Standard
        public const string QUERY_KEY = "$query";
        public const string INSERT_KEY = "$insert";
        public const string UPDATE_KEY = "$update";
        public const string DELETE_KEY = "$delete";
        public const string DATA_KEY = "$data";
        public const string WHERE_KEY = "$where";
        public const string UNION_KEY = "$union";
        public const string JOIN_KEY = "$join";

        // Special parameter name
        // Usable only for AutoComplete Control, help to add filter step
        public const string FILTER_KEYWORD = "filter.keyword";

        // Chart
        public const string REAL_TIME_KEY = "{{REAL_TIME}}";
        public const string FILTER_KEY = "{{FILTER}}";

        // Dynamic List
        public const string SEARCH_KEY = "{{SEARCH}}";
        public const string FILTER_LIST_KEY = "{{FILTER}}";
        public const string ORDER_KEY = "{{ORDER}}";
        public const string CURRENT_PAGE_KEY = "{{PAGECURRENT}}";
        public const string PAGE_START_KEY = "{{PAGESTART}}";
        public const string PAGE_NUM_KEY = "{{PAGENUM}}";

        public const string TRUE_COMPARE_KEY = "1=1";
    }
}
