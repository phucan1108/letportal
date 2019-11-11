using System;

namespace LetPortal.Portal.Mappers
{
    public class CSharpMapper : ICSharpMapper
    {
        public object GetCSharpObjectByType(string value, string type)
        {
            switch(type)
            {
                case "decimal":
                    if(decimal.TryParse(value, out decimal tempDecimal))
                    {
                        return tempDecimal;
                    }
                    break;
                case "double":
                    if(double.TryParse(value, out double tempDouble))
                    {
                        return tempDouble;
                    }
                    break;
                case "float":
                    if(float.TryParse(value, out float tempFloat))
                    {
                        return tempFloat;
                    }
                    break;
                case "timespan":
                    if(TimeSpan.TryParse(value, out TimeSpan timeSpan))
                    {
                        return timeSpan;
                    }
                    break;
                case "long":
                    if(long.TryParse(value, out long tempLong))
                    {
                        return tempLong;
                    }
                    break;
                case "int":
                    if(int.TryParse(value, out int tempInt))
                    {
                        return tempInt;
                    }
                    break;
                case "bool":
                    if(value == "1" || value == "0")
                    {
                        return value == "1";
                    }
                    else
                    {
                        if(bool.TryParse(value, out bool tempBool))
                        {
                            return tempBool;
                        }
                    }
                    break;
                case "date":
                    if(DateTime.TryParse(value, out DateTime dateTime))
                    {
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
