using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Middlewares
{
    public class ResolvingRequestMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequest;

        private readonly IEnumerable<ISiteRouteResolver> _routeResolvers;

        private readonly IEnumerable<ISiteRouteGlobalResolver> _globalResolvers;

        private readonly IMemoryCache _memoryCache;

        public ResolvingRequestMiddleware(
            IEnumerable<ISiteRouteResolver> routeResolvers,
            IEnumerable<ISiteRouteGlobalResolver> globalResolvers,
            ISiteRequestAccessor siteRequest,
            IMemoryCache memoryCache)
        {
            _routeResolvers = routeResolvers;
            _globalResolvers = globalResolvers?.OrderBy(a => a.Order);
            _siteRequest = siteRequest;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var currentSiteRoute = _siteRequest.Current.Route;

            // Found all global resolvers
            if(_globalResolvers != null)
            {
                foreach(var globalResolver in _globalResolvers)
                {
                    if (!await globalResolver.Resolving(context, _siteRequest))
                    {
                        break;
                    };
                }
            }

            if (currentSiteRoute.ResolveMaps != null && currentSiteRoute.ResolveMaps.Any())
            {
                foreach (var resolveMap in currentSiteRoute.ResolveMaps)
                {
                    Dictionary<string, string> dicRouteValues = new Dictionary<string, string>();
                    ISiteRouteResolver foundResolver = _routeResolvers.First(a => a.Name == resolveMap.Resolver);
                    bool foundCached = false;
                    // Enhance by caching in-memory
                    var memoryKey = context.Request.Path + "_" + resolveMap.Key;
                    if (resolveMap.EnableInMemoryCache)
                    {

                        if (_memoryCache.TryGetValue(memoryKey, out object cachedData))
                        {
                            _siteRequest.Current.ResolvedData.Add(resolveMap.Key, cachedData);
                            foundCached = true;
                        }
                    }

                    if (!foundCached)
                    {
                        if (!string.IsNullOrEmpty(resolveMap.Inputs))
                        {
                            var splittedInputs = resolveMap.Inputs.Split(";");
                            foreach (var split in splittedInputs)
                            {
                                if (split.Contains("="))
                                {
                                    var signment = split.Split("=");
                                    dicRouteValues.Add(signment[1], _siteRequest.Current.RouteValues[signment[0]]);
                                }
                                else
                                {
                                    dicRouteValues.Add(split, _siteRequest.Current.RouteValues[split]);
                                }
                            }
                        }
                        else
                        {
                            dicRouteValues = _siteRequest.Current.RouteValues;
                        }

                        await foundResolver.Resolve(_siteRequest, dicRouteValues, resolveMap.Key);
                        if (resolveMap.EnableInMemoryCache)
                        {
                            _memoryCache.Set(memoryKey, _siteRequest.Current.ResolvedData[resolveMap.Key], new TimeSpan(0, 0, resolveMap.CacheDuration));
                        }
                    }
                }
            }

            if (_globalResolvers != null)
            {
                foreach (var globalResolver in _globalResolvers)
                {
                    if(!await globalResolver.Resolved(context, _siteRequest))
                    {
                        break;
                    }
                }
            }

            await next.Invoke(context);
        }
    }
}
