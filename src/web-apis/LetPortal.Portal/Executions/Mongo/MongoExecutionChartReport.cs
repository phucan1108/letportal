using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Models.Charts;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

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

            var filterStages = new List<PipelineStageDefinition<BsonDocument, BsonDocument>>();

            if (model.IsRealTime && !string.IsNullOrEmpty(model.ComparedRealTimeField))
            {
                if (model.LastComparedDate.HasValue)
                {
                    var andDoc = new BsonDocument
                                {
                                    { "$and", new BsonArray(new List<BsonDocument>
                                        {
                                        new BsonDocument
                                        {
                                           { model.ComparedRealTimeField, new BsonDocument
                                            {
                                               { "$gt", new BsonDateTime(model.LastComparedDate.Value) }
                                            }
                                           }
                                       },
                                       new BsonDocument
                                       {
                                           { model.ComparedRealTimeField, new BsonDocument
                                            {
                                               { "$lte", new BsonDateTime(DateTime.UtcNow) }
                                            }
                                           }
                                       }
                                        })
                                    }
                                };
                    filterStages.Add(new BsonDocument
                    {
                        { "$match", andDoc }
                    });
                }
                else
                {
                    var matchDoc = new BsonDocument
                    {
                      {
                          "$match",
                          new BsonDocument
                          {
                              {  model.ComparedRealTimeField,
                                    new BsonDocument
                                    {
                                        { "$lte", new BsonDateTime(DateTime.UtcNow) }
                                    }
                              }
                          }
                      }
                    };
                    filterStages.Add(matchDoc);
                }
            }

            if (model.FilterValues != null && model.FilterValues.Any())
            {
                foreach (var filterValue in model.FilterValues)
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
            bool isSuccess = result != null;
            if (isSuccess)
            {
                var tempResult = result as IEnumerable<dynamic>;
                if (tempResult.Any())
                {
                    var first = tempResult.First();
                    var hasGroup = false;
                    if (first is ExpandoObject)
                    {
                        hasGroup = ((IDictionary<string, object>)first).ContainsKey("group");
                    }
                    else
                    {
                        hasGroup = (first as JObject).ContainsKey("group");
                    }
                    if (hasGroup)
                    {
                        result = tempResult.GroupBy(a => a.group).Where(a => a != null).Select(a => new { series = a.Select(b => new { b.name, b.value }), name = a.Key });
                    }
                }
            }

            return new ExecutionChartResponseModel { IsSuccess = isSuccess, Result = result };
        }

        private BsonDocument GetFilter(ChartFilterValue chartFilterValue)
        {
            switch (chartFilterValue.FilterType)
            {
                case Entities.Components.FilterType.Checkbox:
                    var checkboxDoc = new BsonDocument();
                    checkboxDoc.Add(new BsonElement(chartFilterValue.Name, new BsonBoolean(bool.Parse(chartFilterValue.Value))));
                    return new BsonDocument
                    {
                        {  "$match",checkboxDoc }
                    };
                case Entities.Components.FilterType.Select:
                    var selectDoc = new BsonDocument();
                    if (decimal.TryParse(chartFilterValue.Value, out var tempDecimal))
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonDecimal128(tempDecimal)));
                    }
                    else if (long.TryParse(chartFilterValue.Value, out var tempLong))
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt64(tempLong)));
                    }
                    else if (int.TryParse(chartFilterValue.Value, out var tempInt))
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt32(tempInt)));
                    }
                    else
                    {
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonString(chartFilterValue.Value)));
                        selectDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt32(tempInt)));
                    }
                    return new BsonDocument
                    {
                        { "$match", selectDoc }
                    };
                case Entities.Components.FilterType.NumberPicker:
                    if (chartFilterValue.IsMultiple)
                    {
                        var orDoc = new BsonArray();
                        var selectMultiDoc = new BsonDocument();
                        var isArrayStr = chartFilterValue.Value.Any(a => a == '\'' || a == '"');
                        if (isArrayStr)
                        {
                            var arrayStr = ConvertUtil.DeserializeObject<List<string>>(chartFilterValue.Value);
                            var containsNumberMinMax = chartFilterValue.Value.Contains("-");
                            foreach (var elem in arrayStr)
                            {
                                var splitted = elem.Split("-");
                                var beginNum = long.Parse(splitted[0]);
                                var endNum = long.Parse(splitted[1]);
                                var andDoc = new BsonDocument
                                {
                                    { "$and", new BsonArray(new List<BsonDocument>
                                        {
                                        new BsonDocument
                                        {
                                           { chartFilterValue.Name, new BsonDocument
                                            {
                                               { "$gte", new BsonInt64(beginNum) }
                                            }
                                           }
                                       },
                                       new BsonDocument
                                       {
                                           { chartFilterValue.Name, new BsonDocument
                                            {
                                               { "$lte", new BsonInt64(endNum) }
                                            }
                                           }
                                       }
                                        })
                                    }
                                };

                                orDoc.Add(andDoc);
                            }

                            selectMultiDoc = new BsonDocument
                            {
                                { "$or", orDoc }
                            };
                        }
                        else
                        {
                            var arrayStr = ConvertUtil.DeserializeObject<List<long>>(chartFilterValue.Value);
                            foreach (var elem in arrayStr)
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
                        var containsNumberMinMax = chartFilterValue.Value.Contains("-");
                        if (containsNumberMinMax)
                        {
                            // Ex: 10-30, 30-50
                            var splitted = chartFilterValue.Value.Split("-");
                            var beginNum = long.Parse(splitted[0]);
                            var endNum = long.Parse(splitted[1]);
                            var andDoc = new BsonDocument
                            {
                                { "$and", new BsonArray(new List<BsonDocument>
                                    {
                                       new BsonDocument
                                       {
                                           { chartFilterValue.Name, new BsonDocument
                                            {
                                               { "$gte", new BsonInt64(beginNum) }
                                            }
                                           }
                                       },
                                       new BsonDocument
                                       {
                                           { chartFilterValue.Name, new BsonDocument
                                            {
                                               { "$lte", new BsonInt64(endNum) }
                                            }
                                           }
                                       }
                                    })
                                }
                            };

                            return new BsonDocument
                            {
                                { "$match", andDoc }
                            };
                        }
                        else
                        {
                            var numberDoc = new BsonDocument();
                            if (long.TryParse(chartFilterValue.Value, out var tempLong))
                            {
                                numberDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt64(tempLong)));
                            }
                            else if (int.TryParse(chartFilterValue.Value, out var tempInt))
                            {
                                numberDoc.Add(new BsonElement(chartFilterValue.Name, new BsonInt32(tempInt)));
                            }

                            return new BsonDocument
                            {
                                { "$match", numberDoc }
                            };
                        }
                    }
                case Entities.Components.FilterType.DatePicker:
                    if (chartFilterValue.IsMultiple)
                    {
                        var arrayDates = ConvertUtil.DeserializeObject<string[]>(chartFilterValue.Value);
                        var startDatePickerDt = DateTime.Parse(arrayDates[0]);
                        var endDatePickerDt = DateTime.Parse(arrayDates[1]);
                        var month = startDatePickerDt.Month;
                        var day = startDatePickerDt.Day;
                        var year = startDatePickerDt.Year;
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterStartDate = new DateTime(year, month, day);

                        var monthEnd = endDatePickerDt.Month;
                        var dayEnd = endDatePickerDt.Day;
                        var yearEnd = endDatePickerDt.Year;
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterEndDate = new DateTime(yearEnd, monthEnd, dayEnd);

                        var gteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name,  new BsonDocument(new BsonElement("$gte", new BsonDateTime(filterStartDate)))}
                        };
                        var lteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name,  new BsonDocument(new BsonElement("$lte", new BsonDateTime(filterEndDate)))}
                        };

                        var andDoc = new BsonDocument
                        {
                            {
                                "$and",
                                new BsonArray(new List<BsonDocument>{ gteDoc, lteDoc})
                            }
                        };

                        return new BsonDocument
                        {
                            { "$match", andDoc }
                        };
                    }
                    else
                    {
                        var datetime = DateTime.Parse(chartFilterValue.Value);
                        var month = datetime.Month;
                        var day = datetime.Day;
                        var year = datetime.Year;
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterDateGt = new DateTime(year, month, day);
                        var filterDateLt = new DateTime(year, month, day + 1);
                        filterDateLt = filterDateLt.Subtract(TimeSpan.FromSeconds(1));
                        var gteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name,  new BsonDocument(new BsonElement("$gte", new BsonDateTime(filterDateGt)))}
                        };
                        var lteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name ,  new BsonDocument(new BsonElement("$lte", new BsonDateTime(filterDateLt)))}
                        };

                        var andDoc = new BsonDocument
                        {
                            {
                                "$and",
                                new BsonArray(new List<BsonDocument>{ gteDoc, lteDoc})
                            }
                        };

                        return new BsonDocument
                        {
                            { "$match", andDoc }
                        };
                    }
                case Entities.Components.FilterType.MonthYearPicker:
                    if (chartFilterValue.IsMultiple)
                    {
                        var arrayDates = ConvertUtil.DeserializeObject<string[]>(chartFilterValue.Value);
                        var startDatePickerDt = DateTime.Parse(arrayDates[0]);
                        var endDatePickerDt = DateTime.Parse(arrayDates[1]);
                        var month = startDatePickerDt.Month;
                        var day = startDatePickerDt.Day;
                        var year = startDatePickerDt.Year;
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterStartDate = new DateTime(year, month, 1);

                        var monthEnd = endDatePickerDt.Month;
                        var dayEnd = endDatePickerDt.Day;
                        var yearEnd = endDatePickerDt.Year;
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterEndDate = new DateTime(yearEnd, monthEnd + 1, 1);
                        filterEndDate = filterEndDate.Subtract(TimeSpan.FromDays(1));
                        var gteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name,  new BsonDocument(new BsonElement("$gte", new BsonDateTime(filterStartDate)))}
                        };
                        var lteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name,  new BsonDocument(new BsonElement("$lte", new BsonDateTime(filterEndDate)))}
                        };

                        var andDoc = new BsonDocument
                        {
                            {
                                "$and",
                                new BsonArray(new List<BsonDocument>{ gteDoc, lteDoc})
                            }
                        };

                        return new BsonDocument
                        {
                            { "$match", andDoc }
                        };
                    }
                    else
                    {
                        var datetime = DateTime.Parse(chartFilterValue.Value);
                        var month = datetime.Month;
                        var day = datetime.Day;
                        var year = datetime.Year;
                        // Support Format MM/DD/YYYY, we can change its in configuration later
                        var filterDateGt = new DateTime(year, month + 1, 1);
                        var filterDateLt = new DateTime(year, month + 1, 1);
                        filterDateLt = filterDateLt.Subtract(TimeSpan.FromDays(1));
                        var gteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name ,  new BsonDocument(new BsonElement("$gte", new BsonDateTime(filterDateGt)))}
                        };
                        var lteDoc = new BsonDocument
                        {
                            { chartFilterValue.Name ,  new BsonDocument(new BsonElement("$lte", new BsonDateTime(filterDateLt)))}
                        };

                        var andDoc = new BsonDocument
                        {
                            {
                                "$and",
                                new BsonArray(new List<BsonDocument>{ gteDoc, lteDoc})
                            }
                        };

                        return new BsonDocument
                        {
                            { "$match", andDoc }
                        };
                    }
                default:
                    return null;
            }
        }
    }
}
