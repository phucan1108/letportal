using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Core.Persistences
{
    public class MongoOptions
    {
        public List<string> AllowedFunctions { get; set; }

        public string EliminateDoubleQuotes(string formattedString)
        {
            foreach(var function in AllowedFunctions)
            {
                var index = formattedString.IndexOf("\"" + function, StringComparison.Ordinal);
                while (index > 0)
                {
                    var closedCurly = formattedString.IndexOf(")", index, StringComparison.Ordinal);
                    formattedString = formattedString.Remove(closedCurly + 1, 1);
                    formattedString = formattedString.Remove(index, 1);
                    index = formattedString.IndexOf("\"" + function, StringComparison.Ordinal);
                }   
            }               

            return formattedString;
        }
    }
}
