using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string name)
        {
            return (Char.ToLowerInvariant(name[0]) + name.Substring(1));
        }
    }
}
