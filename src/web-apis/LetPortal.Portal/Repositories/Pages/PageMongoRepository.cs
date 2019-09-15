using LetPortal.Core.Persistences;
using LetPortal.Core.Security;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
                                                               
namespace LetPortal.Portal.Repositories.Pages
{
    public class PageMongoRepository : MongoGenericRepository<Page>, IPageRepository
    {
        public PageMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<List<ShortPageModel>> GetAllShortPagesAsync()
        {
            return await Collection.AsQueryable().OfType<Page>().Select(a => new ShortPageModel { Id = a.Id, Name = a.Name, DisplayName = a.DisplayName, UrlPath = a.UrlPath }).ToListAsync();
        }

        public async Task<Page> GetOneByNameAsync(string name)
        {
            return await Collection.AsQueryable().OfType<Page>().FirstAsync(a => a.Name == name);  
        }

        public async Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync()
        {
            var portalClaims = await Collection.AsQueryable().OfType<Page>().Select(a => new ShortPortalClaimModel { PageDisplayName = a.DisplayName,  PageName = a.Name, Claims = a.Claims }).ToListAsync();
            return portalClaims;
        }
    }
}
