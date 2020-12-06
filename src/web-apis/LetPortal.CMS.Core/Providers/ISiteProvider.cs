using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;

namespace LetPortal.CMS.Core.Providers
{
    public interface ISiteProvider
    {
        Task<Site> LoadAsync(string requestDomain);
    }
}
