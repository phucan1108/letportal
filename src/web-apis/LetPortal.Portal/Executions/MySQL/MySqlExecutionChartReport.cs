using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExecutionChartReport : IExecutionChartReport
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public async Task<ExecutionChartResponseModel> Execute(
            DatabaseConnection databaseConnection,
            string formattedString,
            string mappingProjection,
            IEnumerable<ChartParameterValue> parameters,
            IEnumerable<ChartFilterValue> filterValues)
        {
            var result = new ExecutionChartResponseModel();
            using(var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                using(var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    var listParams = new List<MySqlParameter>();
                    if(parameters != null)
                    {
                        foreach(var parameter in parameters)
                        {
                            var fieldParam = StringUtil.GenerateUniqueName();
                            formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                            object castObject;
                            listParams.Add(
                                new MySqlParameter(fieldParam, GetMySqlDbType(parameter.Value, out castObject))
                                {
                                    Value = castObject,
                                    Direction = ParameterDirection.Input
                                });
                        }
                    }

                    command.Parameters.AddRange(listParams.ToArray());

                    var warpperString = string.Format("SELECT {0} FROM ({1}) s", GenerateChartColumns(mappingProjection), formattedString);

                    if(filterValues != null && filterValues.Any())
                    {
                        warpperString += "WHERE " + GenerateFilterColumns(filterValues, listParams);
                    }
                    command.CommandText = warpperString; 
                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        using(DataTable dt = new DataTable())
                        {
                            dt.Load(reader);
                            result.IsSuccess = true;
                            if(dt.Rows.Count > 0)
                            {
                                var grouped = dt.AsEnumerable()
                                        .GroupBy(a => a.Field<string>("group"));
                                JArray @array = new JArray();
                                foreach(var group in grouped)
                                {
                                    var data = 
                                        group
                                            .Select(a => new { name = a.Field<string>("name"), value = a.Field<object>("value")})
                                            .ToList();
                                    JObject groupObject = JObject.FromObject(new
                                    {
                                        name = group.Key,
                                        series = data
                                    });

                                    @array.Add(groupObject);
                                }

                                result.Result = @array.ToObject<dynamic>();
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsSuccess = false;
                            }
                        }
                    }
                }
            }

            return result;
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
                    columnStr += string.Format("`{0}` as `{1}` ", splitted[1], splitted[0]);
                }
                else
                {
                    columnStr += string.Format("`{0}` as `{1}`, ", splitted[1], splitted[0]);
                }                 
                counter++;
            }

            return columnStr;
        }

        private string GenerateFilterColumns(IEnumerable<ChartFilterValue> filterValues, List<MySqlParameter> parameters)
        {
            string filterStr = string.Empty;

            int counter = 0;
            foreach(var filter in filterValues)
            {
                switch(filter.FilterType)
                {
                    case Entities.Components.FilterType.Checkbox:
                        var tempCheckBoxParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("`{0}`=@{1}", filter.Name, tempCheckBoxParam);

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
                        parameters.Add(new MySqlParameter(tempCheckBoxParam, MySqlDbType.Bit)
                        {
                            Value = passValue,
                            Direction = ParameterDirection.Input
                        });
                        break;
                    case Entities.Components.FilterType.Select:
                        var tempSelectParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("`{0}`=@{1}", filter.Name, tempSelectParam);

                        // Check value is int or string
                        int selectValue = 0;
                        bool isInt = false;
                        if(int.TryParse(filter.Value, out selectValue))
                        {
                            isInt = true;
                        }
                        MySqlParameter selectMySqlParameter;
                        if(isInt)
                        {
                            selectMySqlParameter = new MySqlParameter(tempSelectParam, MySqlDbType.Int32)
                            {
                                Value = selectValue,
                                Direction = ParameterDirection.Input
                            };
                        }
                        else
                        {
                            selectMySqlParameter = new MySqlParameter(tempSelectParam, MySqlDbType.VarChar)
                            {
                                Value = filter.Value,
                                Direction = ParameterDirection.Input
                            };
                        }
                        parameters.Add(selectMySqlParameter);
                        break;
                    case Entities.Components.FilterType.NumberRange:
                        var tempNumberRangeParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("`{0}`=@{1}", filter.Name, tempNumberRangeParam);
                        parameters.Add(new MySqlParameter(tempNumberRangeParam, MySqlDbType.Int64)
                        {
                            Value = long.Parse(filter.Value),
                            Direction = ParameterDirection.Input
                        });
                        break;
                    case Entities.Components.FilterType.DatePicker:
                        var tempDatePickerParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("date(`{0}`)=date(@{1})", filter.Name, tempDatePickerParam);
                        // Support Date Format: MM/DD/YYYY
                        var datePickerDt = DateTime.Parse(filter.Value);
                        parameters.Add(new MySqlParameter(tempDatePickerParam, MySqlDbType.DateTime)
                        {
                            Value = datePickerDt,
                            Direction = ParameterDirection.Input
                        });
                        break;
                    case Entities.Components.FilterType.DateRange:
                        // A value of DateRange will be ['2019-10-10','2019-11-12']
                        var tempStartDateParam = StringUtil.GenerateUniqueName();
                        var tempEndDateParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("(date(`{0}`)>=date(@{1}) AND date(`{0}`)<=date(@{2}))", filter.Name, tempStartDateParam, tempEndDateParam);
                        var strDates = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                        var dateStart = DateTime.Parse(strDates[0]);
                        var dateEnd = DateTime.Parse(strDates[1]);
                        parameters.Add(new MySqlParameter(tempStartDateParam, MySqlDbType.DateTime)
                        {
                            Value = dateStart,
                            Direction = ParameterDirection.Input
                        });

                        parameters.Add(new MySqlParameter(tempEndDateParam, MySqlDbType.DateTime)
                        {
                            Value = dateEnd,
                            Direction = ParameterDirection.Input
                        });
                        break;
                    case Entities.Components.FilterType.MonthYearPicker:
                        var tempMonthPickerParam = StringUtil.GenerateUniqueName();
                        var tempYearPickerParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("(year(`{0}`)=year(@{1}) AND month(`{0}`)=month(@{2}))", filter.Name, tempYearPickerParam, tempMonthPickerParam);
                        // Support Date Format: MM/DD/YYYY
                        var dateMonthPickerDt = DateTime.Parse(filter.Value);
                        parameters.Add(new MySqlParameter(tempMonthPickerParam, MySqlDbType.DateTime)
                        {
                            Value = dateMonthPickerDt,
                            Direction = ParameterDirection.Input
                        });
                        parameters.Add(new MySqlParameter(tempYearPickerParam, MySqlDbType.DateTime)
                        {
                            Value = dateMonthPickerDt,
                            Direction = ParameterDirection.Input
                        });
                        break;
                    case Entities.Components.FilterType.MonthYearRange:
                        // A value of MonthYearRange will be ['2019-10-10','2019-11-12']
                        var tempStartMonthPickerParam = StringUtil.GenerateUniqueName();
                        var tempStartYearPickerParam = StringUtil.GenerateUniqueName();
                        var tempEndMonthPickerParam = StringUtil.GenerateUniqueName();
                        var tempEndYearPickerParam = StringUtil.GenerateUniqueName();
                        filterStr += string.Format("((year(`{0}`)>=year(@{1}) AND month(`{0}`)>=month(@{2})) AND (year(`{0}`)<=year(@{3}) AND month(`{0}`)<=month(@{4})))", filter.Name, tempStartYearPickerParam, tempStartMonthPickerParam, tempEndYearPickerParam, tempEndMonthPickerParam);
                        // Support Date Format: MM/DD/YYYY
                        var strMonthPickers = ConvertUtil.DeserializeObject<string[]>(filter.Value);
                        var dateStartPicker = DateTime.Parse(strMonthPickers[0]);
                        var dateEndPicker = DateTime.Parse(strMonthPickers[1]);
                        parameters.Add(new MySqlParameter(tempStartMonthPickerParam, MySqlDbType.DateTime)
                        {
                            Value = dateStartPicker,
                            Direction = ParameterDirection.Input
                        });
                        parameters.Add(new MySqlParameter(tempStartYearPickerParam, MySqlDbType.DateTime)
                        {
                            Value = dateStartPicker,
                            Direction = ParameterDirection.Input
                        });
                        parameters.Add(new MySqlParameter(tempEndMonthPickerParam, MySqlDbType.DateTime)
                        {
                            Value = dateEndPicker,
                            Direction = ParameterDirection.Input
                        });
                        parameters.Add(new MySqlParameter(tempEndYearPickerParam, MySqlDbType.DateTime)
                        {
                            Value = dateEndPicker,
                            Direction = ParameterDirection.Input
                        }); 
                        break;
                    case Entities.Components.FilterType.HourPicker:
                        break;
                    case Entities.Components.FilterType.HourRange:
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

        private MySqlDbType GetMySqlDbType(string value, out object castObj)
        {
            if(decimal.TryParse(value, out decimal tempDecimal))
            {
                castObj = tempDecimal;
                return MySqlDbType.Decimal;
            }
            else if(long.TryParse(value, out long tempLong))
            {
                castObj = tempLong;
                return MySqlDbType.Int64;
            }
            else if(int.TryParse(value, out int tempInt))
            {
                castObj = tempInt;
                return MySqlDbType.Int32;
            }
            else if(DateTime.TryParse(value, out DateTime tempDateTime))
            {
                castObj = tempDateTime;
                return MySqlDbType.DateTime;
            }
            else if(bool.TryParse(value, out bool tempBool))
            {
                castObj = tempBool;
                return MySqlDbType.Bit;
            }
            else if(TimeSpan.TryParse(value, out TimeSpan tempTimeSpan))
            {
                castObj = tempTimeSpan;
                return MySqlDbType.Timestamp;
            }
            else
            {
                castObj = value;
                return MySqlDbType.VarChar;
            }
        }
    }
}
