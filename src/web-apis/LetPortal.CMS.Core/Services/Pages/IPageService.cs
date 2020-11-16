using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Entities;

namespace LetPortal.CMS.Core.Services.Pages
{
    public interface IPageService
    {
        Task<T> LoadModel<T>(T target, string templateKey, ISiteRequestAccessor siteRequest);

        Task<object> LoadModel<T>(string templateKey, ISiteRequestAccessor siteRequest);

        Task<List<VersionValue>> InitManifests(PageTemplate pageTemplate, Theme chosenTheme);
    }
}
