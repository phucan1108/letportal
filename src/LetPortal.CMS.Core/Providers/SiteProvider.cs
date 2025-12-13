using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Repositories.Sites;

namespace LetPortal.CMS.Core.Providers
{
    public class SiteProvider : ISiteProvider
    {
        private readonly ISiteRepository _siteRepository;

        public SiteProvider(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
        }

        public async Task<Site> LoadAsync(string requestDomain)
        {
            return await _siteRepository.GetByDomain(requestDomain);
        }
    }
}
