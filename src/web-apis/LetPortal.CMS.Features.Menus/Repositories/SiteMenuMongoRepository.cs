using System;
using System.Threading.Tasks;
using LetPortal.CMS.Features.Menus.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Features.Menus.Repositories
{
    public class SiteMenuMongoRepository : MongoGenericRepository<SiteMenu>, ISiteMenuRepository
    {
        public SiteMenuMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<SiteMenu> GetBySiteId(string siteId)
        {
            try
            {
                return await FindAsync(a => a.SiteId == siteId);
            }
            catch(Exception ex)
            {
                return null;
            }               
        }
    }
}
