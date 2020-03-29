﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Pages
{
    public class PageEFRepository : EFGenericRepository<Page>, IPageRepository
    {
        private readonly PortalDbContext _context;

        public PageEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var clonePage = await _context.Pages.AsNoTracking().FirstAsync(a => a.Id == cloneId);
            clonePage.Id = DataUtil.GenerateUniqueId();
            clonePage.Name = cloneName;
            clonePage.DisplayName += " Clone";
            await AddAsync(clonePage);
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
            if (page != null)
            {
                // Security Notes: Because in render mode, we don't need to return a raw json/command in Command/Datasource
                // Just only return params which will be filled by Client side

                if (page.PageDatasources != null)
                {
                    // Datasources
                    page.PageDatasources = page.PageDatasources.Where(a => a.IsActive).ToList();
                    foreach (var ds in page.PageDatasources)
                    {
                        if (ds.Options.Type == Entities.Shared.DatasourceControlType.Database)
                        {
                            ds.Options.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(ds.Options.DatabaseOptions.Query, true));
                        }
                    }
                }

                if (page.Commands != null)
                {
                    // Commands
                    foreach (var command in page.Commands)
                    {
                        if (command.ButtonOptions.ActionCommandOptions.ActionType == Entities.Shared.ActionType.ExecuteDatabase
                                && command.ButtonOptions.ActionCommandOptions.DbExecutionChains != null
                                && command.ButtonOptions.ActionCommandOptions.DbExecutionChains.Steps != null)
                        {
                            foreach(var step in command.ButtonOptions.ActionCommandOptions.DbExecutionChains.Steps)
                            {
                                step.ExecuteCommand = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(step.ExecuteCommand, true));
                            }                            
                        }
                    }
                }
            }
            return Task.FromResult(page);
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortPages(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var pages = await _context.Pages.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName }).ToListAsync();
                return pages?.AsEnumerable();
            }
            else
            {
                return (await _context.Pages.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).ToListAsync())?.AsEnumerable();
            }
        }

        public Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync()
        {
            var portalClaims = _context.Pages.Select(a => new ShortPortalClaimModel { PageDisplayName = a.DisplayName, PageName = a.Name, Claims = a.Claims });
            return Task.FromResult(portalClaims.ToList());
        }
    }
}
