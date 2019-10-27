using LetPortal.Core.Utils;
using LetPortal.Portal.Models.Charts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LetPortal.Portal.Executions
{
    public class ChartReportQueryBuilder : IChartReportQueryBuilder
    {
        private ChartReportQueryOptions options;

        private string formattedString;

        private string mappingProjection;

        private IEnumerable<ChartFilterValue> filterValues;

        private IEnumerable<ChartParameterValue> parameterValues;

        public ChartReportQueryBuilder()
        {
            options = new ChartReportQueryOptions();
        }

        public IChartReportQueryBuilder Init(string formattedString, string mappingProjection, Action<ChartReportQueryOptions> options = null)
        {
            if(options != null)
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

        public ChartReportQuery Build()
        {
            var listParams = new List<ChartReportParameter>();
            var chartReportQuery = new ChartReportQuery();

            if(parameterValues != null)
            {
                foreach(var param in parameterValues)
                {
                    var fieldParam = StringUtil.GenerateUniqueName();
                    formattedString = formattedString.Replace("{{" + param.Name + "}}", "@" + fieldParam);
                    var paramType = GetValueDbType(param.Value, out object castObject);
                    listParams.Add(new ChartReportParameter
                    {
                        Name = fieldParam,
                        CastedValue = castObject,
                        ValueType = paramType
                    });
                }
            }

            var warpperString = string.Format("SELECT {0} FROM ({1}) s", GenerateChartColumns(mappingProjection), formattedString);

            if(filterValues != null && filterValues.Any())
            {
                warpperString += " WHERE " + GenerateFilterColumns(filterValues, ref listParams);
            }

            chartReportQuery.CombinedQuery = warpperString;
            chartReportQuery.DbParameters = listParams;
            return chartReportQuery;
        }

        private string GetFieldFormat(string fieldName)
        {
            return string.Format(options.FieldFormat, fieldName);
        }

        private string GenerateChartColumns(string mappingProjection)
        {
            string columnStr = string.Empty;
            var list = mappingProjection.Split(";");
            int counter = 0;
            foreach(var elem in list)
            {
                var splitted = elem.Split("=");
                if(counter == list.Length - 1)
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

        private Type GetValueDbType(string value, out object castObj)
        {
            if(decimal.TryParse(value, out decimal tempDecimal))
            {
                castObj = tempDecimal;
                return typeof(decimal);
            }
            else if(TimeSpan.TryParse(value, out TimeSpan tempTimeSpan))
            {
                castObj = tempTimeSpan;
                return typeof(TimeSpan);
            }
            else if(long.TryParse(value, out long tempLong))
            {
                castObj = tempLong;
                return typeof(long);
            }
            else if(int.TryParse(value, out int tempInt))
            {
                castObj = tempInt;
                return typeof(int);
            }
            else if(DateTime.TryParse(value, out DateTime tempDateTime))
            {
                castObj = tempDateTime;
                return typeof(DateTime);
            }
            else if(bool.TryParse(value, out bool tempBool))
            {
                castObj = tempBool;
                return typeof(bool);
            }
            else
            {
                castObj = value;
                return typeof(string);
            }
        }

        private string GenerateFilterColumns(IEnumerable<ChartFilterValue> filterValues, ref List<ChartReportParameter> parameters)
        {
            string filterStr = string.Empty;

            int counter = 0;
            foreach(var filter in filterValues)
            {
                switch(filter.FilterType)
                {
                    case Entities.Components.FilterType.Checkbox:
                        var tempCheckBoxParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("{0}={1}{2}", GetFieldFormat(filter.Name), options.ParamSign, tempCheckBoxParam);

                        // Only for MySQL, bool is 0 or 1
                        int passValue = 0;
                        if(filter.Value == "0" || filter.Value == "1")
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
                            ValueType = typeof(bool)
                        });
                        break;
                    case Entities.Components.FilterType.Select:
                        var tempSelectParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("{0}={1}{2}", GetFieldFormat(filter.Name), options.ParamSign, tempSelectParam);

                        // Check value is int or string
                        long selectValue = 0;
                        bool isLong = false;
                        if(long.TryParse(filter.Value, out selectValue))
                        {
                            isLong = true;
                        }
                        ChartReportParameter selectMySqlParameter;
                        if(isLong)
                        {
                            selectMySqlParameter = new ChartReportParameter
                            {
                                Name = tempSelectParam,
                                CastedValue = selectValue,
                                ValueType = typeof(long)
                            };
                        }
                        else
                        {
                            selectMySqlParameter = new ChartReportParameter
                            {
                                Name = tempSelectParam,
                                CastedValue = filter.Value,
                                ValueType = typeof(string)
                            };
                        }
                        parameters.Add(selectMySqlParameter);
                        break;
                    case Entities.Components.FilterType.NumberPicker:
                        var tempNumberRangeParam = StringUtil.GenerateUniqueName();
                        if(filter.IsMultiple)
                        {
                            var arrayStrings = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                            bool containsNumberMinMax = arrayStrings[0].Contains("-");
                            if(containsNumberMinMax)
                            {
                                // Ex: [10-20,20-30]
                                string compareStr = string.Empty;
                                int counterTemp = 0;
                                foreach(var numRange in arrayStrings)
                                {
                                    var splitted = numRange.Split("-");

                                    var tempStartNumParam = StringUtil.GenerateUniqueName();
                                    var tempEndNumParam = StringUtil.GenerateUniqueName();
                                    if(counterTemp < arrayStrings.Length - 1)
                                    {
                                        compareStr += 
                                            string.Format(options.NumberRangeCompare + " OR ", 
                                                GetFieldFormat(filter.Name), tempStartNumParam, tempEndNumParam);
                                    }
                                    else
                                    {
                                        compareStr += string.Format(options.NumberRangeCompare, 
                                                GetFieldFormat(filter.Name), tempStartNumParam, tempEndNumParam);
                                    }
                                    counterTemp++;
                                    parameters.Add(new ChartReportParameter
                                    {
                                        Name = tempStartNumParam,
                                        CastedValue = long.Parse(splitted[0]),
                                        ValueType = typeof(long)
                                    });
                                    parameters.Add(new ChartReportParameter
                                    {
                                        Name = tempEndNumParam,
                                        CastedValue = long.Parse(splitted[1]),
                                        ValueType = typeof(long)
                                    });
                                }

                                filterStr += string.Format("({0})", compareStr);
                            }
                            else
                            {
                                string inNumberStr = string.Join(",", arrayStrings);
                                inNumberStr = string.Format(options.InOperator, GetFieldFormat(filter.Name), inNumberStr);
                                filterStr += inNumberStr;
                            }
                        }
                        else
                        {
                            bool containsNumberMinMax = filter.Value.Contains("-");
                            if(containsNumberMinMax)
                            {
                                var splitted = filter.Value.Split("-");
                                var tempStartNumParam = StringUtil.GenerateUniqueName();
                                var tempEndNumParam = StringUtil.GenerateUniqueName();
                                filterStr += string.Format(options.NumberRangeCompare, GetFieldFormat(filter.Name), tempStartNumParam, tempEndNumParam);
                                parameters.Add(new ChartReportParameter
                                {
                                    Name = tempStartNumParam,
                                    CastedValue = long.Parse(splitted[0]),
                                    ValueType = typeof(long)
                                });
                                parameters.Add(new ChartReportParameter
                                {
                                    Name = tempEndNumParam,
                                    CastedValue = long.Parse(splitted[1]),
                                    ValueType = typeof(long)
                                });
                            }
                            else
                            {
                                filterStr += string.Format("{0}={1}{2}",GetFieldFormat(filter.Name), options.ParamSign, tempNumberRangeParam);
                                parameters.Add(new ChartReportParameter
                                {
                                    Name = tempNumberRangeParam,
                                    CastedValue = long.Parse(filter.Value),
                                    ValueType = typeof(long)
                                });
                            }
                        }
                        break;
                    case Entities.Components.FilterType.DatePicker:
                        if(filter.IsMultiple)
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
                                        options.ParamSign + tempStartDateParam),
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        "<=",
                                        options.ParamSign + tempEndDateParam));                                    
                            var arrayDates = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                            var startDatePickerDt = DateTime.Parse(arrayDates[0]);
                            var endDatePickerDt = DateTime.Parse(arrayDates[1]);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempStartDateParam,
                                CastedValue = startDatePickerDt,
                                ValueType = typeof(DateTime)
                            });
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempEndDateParam,
                                CastedValue = endDatePickerDt,
                                ValueType = typeof(DateTime)
                            });
                        }
                        else
                        {
                            var tempDatePickerParam = StringUtil.GenerateUniqueName();
                            filterStr += string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        "=",
                                        options.ParamSign + tempDatePickerParam);
                            // Support Date Format: MM/DD/YYYY
                            var datePickerDt = DateTime.Parse(filter.Value);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempDatePickerParam,
                                CastedValue = datePickerDt,
                                ValueType = typeof(DateTime)
                            });
                        }
                        break;
                    case Entities.Components.FilterType.MonthYearPicker:
                        if(filter.IsMultiple)
                        {
                            var tempStartDateParam = StringUtil.GenerateUniqueName();
                            var tempEndDateParam = StringUtil.GenerateUniqueName();
                            filterStr +=
                                string.Format("({0} AND {1})",
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        ">=",
                                        options.ParamSign + tempStartDateParam),
                                    string.Format(
                                        options.DateCompare,
                                        GetFieldFormat(filter.Name),
                                        "<=",
                                        options.ParamSign + tempEndDateParam));

                            var arrayDates = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                            var startDatePickerDt = DateTime.Parse(arrayDates[0]);
                            var endDatePickerDt = DateTime.Parse(arrayDates[1]);
                            parameters.Add(new ChartReportParameter
                            {
                                 Name = tempStartDateParam,
                                 CastedValue = startDatePickerDt,
                                 ValueType = typeof(DateTime)
                            });
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempEndDateParam,
                                CastedValue = endDatePickerDt,
                                ValueType = typeof(DateTime)
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
                                        options.ParamSign + tempMonthPickerParam),
                                    string.Format(
                                        options.YearCompare,
                                        GetFieldFormat(filter.Name),
                                        "=",
                                        options.ParamSign + tempYearPickerParam));
                            // Support Date Format: MM/DD/YYYY
                            var dateMonthPickerDt = DateTime.Parse(filter.Value);
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempMonthPickerParam,
                                CastedValue = dateMonthPickerDt,
                                ValueType = typeof(DateTime)
                            });
                            parameters.Add(new ChartReportParameter
                            {
                                Name = tempYearPickerParam,
                                CastedValue = dateMonthPickerDt,
                                ValueType = typeof(DateTime)
                            });
                        }
                        break;
                }
                counter++;
                if(counter < filterValues.Count())
                {
                    filterStr += " AND ";
                }
            }

            return filterStr;
        }
    }
}
