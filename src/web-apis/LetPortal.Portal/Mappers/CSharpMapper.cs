using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Mappers
{
    public class CSharpMapper : ICSharpMapper
    {
        public object GetCSharpObjectByType(string value, string type)
        {
            switch(type)
            {
                case "decimal":
                    decimal.TryParse(value, out decimal tempDecimal);
                    return tempDecimal;
                case "double":
                    double.TryParse(value, out double tempDouble);
                    return tempDouble;
                case "float":
                    float.TryParse(value, out float tempFloat);
                    return tempFloat;
                case "timespan":
                    TimeSpan.TryParse(value, out TimeSpan timeSpan);
                    return timeSpan;
                case "long":
                    long.TryParse(value, out long tempLong);
                    return tempLong;
                case "int":
                    int.TryParse(value, out int tempInt);
                    return tempInt;
                case "bool":
                    if(value == "1" || value == "0")
                    {
                        return value == "1";
                    }
                    else
                    {
                        bool.TryParse(value, out bool tempBool);
                        return tempBool;
                    }
                case "date":
                    DateTime.TryParse(value, out DateTime dateTime);
                    return dateTime;
                default:
                    return value;
            }
        }
    }
}
