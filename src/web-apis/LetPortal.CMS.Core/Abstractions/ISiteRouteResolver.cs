using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.CMS.Core.Abstractions
{
    public interface ISiteRouteResolver
    {
        public string Name { get; }

        public Task Resolve(
            ISiteRequestAccessor request, 
            Dictionary<string, string> routeValues,
            string setterKey);
    }
}
