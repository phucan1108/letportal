namespace LetPortal.Portal.Databases.Models
{
    public class ParsingParam
    {
        public string Name { get; set; }
        public string ParamType { get; set; }
        public string DefaultValue { get; set; }

        public string GetFullParam()
        {
            return string.Format("({0}|{1}|{2})", Name, DefaultValue, ParamType);
        }

        public string GetNameParam()
        {
            return string.Format("({0})", Name);
        }
    }
}
