using LetPortal.Core.Utils;
using LetPortal.Portal.Models.DynamicLists;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LetPortal.Portal.Executions
{
    public class DynamicQueryBuilder : IDynamicQueryBuilder
    {
        private DynamicQueryBuilderOptions builderOptions;

        private DynamicQuery dynamicQuery;

        private string filterString;

        private string searchString;

        private string orderString;

        private string paginationString;

        public DynamicQueryBuilder()
        {
            builderOptions = new DynamicQueryBuilderOptions();
            dynamicQuery = new DynamicQuery
            {
                Parameters = new List<DynamicQueryParameter>()
            };
        }

        public IDynamicQueryBuilder Init(string query, List<FilledParameter> parameters, Action<DynamicQueryBuilderOptions> action = null)
        {
            if(action != null)
            {
                action.Invoke(builderOptions);
            }

            dynamicQuery.CombinedQuery = query;
            foreach(var param in parameters)
            {
                var fieldParam = StringUtil.GenerateUniqueName();
                dynamicQuery.CombinedQuery = dynamicQuery.CombinedQuery.Replace("{{" + param.Name + "}}", builderOptions.ParamSign + fieldParam);
                dynamicQuery.Parameters.Add(new DynamicQueryParameter
                {
                    Name = fieldParam,
                    Value = param.Value,
                    ValueType = Entities.SectionParts.FieldValueType.Text
                });
            }

            searchString = builderOptions.DefaultSearchNull;
            filterString = builderOptions.DefaultFilterNull;
            orderString = builderOptions.DefaultOrderNull;
            return this;
        }

        public IDynamicQueryBuilder AddFilter(List<FilterGroup> groups)
        {
            if(groups != null && groups.Count > 0)
            {
                filterString = null;
                foreach(var group in groups)
                {
                    foreach(var filter in group.FilterOptions)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        if(filter.FilterOperator != FilterOperator.Contains)
                        {
                            filterString += string.Format(builderOptions.FieldFormat, filter.FieldName) + GetOperator(filter.FilterOperator) + builderOptions.ParamSign + fieldParam + GetChainOperator(filter.FilterChainOperator) + " ";
                        }
                        else
                        {
                            filterString += string.Format(builderOptions.FieldFormat, filter.FieldName) + GetOperator(filter.FilterOperator, fieldParam)  + GetChainOperator(filter.FilterChainOperator) + " ";
                        }                        

                        dynamicQuery.Parameters.Add(new DynamicQueryParameter
                        {
                            Name = fieldParam,
                            Value = filter.FieldValue,
                            ValueType = filter.FilterValueType
                        });
                    }
                }
            }
            return this;
        }

        public IDynamicQueryBuilder AddSort(List<SortableField> sorts)
        {
            if(sorts != null && sorts.Count > 0)
            {
                orderString = null;
                foreach(var sort in sorts)
                {
                    orderString += string.Format(builderOptions.FieldFormat, sort.FieldName) + " " + (sort.SortType == SortType.Asc ? "asc" : "desc");
                }
            }
            return this;
        }

        public IDynamicQueryBuilder AddTextSearch(string text, IEnumerable<string> searchFields)
        {
            if(!string.IsNullOrEmpty(text))
            {
                searchString = null;
                int i = 0;
                foreach(var field in searchFields)
                {
                    var fieldParam = StringUtil.GenerateUniqueName();
                    searchString += string.Format(builderOptions.FieldFormat, field) + string.Format(builderOptions.ContainsOperatorFormat, builderOptions.ParamSign + fieldParam);
                    dynamicQuery.Parameters.Add(new DynamicQueryParameter
                    {
                        Name = fieldParam,
                        Value = text,
                        ValueType = Entities.SectionParts.FieldValueType.Text
                    });
                    i++;
                    if(searchFields.Count() > i)
                    {
                        searchString += " OR ";
                    }
                }
            }
            return this;
        }

        public IDynamicQueryBuilder AddPagination(int currentPage, int numberPerPage)
        {
            paginationString += string.Format(builderOptions.PaginationFormat, numberPerPage, currentPage * numberPerPage);

            return this;
        }

        public DynamicQuery Build()
        {
            string warpQuery;
            // 1. Check a query contains WHERE clause, if not, add search, filter, order and pagination
            var whereIndex = dynamicQuery.CombinedQuery.ToUpper().IndexOf(" WHERE ");
            var closeTagIndex = dynamicQuery.CombinedQuery.IndexOf(")");
            if(whereIndex < 0)
            {
                warpQuery = @"{0} WHERE (({1}) AND ({2})) Order By {3} {4}";
            }
            else if(whereIndex > 0 && whereIndex > closeTagIndex)
            {
                warpQuery = @"{0} AND (({1}) AND ({2})) Order By {3} {4}";
            }
            else
            {
                warpQuery = @"Select * From ({0}) s Where (({1}) AND ({2})) Order By {3} {4}";
            }

            if(string.IsNullOrEmpty(paginationString))
            {
                paginationString = string.Format(builderOptions.PaginationFormat, builderOptions.DefaultNumberPage, builderOptions.DefaultCurrentPage);
            }
            dynamicQuery.CombinedQuery =
                   string.Format(
                       warpQuery,
                       dynamicQuery.CombinedQuery,
                       searchString,
                       filterString,
                       orderString,
                       paginationString);
            return dynamicQuery;
        }

        private string GetOperator(FilterOperator filterOperator, string comparision = null)
        {
            switch(filterOperator)
            {
                case FilterOperator.Contains:
                    return string.Format(builderOptions.ContainsOperatorFormat, builderOptions.ParamSign +  comparision);
                case FilterOperator.Great:
                    return ">";
                case FilterOperator.Greater:
                    return ">=";
                case FilterOperator.Less:
                    return "<";
                case FilterOperator.Lesser:
                    return "<=";
                case FilterOperator.Equal:
                default:
                    return "=";
            }
        }

        private string GetChainOperator(FilterChainOperator filterChainOperator)
        {
            switch(filterChainOperator)
            {
                case FilterChainOperator.And:
                    return "AND";
                case FilterChainOperator.Or:
                    return "OR";
                default:
                    return string.Empty;
            }
        }
    }
}
