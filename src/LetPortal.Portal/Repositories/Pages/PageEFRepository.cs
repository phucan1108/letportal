using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
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

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var allPages = await GetAllAsync(a => a.AppId == appId);

            var languages = new List<LanguageKey>();

            foreach(var page in allPages)
            {
                languages.AddRange(GetPageLanguages(page));
            }

            return languages;
        }

        public Task<List<ShortPageModel>> GetAllShortPagesAsync()
        {
            var pages = _context.Pages.Select(a => new ShortPageModel { Id = a.Id, Name = a.Name, DisplayName = a.DisplayName, UrlPath = a.UrlPath });

            return Task.FromResult(pages.ToList());
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeys(string pageId)
        {
            var page = await GetOneAsync(pageId);
            return GetPageLanguages(page);
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
                var pages = await _context.Pages.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName, AppId = b.AppId }).ToListAsync();
                return pages?.AsEnumerable();
            }
            else
            {
                return (await _context.Pages.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).ToListAsync())?.AsEnumerable();
            }
        }

        public Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync()
        {
            var portalClaims = _context.Pages.Select(a => new ShortPortalClaimModel { PageDisplayName = a.DisplayName, PageName = a.Name, Claims = a.Claims });
            return Task.FromResult(portalClaims.ToList());
        }

        private IEnumerable<LanguageKey> GetPageLanguages(Page page)
        {
            var languages = new List<LanguageKey>();

            var pageName = new LanguageKey
            {
                Key = $"pages.{page.Name}.options.displayName",
                Value = page.DisplayName
            };

            languages.Add(pageName);

            if (page.Commands != null && page.Commands.Count > 0)
            {
                foreach (var command in page.Commands)
                {
                    var commandName = new LanguageKey
                    {
                        Key = $"pages.{page.Name}.commands.{command.Name}.options.name",
                        Value = command.Name
                    };

                    languages.Add(commandName);
                    if (command.ButtonOptions.ActionCommandOptions.ConfirmationOptions != null
                        && command.ButtonOptions.ActionCommandOptions.ConfirmationOptions.IsEnable)
                    {
                        var commandConfirmationName = new LanguageKey
                        {
                            Key = $"pages.{page.Name}.commands.{command.Name}.options.confirmation",
                            Value = command.ButtonOptions.ActionCommandOptions.ConfirmationOptions.ConfirmationText
                        };

                        languages.Add(commandConfirmationName);
                    }

                    if (command.ButtonOptions.ActionCommandOptions.NotificationOptions != null)
                    {
                        var commandSuccessText = new LanguageKey
                        {
                            Key = $"pages.{page.Name}.commands.{command.Name}.options.success",
                            Value = command.ButtonOptions.ActionCommandOptions.NotificationOptions.CompleteMessage
                        };

                        var commandFailedText = new LanguageKey
                        {
                            Key = $"pages.{page.Name}.commands.{command.Name}.options.failed",
                            Value = command.ButtonOptions.ActionCommandOptions.NotificationOptions.FailedMessage
                        };
                        languages.Add(commandSuccessText);
                        languages.Add(commandFailedText);
                    }
                }
            }
            return languages;
        }
    }
}
