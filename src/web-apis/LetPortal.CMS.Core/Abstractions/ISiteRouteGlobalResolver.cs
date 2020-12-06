using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Abstractions
{
    public interface ISiteRouteGlobalResolver
    {
        int Order { get; }

        public Task<bool> Resolving(
            HttpContext httpContext,
            ISiteRequestAccessor request);

        public Task<bool> Resolved(
            HttpContext httpContext,
            ISiteRequestAccessor request);
    }
}
