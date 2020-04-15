using System;

namespace LetPortal.Portal.Mappers
{
    public class CSharpMapper : ICSharpMapper
    {
        public object GetCSharpObjectByType(string value, string type)
        {
            switch (type)
            {
                case "decimal":
                    if (decimal.TryParse(value, out var tempDecimal))
                    {
                        return tempDecimal;
                    }
                    break;
                case "double":
                    if (double.TryParse(value, out var tempDouble))
                    {
                        return tempDouble;
                    }
                    break;
                case "float":
                    if (float.TryParse(value, out var tempFloat))
                    {
                        return tempFloat;
                    }
                    break;
                case "timespan":
                    if (TimeSpan.TryParse(value, out var timeSpan))
                    {
                        return timeSpan;
                    }
                    break;
                case "long":
                    if (long.TryParse(value, out var tempLong))
                    {
                        return tempLong;
                    }
                    break;
                case "int":
                    if (int.TryParse(value, out var tempInt))
                    {
                        return tempInt;
                    }
                    break;
                case "bool":
                    if (value == "1" || value == "0")
                    {
                        return value == "1";
                    }
                    else
                    {
                        if (bool.TryParse(value, out var tempBool))
                        {
                            return tempBool;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var dateTime))
                    {
                        // Very dangerous if we accepts < 1973
                        if(dateTime.Year < 1973)
                        {
                            dateTime = new DateTime(1973, 1, 1);
                        }
                        return dateTime;
                    }
                    break;
                default:
                    return value;
            }

            return null;
        }
    }
}
