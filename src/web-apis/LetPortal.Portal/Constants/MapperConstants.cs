using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Constants
{
    public class MapperConstants
    {
        public const string String = "string";
        public const string Decimal = "decimal";
        public const string Double = "double";
        public const string Float = "float";
        public const string Timespan = "timespan";
        public const string Long = "long";
        public const string Int = "int";
        public const string Bool = "bool";
        public const string Date = "date";

        public static string ConvertFromJToken(JTokenType tokenType)
        {
            string typeMap;
            switch (tokenType)
            {
                case JTokenType.Boolean:
                    typeMap = Bool;
                    break;
                case JTokenType.Integer:
                    typeMap = Long; 
                    break;
                case JTokenType.Date:
                    typeMap = Date;
                    break;
                case JTokenType.Float:
                    typeMap = Float;
                    break;
                case JTokenType.TimeSpan:
                    typeMap = Timespan;
                    break;
                default:
                    typeMap = String;
                    break;
            }

            return typeMap;
        }
    }
}
