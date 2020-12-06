using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        private readonly IOptionsMonitor<MongoOptions> _mongoOptions;

        public MongoDynamicListQueryDatabase(IOptionsMonitor<MongoOptions> options)
        {
            _mongoOptions = options;
        }

        public async Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            dynamicList.GenerateFilters();

            var dynamicListResponseDataModel = new DynamicListResponseDataModel();

            // Because this flow is very complicated. We MUST UPDATE this flow fequently
            // 1. Extract collection name for executing
            // 2. Prepare execution query with some parts:
            // 2.1 Params
            // 2.2 Combine with Filters
            // 2.3 Sort
            // 3. Execute query with pagination
            // 4. Response data
            // 5. Note: in case entity, we still use the same way with EntityName field

            // 1. Extract collection name for executing, either Entity mode or Custom mode, EntityName is the same

            var executingQuery = dynamicList.ListDatasource.DatabaseConnectionOptions.Query;

            var parsingObject = JObject.Parse(executingQuery);
            var executingCollectionName = (parsingObject[Constants.QUERY_KEY].First as JProperty).Name;

            // 2.  Prepare execution query
            // 2.2 Combine with Text Search, Filters and Sort           
            var mongoCollection = new MongoClient(databaseConnection.ConnectionString).GetDatabase(databaseConnection.DataSource).GetCollection<BsonDocument>(executingCollectionName);
            var filterDefinitionOptions = FilterDefinition<BsonDocument>.Empty;

            var collectionQuery = parsingObject[Constants.QUERY_KEY][executingCollectionName].ToString(Newtonsoft.Json.Formatting.Indented);

            foreach (var filledParam in fetchDataModel.FilledParameterOptions.FilledParameters)
            {
                collectionQuery = collectionQuery.Replace("{{" + filledParam.Name + "}}", filledParam.Value);
            }

            collectionQuery = _mongoOptions.CurrentValue.EliminateDoubleQuotes(collectionQuery);
            var aggregatePipes = BsonSerializer.Deserialize<BsonDocument[]>(collectionQuery).Select(a => (PipelineStageDefinition<BsonDocument, BsonDocument>)a).ToList();

            var aggregateFluent = mongoCollection.Aggregate();
            foreach (var pipe in aggregatePipes)
            {
                aggregateFluent = aggregateFluent.AppendStage(pipe);
            }
            // Add Text search first if had
            if (!string.IsNullOrEmpty(fetchDataModel.TextSearch))
            {
                aggregateFluent = aggregateFluent.Match(CombineTextSearch(fetchDataModel.TextSearch, dynamicList.FiltersList));
            }

            // Add Filter Options if had
            if (fetchDataModel.FilterGroupOptions != null
                && fetchDataModel.FilterGroupOptions.FilterGroups != null
                    && fetchDataModel.FilterGroupOptions.FilterGroups.Count > 0
                        && (fetchDataModel.FilterGroupOptions.FilterGroups[0].FilterOptions.Count > 0))
            {
                aggregateFluent = aggregateFluent.AppendStage(PipelineStageDefinitionBuilder.Match(BuildFilters(fetchDataModel.FilterGroupOptions.FilterGroups)));
            }

            // Projection only columns
            var projectDoc = new BsonDocument();
            foreach (var column in dynamicList.ColumnsList.ColumnDefs)
            {
                projectDoc.Add(new BsonElement(column.Name, 1));
            }
            var projection = new BsonDocument
            {
                { "$project", projectDoc }
            };
            aggregateFluent = aggregateFluent.AppendStage((PipelineStageDefinition<BsonDocument, BsonDocument>)projection);

            // Add Sort if had
            if (fetchDataModel.SortOptions.SortableFields != null && fetchDataModel.SortOptions.SortableFields.Count > 0)
            {
                var sortField = fetchDataModel.SortOptions.SortableFields[0];
                FieldDefinition<BsonDocument, string> field = sortField.FieldName;
                var sortDefinition = sortField.SortType == SortType.Asc
                                                                ? Builders<BsonDocument>.Sort.Ascending(field) :
                                                                    Builders<BsonDocument>.Sort.Descending(field);
                aggregateFluent = aggregateFluent.AppendStage(PipelineStageDefinitionBuilder.Sort(sortDefinition));
            }

            if (fetchDataModel.PaginationOptions.NeedTotalItems)
            {
                var aggregateFluentForCountTotal = aggregateFluent.Count();
                var totalItems = await aggregateFluentForCountTotal.FirstOrDefaultAsync();
                dynamicListResponseDataModel.TotalItems = totalItems != null ? totalItems.Count : 0;
            }

            if (fetchDataModel.PaginationOptions.NeedTotalItems && dynamicListResponseDataModel.TotalItems > 0)
            {
                // Add Pagination
                aggregateFluent = aggregateFluent
                    .Skip(fetchDataModel.PaginationOptions.PageNumber * fetchDataModel.PaginationOptions.PageSize)
                    .Limit(fetchDataModel.PaginationOptions.PageSize);

                using (var executingCursor = await aggregateFluent.ToCursorAsync())
                {
                    while (executingCursor.MoveNext())
                    {
                        var currentBson = executingCursor.Current;
                        foreach (var item in currentBson)
                        {
                            var addedFields = new List<BsonElement>();
                            var removedFields = new List<string>();
                            foreach (var elem in item)
                            {
                                if (elem.Value.IsObjectId)
                                {
                                    addedFields.Add(new BsonElement(elem.Name == "_id" ? "id" : elem.Name, BsonValue.Create(elem.Value.AsObjectId.ToString())));
                                    removedFields.Add(elem.Name);
                                }
                            }

                            foreach (var removedField in removedFields)
                            {
                                item.Remove(removedField);
                            }

                            foreach (var addedField in addedFields)
                            {
                                item.Add(addedField);
                            }
                        }

                        // Important note: this query must have one row result for extracting params and filters
                        dynamicListResponseDataModel.Data =
                            currentBson
                                .Select(a =>
                                    a.ToJson(
                                        new MongoDB.Bson.IO.JsonWriterSettings
                                        {
                                            OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict
                                        })).Select(b =>
                                            JsonConvert.DeserializeObject<dynamic>(b, new BsonConverter(GetFormatFields(dynamicList.ColumnsList.ColumnDefs)))).ToList();
                    }
                }
            }
            else if (!fetchDataModel.PaginationOptions.NeedTotalItems)
            {
                // Add Pagination
                aggregateFluent = aggregateFluent
                    .Skip(fetchDataModel.PaginationOptions.PageNumber * fetchDataModel.PaginationOptions.PageSize)
                    .Limit(fetchDataModel.PaginationOptions.PageSize);

                using (var executingCursor = await aggregateFluent.ToCursorAsync())
                {
                    while (executingCursor.MoveNext())
                    {
                        var currentBson = executingCursor.Current;
                        foreach (var item in currentBson)
                        {
                            var addedFields = new List<BsonElement>();
                            var removedFields = new List<string>();
                            foreach (var elem in item)
                            {
                                if (elem.Value.IsObjectId)
                                {
                                    addedFields.Add(new BsonElement(elem.Name == "_id" ? "id" : elem.Name, BsonValue.Create(elem.Value.AsObjectId.ToString())));
                                    removedFields.Add(elem.Name);
                                }
                            }

                            foreach (var removedField in removedFields)
                            {
                                item.Remove(removedField);
                            }

                            foreach (var addedField in addedFields)
                            {
                                item.Add(addedField);
                            }
                        }

                        // Important note: this query must have one row result for extracting params and filters
                        dynamicListResponseDataModel.Data =
                            currentBson
                                .Select(a =>
                                    a.ToJson(
                                        new MongoDB.Bson.IO.JsonWriterSettings
                                        {
                                            OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict
                                        })).Select(b =>
                                            JsonConvert.DeserializeObject<dynamic>(b, new BsonConverter(GetFormatFields(dynamicList.ColumnsList.ColumnDefs)))).ToList();
                    }
                }
            }

            return dynamicListResponseDataModel;
        }

        private List<FormatBsonField> GetFormatFields(List<ColumnDef> columndDefs)
        {
            return columndDefs.Where(a => !string.IsNullOrEmpty(a.DisplayFormat)).Select(b => new FormatBsonField { BsonFieldName = b.Name, Format = b.DisplayFormat }).ToList();
        }

        private FilterDefinition<BsonDocument> CombineTextSearch(string textSearch, FiltersList filtersList)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filtersDefinitionList = new List<FilterDefinition<BsonDocument>>();

            foreach (var filter in filtersList.FilterFields)
            {
                if (filter.AllowTextSearch)
                {
                    filtersDefinitionList.Add(builder.Regex(filter.Name, new BsonRegularExpression(Regex.Escape(textSearch), "i")));
                }
            }

            if (filtersDefinitionList.Count > 0)
            {
                return builder.Or(filtersDefinitionList);
            }

            return FilterDefinition<BsonDocument>.Empty;
        }

        private FilterDefinition<BsonDocument> BuildFilters(List<FilterGroup> filterGroups)
        {
            var executingFilterDefinition = FilterDefinition<BsonDocument>.Empty;
            foreach (var filterGroup in filterGroups ?? Enumerable.Empty<FilterGroup>())
            {
                var filterGroupBuilder = Builders<BsonDocument>.Filter;

                var isInCombiningAnd = false;
                var needToGroupOr = false;
                var andBuilder = Builders<BsonDocument>.Filter;
                var groupAndFilter = new List<FilterDefinition<BsonDocument>>();

                var isInCombiningOr = false;
                var orBuilder = Builders<BsonDocument>.Filter;
                var groupOrFilter = new List<FilterDefinition<BsonDocument>>();

                foreach (var filter in filterGroup.FilterOptions ?? Enumerable.Empty<FilterOption>())
                {
                    if (filter.FilterChainOperator == FilterChainOperator.None)
                    {
                        if (isInCombiningAnd)
                        {
                            if (needToGroupOr)
                            {
                                if (groupAndFilter.Count > 0)
                                {
                                    groupOrFilter.Add(andBuilder.And(groupAndFilter));
                                }
                                groupOrFilter.Add(andBuilder.And(groupAndFilter));
                                executingFilterDefinition = filterGroupBuilder.Or(groupOrFilter);
                                needToGroupOr = false;
                                isInCombiningAnd = false;
                            }
                            else
                            {
                                groupAndFilter.Add(BuildOperator(filter));
                                executingFilterDefinition = filterGroupBuilder.And(groupAndFilter);
                                isInCombiningAnd = false;
                            }
                        }
                        else if (isInCombiningOr)
                        {
                            if (needToGroupOr)
                            {
                                if (groupAndFilter.Count > 0)
                                {
                                    groupOrFilter.Add(andBuilder.And(groupAndFilter));
                                }
                                groupOrFilter.Add(BuildOperator(filter));
                                executingFilterDefinition = filterGroupBuilder.Or(groupOrFilter);
                                needToGroupOr = false;
                                isInCombiningOr = false;
                            }
                        }
                        else
                        {
                            executingFilterDefinition = BuildOperator(filter);
                        }
                    }
                    else if (filter.FilterChainOperator == FilterChainOperator.And)
                    {
                        if (isInCombiningOr)
                        {
                            groupAndFilter.Add(BuildOperator(filter));
                            isInCombiningOr = false;
                            needToGroupOr = true;
                        }
                        else
                        {
                            groupAndFilter.Add(BuildOperator(filter));
                        }
                        isInCombiningAnd = true;
                    }
                    else if (filter.FilterChainOperator == FilterChainOperator.Or)
                    {
                        if (isInCombiningAnd)
                        {
                            groupAndFilter.Add(BuildOperator(filter));
                            isInCombiningAnd = false;
                            needToGroupOr = true;

                            groupOrFilter.Add(andBuilder.And(groupAndFilter));
                            groupAndFilter = new List<FilterDefinition<BsonDocument>>();
                        }
                        else
                        {
                            groupOrFilter.Add(BuildOperator(filter));
                        }

                        isInCombiningOr = true;
                    }
                }
            }

            return executingFilterDefinition;
        }

        private FilterDefinition<BsonDocument> BuildOperator(FilterOption filterOption)
        {
            var filterBuilderOption = Builders<BsonDocument>.Filter;
            FieldDefinition<BsonDocument, string> field = filterOption.FieldName;
            switch (filterOption.FilterValueType)
            {
                case FieldValueType.Text:
                    if (filterOption.FilterOperator == FilterOperator.Contains)
                    {
                        return filterBuilderOption.Regex(field, new BsonRegularExpression(filterOption.FieldValue, "i"));
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Equal)
                    {
                        return filterBuilderOption.Eq(field, filterOption.FieldValue);
                    }
                    break;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    return filterBuilderOption.Eq(filterOption.FieldName, new BsonBoolean(bool.Parse(filterOption.FieldValue)));
                case FieldValueType.Select:
                    if (filterOption.FieldValue is bool)
                    {
                        return filterBuilderOption.Eq(filterOption.FieldName, new BsonBoolean(filterOption.FieldValue));
                    }
                    else if (filterOption.FieldValue is long)
                    {
                        return filterBuilderOption.Eq(filterOption.FieldName, new BsonInt64(filterOption.FieldValue));
                    }
                    else if (filterOption.FieldValue is decimal)
                    {
                        return filterBuilderOption.Eq(filterOption.FieldName, new BsonDecimal128(filterOption.FieldValue));
                    }
                    else
                    {
                        return filterBuilderOption.Eq(field, filterOption.FieldValue);
                    }
                    
                case FieldValueType.DatePicker:
                    var datetime = DateTime.Parse(filterOption.FieldValue);
                    var month = datetime.Month;
                    var day = datetime.Day;
                    var year = datetime.Year;
                    if (filterOption.FilterOperator == FilterOperator.Equal)
                    {
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterDateGt = new DateTime(year, month, day);
                        var filterDateLt = new DateTime(year, month, day + 1);
                        filterDateLt = filterDateLt.Subtract(TimeSpan.FromSeconds(1));
                        return filterBuilderOption.And(
                            filterBuilderOption.Gte(filterOption.FieldName, filterDateGt),
                            filterBuilderOption.Lte(filterOption.FieldName, filterDateLt));
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Great)
                    {
                        var filterDate = new DateTime(year, month, day);
                        return filterBuilderOption.Gt(filterOption.FieldName, filterDate);
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Less)
                    {
                        var filterDate = new DateTime(year, month, day);
                        return filterBuilderOption.Lt(filterOption.FieldName, filterDate);
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Greater)
                    {
                        var filterDate = new DateTime(year, month, day);
                        return filterBuilderOption.Gte(filterOption.FieldName, filterDate);
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Lesser)
                    {
                        var filterDate = new DateTime(year, month, day);
                        return filterBuilderOption.Lte(filterOption.FieldName, filterDate);
                    }
                    break;
                case FieldValueType.Number:
                    if (filterOption.FilterOperator == FilterOperator.Equal)
                    {
                        return filterBuilderOption.Eq(filterOption.FieldName, new BsonInt64(long.Parse(filterOption.FieldValue)));
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Great)
                    {
                        return filterBuilderOption.Gt(filterOption.FieldName, new BsonInt64(long.Parse(filterOption.FieldValue)));
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Greater)
                    {
                        return filterBuilderOption.Gte(filterOption.FieldName, new BsonInt64(long.Parse(filterOption.FieldValue)));
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Less)
                    {
                        return filterBuilderOption.Lt(filterOption.FieldName, new BsonInt64(long.Parse(filterOption.FieldValue)));
                    }
                    else if (filterOption.FilterOperator == FilterOperator.Lesser)
                    {
                        return filterBuilderOption.Lte(filterOption.FieldName, new BsonInt64(long.Parse(filterOption.FieldValue)));
                    }
                    break;
            }
            return null;
        }
    }
}
