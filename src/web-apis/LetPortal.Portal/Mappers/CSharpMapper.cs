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
                    return decimal.Parse(value);
                case "double":
                    return double.Parse(value);
                case "float":
                    return float.Parse(value);
                case "timespan":
                    return TimeSpan.Parse(value);
                case "long":
                    return long.Parse(value);
                case "int":
                    return int.Parse(value);
                case "bool":
                    if(value == "1" || value == "0")
                    {
                        return value == "1";
                    }
                    else
                    {
                        return bool.Parse(value);
                    }
                case "date":
                    return DateTime.Parse(value);
                default:
                    return value;
            }
        }
    }
}
