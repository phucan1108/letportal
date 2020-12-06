using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Portal.Repositories.Pages
{
    public class PageMongoRepository : MongoGenericRepository<Page>, IPageRepository
    {
        public PageMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var clonePage = await GetOneAsync(cloneId);
            clonePage.Id = DataUtil.GenerateUniqueId();
            clonePage.Name = cloneName;
            clonePage.DisplayName += " Clone";
            await AddAsync(clonePage);
        }

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var allPages = await GetAllAsync(a => a.AppId == appId);

            var languages = new List<LanguageKey>();

            foreach (var page in allPages)
            {
                languages.AddRange(GetPageLanguages(page));
            }

            return languages;
        }

        public async Task<List<ShortPageModel>> GetAllShortPagesAsync()
        {
            return await Collection.AsQueryable().OfType<Page>().Select(a => new ShortPageModel { Id = a.Id, Name = a.Name, DisplayName = a.DisplayName, UrlPath = a.UrlPath }).ToListAsync();
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeys(string pageId)
        {
            var page = await GetOneAsync(pageId);
            return GetPageLanguages(page);
        }

        public async Task<Page> GetOneByNameAsync(string name)
        {
            return await Collection.AsQueryable().OfType<Page>().FirstAsync(a => a.Name == name);
        }

        public async Task<Page> GetOneByNameForRenderAsync(string name)
        {
            var page = await Collection.AsQueryable().OfType<Page>().FirstAsync(a => a.Name == name);
            if (page != null)
            {
                // Security Notes: Because in render mode, we don't need to return a raw json/command in Command/Datasource
                // Just only return params which will be filled by Client side

                // Datasources
                if (page.PageDatasources != null)
                {
                    page.PageDatasources = page.PageDatasources.Where(a => a.IsActive).ToList();
                    foreach (var ds in page.PageDatasources)
                    {
                        if (ds.Options.Type == Entities.Shared.DatasourceControlType.Database)
                        {
                            ds.Options.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(ds.Options.DatabaseOptions.Query, true));
                        }
                    }
                }

                // Commands
                if (page.Commands != null)
                {
                    foreach (var command in page.Commands)
                    {
                        if (command.ButtonOptions.ActionCommandOptions.ActionType == Entities.Shared.ActionType.ExecuteDatabase
                                && command.ButtonOptions.ActionCommandOptions.DbExecutionChains != null
                                && command.ButtonOptions.ActionCommandOptions.DbExecutionChains.Steps != null)
                        {
                            foreach (var step in command.ButtonOptions.ActionCommandOptions.DbExecutionChains.Steps)
                            {
                                step.ExecuteCommand = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(step.ExecuteCommand, true));
                            }
                        }
                    }
                }
            }
            return page;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortPages(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var filterBuilder = Builders<Page>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var pages = Collection.Find(filterBuilder).ToList();
                return Task.FromResult(pages?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }));
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
        }

        public async Task<List<ShortPortalClaimModel>> GetShortPortalClaimModelsAsync()
        {
            var portalClaims = await Collection.AsQueryable().OfType<Page>().Select(a => new ShortPortalClaimModel { PageDisplayName = a.DisplayName, PageName = a.Name, Claims = a.Claims }).ToListAsync();
            return portalClaims;
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

            if(page.Builder != null 
                && page.Builder.Sections != null
                && page.Builder.Sections.Count > 0)
            {
                foreach(var section in page.Builder.Sections)
                {
                    var sectionName = new LanguageKey
                    {
                        Key = $"pages.{page.Name}.sections.{section.Name}.options.displayName",
                        Value = section.DisplayName
                    };

                    languages.Add(sectionName);
                }
            }

            if(page.Commands != null && page.Commands.Count > 0)
            {
                foreach(var command in page.Commands)
                {
                    var commandName = new LanguageKey
                    {
                        Key = $"pages.{page.Name}.commands.{command.Name}.options.name",
                        Value = command.Name
                    };

                    languages.Add(commandName);
                    if(command.ButtonOptions.ActionCommandOptions.ConfirmationOptions != null
                        && command.ButtonOptions.ActionCommandOptions.ConfirmationOptions.IsEnable)
                    {
                        var commandConfirmationName = new LanguageKey
                        {
                            Key = $"pages.{page.Name}.commands.{command.Name}.options.confirmation",
                            Value = command.ButtonOptions.ActionCommandOptions.ConfirmationOptions.ConfirmationText
                        };

                        languages.Add(commandConfirmationName);
                    }

                    if(command.ButtonOptions.ActionCommandOptions.NotificationOptions != null)
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
