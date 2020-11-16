using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Models;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Middlewares
{
    public class CombiningPageRequestMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public CombiningPageRequestMiddleware(ISiteRequestAccessor siteRequest)
        {
            _siteRequest = siteRequest;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var flexiblePage = new FlexiblePageModel
            {
                Sections = _siteRequest.Current.PageTemplate.Sections.Select(a => new SectionPartModel { Name = a.ThemePartRef, BindingType = a.BindingType, TemplateKey = a.Key }).ToList(),
                GoogleMetadata = _siteRequest.Current.Page.GoogleMetadata
            };

            var pageVersion = _siteRequest.Current.PageVersion;

            if(flexiblePage.Sections != null)
            {
                foreach(var section in flexiblePage.Sections)
                {
                    var foundPartVersion = pageVersion.Manifests.FirstOrDefault(a => a.TemplateKey == section.TemplateKey);
                    if(foundPartVersion != null)
                    {
                        section.DatasourceName = foundPartVersion.DatasourceKey;
                    }
                }
            }

            _siteRequest.Current.RenderedPageModel = flexiblePage;

            await next.Invoke(context);
        }
    }
}
