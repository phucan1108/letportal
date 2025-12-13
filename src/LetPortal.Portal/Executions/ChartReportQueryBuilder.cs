using System;
using System.Collections.Generic;
using System.Linq;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions
{
    public class ChartReportQueryBuilder : IChartReportQueryBuilder
    {
        private ChartReportQueryOptions options;

        private string formattedString;

        private string mappingProjection;

        private IEnumerable<ChartFilterValue> filterValues;

        private IEnumerable<ChartParameterValue> parameterValues;

        private Func<string, string, object> mapperFunc;

        private bool allowRealTime;

        private string comparedRealTimeField;

        private DateTime lastComparedDate;

        private DateTime comparedDate;

        public ChartReportQueryBuilder()
        {
            options = new ChartReportQueryOptions();
        }

        public IChartReportQueryBuilder Init(string formattedString, string mappingProjection, Action<ChartReportQueryOptions> options = null)
        {
            if (options != null)
            {
                options.Invoke(this.options);
            }

            this.formattedString = formattedString;
            this.mappingProjection = mappingProjection;
            return this;
        }

        public IChartReportQueryBuilder AddFilters(IEnumerable<ChartFilterValue> filterValues)
        {
            this.filterValues = filterValues;
            return this;
        }

        public IChartReportQueryBuilder AddParameters(IEnumerable<ChartParameterValue> parameters)
        {
            parameterValues = parameters;
            return this;
        }

        public IChartReportQueryBuilder AddMapper(Func<string, string, object> mapperFunc)
        {
            this.mapperFunc = mapperFunc;
            return this;
        }

        public IChartReportQueryBuilder AddRealTime(string comparedField, DateTime lastComparedDate, DateTime comparedDate)
        {
            allowRealTime = true;
            comparedRealTimeField = comparedField;
            this.lastComparedDate = lastComparedDate;
            this.comparedDate = comparedDate;
            return this;
        }

        public ChartReportQuery Build()
        {
            var listParams = new List<ChartReportParameter>();
            var chartReportQuery = new ChartReportQuery();

            if (parameterValues != null)
            {
                foreach (var param in parameterValues)
                {
                    var fieldParam = StringUtil.GenerateUniqueName();
                    formattedString = formattedString.Replace("{{" + param.Name + "}}", GetFieldWithParamSign(fieldParam));
                    var paramType = GetValueDbType(param.Name, param.Value, out var castObject);
                    listParams.Add(new ChartReportParameter
                    {
                        Name = fieldParam,
                        CastedValue = castObject,
                        ValueType = paramType
                    });
                }
            }

            var warpperString = string.Format("SELECT {0} FROM ({1}) s", GenerateChartColumns(mappingProjection), formattedString);

            if (filterValues != null && filterValues.Any() && warpperString.Contains(options.FilterWord))
            {
                warpperString = warpperString.Replace(options.FilterWord, GenerateFilterColumns(filterValues, ref listParams));
            }
            else
            {
                if (warpperString.Contains(options.FilterWord))
                {
                    warpperString = warpperString.Replace(options.FilterWord, options.DefaultFilterNull);

                }
            }

            if (allowRealTime 
                && !string.IsNullOrEmpty(comparedRealTimeField) 
                && warpperString.Contains(options.RealTimeWord))
            {
                warpperString = warpperString.Replace(options.RealTimeWord, GenerateRealTimeComparision(comparedRealTimeField, lastComparedDate, comparedDate, ref listParams));
            }
            else
            {
                if (warpperString.Contains(options.RealTimeWord))
                {
                    warpperString = warpperString.Replace(options.RealTimeWord, options.DefaultRealTimeNull);
                }
            }

            chartReportQuery.CombinedQuery = warpperString;
            chartReportQuery.DbParameters = listParams;
            return chartReportQuery;
        }

        private string GetFieldFormat(string fieldName)
        {
            return string.Format(options.FieldFormat, fieldName);
        }

        private string GenerateRealTimeComparision(string comparedField, DateTime startDate, DateTime endDate, ref List<ChartReportParameter> parameters)
        {
            var startDateParam = StringUtil.GenerateUniqueName();
            var endDateParam = StringUtil.GenerateUniqueName();
            parameters.Add(new ChartReportParameter
            {
                Name = startDateParam,
                ValueType = "date",
                CastedValue = startDate
            });

            parameters.Add(new ChartReportParameter
            {
                Name = endDateParam,
                ValueType = "date",
                CastedValue = endDate
            });

            return string.Format("({0} AND {1})",
                string.Format("{0}{1}{2}", GetFieldFormat(comparedField), ">", GetFieldWithParamSign(startDateParam)),
                string.Format("{0}{1}{2}", GetFieldFormat(comparedField), "<=", GetFieldWithParamSign(endDateParam)));
        }

        private string GetFieldWithParamSign(string fieldName)
        {
            return options.ParamSign + fieldName;
        }

        private string GenerateChartColumns(string mappingProjection)
        {
            var columnStr = string.Empty;
            var list = mappingProjection.Split(";");
            var counter = 0;
            foreach (var elem in list)
            {
                var splitted = elem.Split("=");
                if (counter == list.Length - 1)
                {
                    columnStr += string.Format("{0} as {1} ", GetFieldFormat(splitted[1]), GetFieldFormat(splitted[0]));
                }
                else
                {
                    columnStr += string.Format("{0} as {1}, ", GetFieldFormat(splitted[1]), GetFieldFormat(splitted[0]));
                }
                counter++;
            }

            return columnStr;
        }

        private string GetValueDbType(string paramName, string value, out object castObj)
        {
            var splitted = paramName.Split("|");
            if (splitted.Length == 1)
            {
                castObj = mapperFunc.Invoke(value, MapperConstants.String);
                return MapperConstants.String;
            }
            else
            {
                castObj = mapperFunc.Invoke(splitted[1], value);
                return splitted[1];
            }
        }

        private string GenerateFilterColumns(IEnumerable<ChartFilterValue> filterValues, ref List<ChartReportParameter> parameters)
        {
            var filterStr = string.Empty;

            var counter = 0;
            foreach (var filter in filterValues)
            {
                switch (filter.FilterType)
                {
                    case Entities.Components.FilterType.Checkbox:
                        var tempCheckBoxParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("{0}={1}", GetFieldFormat(filter.Name), GetFieldWithParamSign(tempCheckBoxParam));

                        // Only for MySQL, bool is 0 or 1
                        var passValue = 0;
                        if (filter.Value == "0" || filter.Value == "1")
                        {
                            passValue = filter.Value == "0" ? 0 : 1;
                        }
                        else
                        {
                            passValue = !bool.Parse(filter.Value) ? 0 : 1;
                        }
                        parameters.Add(new ChartReportParameter
                        {
                            Name = tempCheckBoxParam,
                            CastedValue = passValue == 1,
                            ValueType = "bool"
                        });
                        break;
                    case Entities.Components.FilterType.Select:
                        if (filter.IsMultiple)
                        {
                            var isArrayStr = filter.Value.Any(a => a == '\'' || a == '"');
                            if (isArrayStr)
                            {
                                var arrayStr = ConvertUtil.DeserializeObject<List<string>>(filter.Value);
                                foreach (var str in arrayStr)
                                {
                                    if (arrayStr.IndexOf(str) == 0)
                                    {
                                        filterStr += "(";
                                    }
                                    var tempSelectParam = StringUtil.GenerateUniqueName();
                                    filterStr += string.Format("{0}={1}", GetFieldFormat(filter.Name), GetFieldWithParamSign(tempSelectParam));
                                    if (arrayStr.IndexOf(str) < arrayStr.Count - 1)
                                    {
                                        filterStr += " OR ";
                                    }
                                    else
                                    {
                                        filterStr += ")";
                                    }

                                    parameters.Add(new ChartReportParameter
                                    {
                                        Name = tempSelectParam,
                                        CastedValue = str,
                                        ValueType = "string"
                                    });
                                }
                            }
                            else
                            {
                                var longStr = ConvertUtil.DeserializeObject<List<long>>(filter.Value);
                                foreach (var longElem in longStr)
                                {
                                    if (longStr.IndexOf(longElem) == 0)
                                    {
                                        filterStr += "(";
                                    }
                                    var tempSelectParam = StringUtil.GenerateUniqueName();
                                    filterStr += string.Format("{0}={1}", GetFieldFormat(filter.Name), GetFieldWithParamSign(tempSelectParam));
                                    if (longStr.IndexOf(longElem) < longStr.Count - 1)
                                    {
                                        filterStr += " OR ";
                                    }
                                    else
                                    {
                                        filterStr += ")";
                                    }

                                    parameters.Add(new ChartReportParameter
                                    {
                                        Name = tempSelectParam,
                                        CastedValue = longElem,
                                        ValueType = "long"
                                    });
                                }
                            }
                        }
                        else
                        {
                            var tempSelectParam = StringUtil.GenerateUniqueName();
                            filterStr += string.Format("{0}={1}", GetFieldFormat(filter.Name), GetFieldWithParamSign(tempSelectParam));

                            // Check value is int or string
                            long selectValue = 0;
                            var isLong = false;
                            if (long.TryParse(filter.Value, out selectValue))
                            {
                                isLong = true;
                            }
                            ChartReportParameter selectMySqlParameter;
                            if (isLong)
                            {
                                selectMySqlParameter = new ChartReportParameter
                                {
                                    Name = tempSelectParam,
                                    CastedValue = selectValue,
                                    ValueType = "long"
                                };
                            }
                            else
                            {
                                selectMySqlParameter = new ChartReportParameter
                                {
                                    Name = tempSelectParam,
                                    CastedValue = filter.Value,
                                    ValueType = "string"
                                };
                            }
                            parameters.Add(selectMySqlParameter);
                        }
                        break;
                    case Entities.Components.FilterType.NumberPicker:
                        var tempNumberRangeParam = StringUtil.GenerateUniqueName();
                        if (filter.IsMultiple)
                        {
                            var arrayStrings = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                            var containsNumberMinMax = arrayStrings[0].Contains("-");
                            if (containsNumberMinMax)
                            {
                                // Ex: [10-20,20-30]
                                var compareStr = string.Empty;
                                var counterTemp = 0;
                                foreach (var numRange in arrayStrings)
                                {
                                    var splitted = numRange.Split("-");

                                    var tempStartNumParam = StringUtil.GenerateUniqueName();
                                    var tempEndNumParam = StringUtil.GenerateUniqueName();
                                    if (counterTemp < arrayStrings.Length - 1)
                                    {
                                        compareStr +=
                                            string.Format(options.NumberRangeCompare + " OR ",
                                                GetFieldFormat(filter.Name), GetFieldWithParamSign(tempStartNumParam), GetFieldWithParamSign(tempEndNumParam));
                                    }
                                    else
                                    {
                                        compareStr += string.Format(options.NumberRangeCompare,
                                                GetFieldFormat(filter.Name), GetFieldWithParamSign(tempStartNumParam), GetFieldWithParamSign(tempEndNumParam));
                                    }
                                    counterTemp++;
                                    parameters.Add(new ChartReportParameter
                                    {
                                        Name = tempStartNumParam,
                                        CastedValue = long.Parse(splitted[0]),
                                        ValueType = "long"
                                    });
                                    parameters.Add(new ChartReportParameter
                                    {
                                        Name = tempEndNumParam,
                                        CastedValue = long.Parse(splitted[1]),
                                        ValueType = "long"
                                    });
                                }

                                filterStr += string.Format("({0})", compareStr);
                            }
                            else
                            {
                                var inNumberStr = string.Join(",", arrayStrings);
                                inNumberStr = string.Format(options.InOperator, GetFieldFormat(filter.Name), inNumberStr);
                                filterStr += inNumberStr;
                            }
                        }
                        else
                        {
                            var containsNumberMinMax = filter.Value.Contains("-");
                            if (containsNumberMinMax)
                            {
                                var splitted = filter.Value.Split("-");
                                var tempStartNumParam = StringUtil.GenerateUniqueName();
                                var tempEndNumParam = StringUtil.GenerateUniqueName();
                                filterStr += string.Format(
                                    options.NumberRangeCompare,
                                    GetFieldFormat(filter.Name),
                                    GetFieldWithParamSign(tempStartNumParam),
                                    GetFieldWithParamSign(tempEndNumParam));
                                parameters.Add(new ChartReportParameter
                                {
                                    Name = tempStartNumParam,
                                    CastedValue = long.Parse(splitted[0]),
                                    ValueType = "long"
                                });
                                parameters.Add(new ChartReportParameter
                                {
                                    Name = tempEndNumParam,
                                    CastedValue = long.Parse(splitted[1]),
                                    ValueType = "long"
                                });
                            }
                            else
                            {
                                filterStr += string.Format("{0}={1}",
                                    GetFieldFormat(filter.Name),
                                    GetFieldWithParamSign(tempNumberRangeParam));
                                parameters.Add(new ChartReportParameter
                                {
                                    Name = tempNumberRangeParam,
                                    CastedValue = long.Parse(filter.Value),
                                    ValueType = "long"
                                });
                            }
                        }
                        break;
                    case Entities.Components.FilterType.DatePicker:
                        if (filter.IsMultiple)
                        {
                            // Ex: ['10/10/2019', '11/10/2019']
                            var tempStartDateParam = StringUtil.GenerateUniqueName();
                            var tempEndDateParam = StringUtil.GenerateUniqueName();
                            filterStr +=
                                string.Format("({0} AND {1})",
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        ">=",
                                        GetFieldWithParamSign(tempStartDateParam)),
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        "<=",
                                        GetFieldWithParamSign(tempEndDateParam)));
                            var arrayDates = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                            var startDatePickerDt = DateTime.Parse(arrayDates[0]);
                            var endDatePickerDt = DateTime.Parse(arrayDates[1]);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempStartDateParam,
                                CastedValue = startDatePickerDt,
                                ValueType = "date"
                            });
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempEndDateParam,
                                CastedValue = endDatePickerDt,
                                ValueType = "date"
                            });
                        }
                        else
                        {
                            var tempDatePickerParam = StringUtil.GenerateUniqueName();
                            filterStr += string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        "=",
                                        GetFieldWithParamSign(tempDatePickerParam));
                            // Support Date Format: MM/DD/YYYY
                            var datePickerDt = DateTime.Parse(filter.Value);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempDatePickerParam,
                                CastedValue = datePickerDt,
                                ValueType = "date"
                            });
                        }
                        break;
                    case Entities.Components.FilterType.MonthYearPicker:
                        if (filter.IsMultiple)
                        {
                            var tempStartDateParam = StringUtil.GenerateUniqueName();
                            var tempEndDateParam = StringUtil.GenerateUniqueName();
                            filterStr +=
                                string.Format("({0} AND {1})",
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        ">=",
                                        GetFieldWithParamSign(tempStartDateParam)),
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        "<=",
                                        GetFieldWithParamSign(tempEndDateParam)));

                            var arrayDates = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                            var startDatePickerDt = DateTime.Parse(arrayDates[0]);
                            var endDatePickerDt = DateTime.Parse(arrayDates[1]);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempStartDateParam,
                                CastedValue = startDatePickerDt,
                                ValueType = "date"
                            });
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempEndDateParam,
                                CastedValue = endDatePickerDt,
                                ValueType = "date"
                            });
                        }
                        else
                        {
                            var tempMonthPickerParam = StringUtil.GenerateUniqueName();
                            var tempYearPickerParam = StringUtil.GenerateUniqueName();
                            filterStr +=
                                string.Format("({0} AND {1})",
                                    string.Format(
                                        options.MonthCompare,
                                        GetFieldFormat(filter.Name),
                                        "=",
                                        GetFieldWithParamSign(tempMonthPickerParam)),
                                    string.Format(
                                        options.YearCompare,
                                        GetFieldFormat(filter.Name),
                                        "=",
                                        GetFieldWithParamSign(tempYearPickerParam)));
                            // Support Date Format: MM/DD/YYYY
                            var dateMonthPickerDt = DateTime.Parse(filter.Value);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempMonthPickerParam,
                                CastedValue = dateMonthPickerDt,
                                ValueType = "date"
                            });
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempYearPickerParam,
                                CastedValue = dateMonthPickerDt,
                                ValueType = "date"
                            });
                        }
                        break;
                }
                counter++;
                if (counter < filterValues.Count())
                {
                    filterStr += " AND ";
                }
            }

            return filterStr;
        }
    }
}
