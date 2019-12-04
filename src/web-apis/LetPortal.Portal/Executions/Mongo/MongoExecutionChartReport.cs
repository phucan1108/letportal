using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Models.Charts;
using LetPortal.Portal.Repositories.Components;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.Mongo
{
    public class MongoExecutionChartReport : IExecutionChartReport
    {
        private readonly IMongoQueryExecution _queryExecution;

        public ConnectionType ConnectionType => ConnectionType.MongoDB;

        public MongoExecutionChartReport(
            IMongoQueryExecution queryExecution)
        {
            _queryExecution = queryExecution;
        }

        public async Task<ExecutionChartResponseModel> Execute(ExecutionChartReportModel model)
        {
            var mongoDatabase = new MongoClient(model.DatabaseConnection.ConnectionString).GetDatabase(model.DatabaseConnection.DataSource);

            List<PipelineStageDefinition<BsonDocument, BsonDocument>> filterStages = new List<PipelineStageDefinition<BsonDocument, BsonDocument>>();
            if(model.FilterValues != null && model.FilterValues.Any())
            {
                foreach(var filterValue in model.FilterValues)
                {
                    filterStages.Add(GetFilter(filterValue));
                }
            }

            var result = await _queryExecution.ExecuteAsync(
                    mongoDatabase, 
                    model.FormattedString,
                    model.MappingProjection,
                    model.Parameters?.Select(a => new Models.Databases.ExecuteParamModel { Name = a.Name, RemoveQuotes = a.ReplaceDQuotes, ReplaceValue = a.Value }),
                    filterStages);

            return new ExecutionChartResponseModel { IsSuccess = result != null, Result = result };
        }

        private BsonDocument GetFilter(ChartFilterValue chartFilterValue)
        {
            switch(chartFilterValue.FilterType)
            {
                case Entities.Components.FilterType.Checkbox:
                    BsonDocument checkboxDoc = new BsonDocument();
                    checkboxDoc.Add(new BsonElement(chartFilterValue.Name, new BsonBoolean(bool.Parse(chartFilterValue.Value))));
                    return new BsonDocument
                    {
                        {  "$match",checkboxDoc }
                    };
                case Entities.Components.FilterType.Select:
                    BsonDocument selectDoc = new BsonDocument();
                    if(decimal.TryParse(chartFilterValue.Value, out decimal tempDecimal))
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonDecimal128(tempDecimal)));
                    }
                    else if(long.TryParse(chartFilterValue.Value, out long tempLong))
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt64(tempLong)));
                    }
                    else if(int.TryParse(chartFilterValue.Value, out int tempInt))
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt32(tempInt)));
                    }
                    else
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonString(chartFilterValue.Value)));                                                                                 selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt32(tempInt)));
                    }
                    return new BsonDocument
                    {
                        { "$match", selectDoc }
                    };
                case Entities.Components.FilterType.NumberPicker:
                    if(chartFilterValue.IsMultiple)
                    {
                        BsonArray orDoc = new BsonArray();
                        BsonDocument selectMultiDoc = new BsonDocument();
                        bool isArrayStr = chartFilterValue.Value.Any(a => a == '\'' || a == '"');
                        if(isArrayStr)
                        {
                            var arrayStr = ConvertUtil.DeserializeObject<List<string>>(chartFilterValue.Value);
                            foreach(var elem in arrayStr)
                            {
                                orDoc.Add(new BsonDocument
                                {
                                    new BsonElement(chartFilterValue.Name, new BsonString(elem))
                                });
                            }

                            selectMultiDoc = new BsonDocument
                            {
                                { "$or", orDoc }
                            };
                        }
                        else
                        {
                            var arrayStr = ConvertUtil.DeserializeObject<List<long>>(chartFilterValue.Value);
                            foreach(var elem in arrayStr)
                            {
                                orDoc.Add(new BsonDocument
                                {
                                    new BsonElement(chartFilterValue.Name, new BsonInt64(elem))
                                });
                            };

                            selectMultiDoc = new BsonDocument
                            {
                                { "$or", orDoc }
                            };
                        }

                        return new BsonDocument
                        {
                            { "$match", selectMultiDoc }
                        };
                    }
                    else
                    {
                        BsonDocument numberDoc = new BsonDocument();
                        if(long.TryParse(chartFilterValue.Value, out long tempLong))
                        {
                            numberDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt64(tempLong)));
                        }
                        else if(int.TryParse(chartFilterValue.Value, out int tempInt))
                        {
                            numberDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt32(tempInt)));
                        }

                        return new BsonDocument
                        {
                            { "$match", numberDoc }
                        };
                    }                
                default:
                    return null;
            }
        } 
    }
}
