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
                    // Cheat: mongodb doesn't support to parse string to boolean, we need to remove by hardcode
                    if (function.Equals("Boolean", StringComparison.Ordinal))
                    {
                        formattedString = formattedString.Remove(closedCurly, 2); // Remove )"
                        formattedString = formattedString.Remove(index, 1);
                    }
                    else
                    {
                        formattedString = formattedString.Remove(closedCurly + 1, 1);
                        formattedString = formattedString.Remove(index, 1);
                    }
                    index = formattedString.IndexOf("\"" + function, StringComparison.Ordinal);
                }
            }


            formattedString = formattedString.Replace("Boolean(", "", StringComparison.Ordinal);

            return formattedString;
        }
    }
}
