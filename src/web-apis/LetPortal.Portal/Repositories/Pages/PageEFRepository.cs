using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Pages
{
    public class PageEFRepository : EFGenericRepository<Page>, IPageRepository
    {
        private readonly LetPortalDbContext _context;

        public PageEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<List<ShortPageModel>> GetAllShortPagesAsync()
        {
            var pages = _context.Pages.Select(a => new ShortPageModel { Id = a.Id, Name = a.Name, DisplayName = a.DisplayName, UrlPath = a.UrlPath });

            return Task.FromResult(pages.ToList());
        }

        public Task<Page> GetOneByNameAsync(string name)
        {
            return Task.FromResult(_context.Pages.First(a => a.Name == name));
        }

        public Task<Page> GetOneByNameForRenderAsync(string name)
        {
            var page = _context.Pages.First(a => a.Name == name);
            if(page != null)
            {
                // Security Notes: Because in render mode, we don't need to return a raw json/command in Command/Datasource
                // Just only return params which will be filled by Client side

                // Datasources
                page.PageDatasources = page.PageDatasources.Where(a => a.IsActive).ToList();
                foreach(var ds in page.PageDatasources)
                {
                    if(ds.Options.Type == Entities.Shared.DatasourceControlType.Database)
                    {
                        ds.Options.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(ds.Options.DatabaseOptions.Query, true));
                    }
                }

                // Commands
                foreach(var command in page.Commands)
                {
                    if(command.ButtonOptions.ActionCommandOptions.ActionType == Entities.Shared.ActionType.ExecuteDatabase)
                    {
                        command.ButtonOptions.ActionCommandOptions.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(command.ButtonOptions.ActionCommandOptions.DatabaseOptions.Query, true));
                    }
                }
            }
            return Task.FromResult(page);
        }

        public Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync()
        {
            var portalClaims = _context.Pages.Select(a => new ShortPortalClaimModel { PageDisplayName = a.DisplayName, PageName = a.Name, Claims = a.Claims });
            return Task.FromResult(portalClaims.ToList());
        }
    }
}
