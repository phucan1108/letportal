using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Models.Shared;
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

        public async Task<Page> GetOneByNameForRenderAsync(string name)
        {
            var page = await Collection.AsQueryable().OfType<Page>().FirstAsync(a => a.Name == name);
            if(page != null)
            {
                // Security Notes: Because in render mode, we don't need to return a raw json/command in Command/Datasource
                // Just only return params which will be filled by Client side

                // Datasources
                if(page.PageDatasources != null)
                {
                    page.PageDatasources = page.PageDatasources.Where(a => a.IsActive).ToList();
                    foreach(var ds in page.PageDatasources)
                    {
                        if(ds.Options.Type == Entities.Shared.DatasourceControlType.Database)
                        {
                            ds.Options.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(ds.Options.DatabaseOptions.Query, true));
                        }
                    }
                }


                // Commands
                if(page.Commands != null)
                {
                    foreach(var command in page.Commands)
                    {
                        if(command.ButtonOptions.ActionCommandOptions.ActionType == Entities.Shared.ActionType.ExecuteDatabase)
                        {
                            command.ButtonOptions.ActionCommandOptions.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(command.ButtonOptions.ActionCommandOptions.DatabaseOptions.Query, true));
                        }
                    }
                }
            }
            return page;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortPages(string keyWord = null)
        {
            if(!string.IsNullOrEmpty(keyWord))
            {
                var filterBuilder = Builders<Page>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var pages = Collection.Find(filterBuilder).ToList();
                return Task.FromResult(pages?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }));
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }

        public async Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync()
        {
            var portalClaims = await Collection.AsQueryable().OfType<Page>().Select(a => new ShortPortalClaimModel { PageDisplayName = a.DisplayName, PageName = a.Name, Claims = a.Claims }).ToListAsync();
            return portalClaims;
        }
    }
}
