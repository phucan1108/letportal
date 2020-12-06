using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories.Components
{
    public class DynamicListMongoRepository : MongoGenericRepository<DynamicList>, IDynamicListRepository
    {
        public DynamicListMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneList = await GetOneAsync(cloneId);
            cloneList.Id = DataUtil.GenerateUniqueId();
            cloneList.Name = cloneName;
            cloneList.DisplayName += " Clone";
            await AddAsync(cloneList);
        }

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var allDynamicLists = await GetAllAsync(a => a.AppId == appId, isRequiredDiscriminator: true);

            var languages = new List<LanguageKey>();

            foreach (var dynamicList in allDynamicLists)
            {
                languages.AddRange(GetDynamicListLanguages(dynamicList));
            }

            return languages;
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeysAsync(string dynamicListId)
        {
            var dynamicList = await GetOneAsync(dynamicListId);

            return GetDynamicListLanguages(dynamicList);
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<DynamicList>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<DynamicList>.Filter.Eq("_t", typeof(DynamicList).Name);
                var combineFilter = Builders<DynamicList>.Filter.And(discriminatorFilter, regexFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
            else
            {                   
                var discriminatorFilter = Builders<DynamicList>.Filter.Eq("_t", typeof(DynamicList).Name);
                return Task.FromResult(Collection.Find(discriminatorFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }            
        }

        private IEnumerable<LanguageKey> GetDynamicListLanguages(DynamicList dynamicList)
        {
            var langauges = new List<LanguageKey>();

            var dynamicListName = new LanguageKey
            {
                Key = $"dynamicLists.{dynamicList.Name}.options.displayName",
                Value = dynamicList.DisplayName
            };

            langauges.Add(dynamicListName);

            if (dynamicList.ColumnsList != null && dynamicList.ColumnsList.ColumnDefs != null && dynamicList.ColumnsList.ColumnDefs.Count > 0)
            {

                foreach (var column in dynamicList.ColumnsList.ColumnDefs)
                {
                    if (!column.IsHidden)
                    {
                        var columnName = new LanguageKey
                        {
                            Key = $"dynamicLists.{dynamicList.Name}.cols.{column.Name}.displayName",
                            Value = column.DisplayName
                        };

                        langauges.Add(columnName);
                    }
                }
            }

            if (dynamicList.CommandsList != null && dynamicList.CommandsList.CommandButtonsInList != null)
            {
                foreach (var command in dynamicList.CommandsList.CommandButtonsInList)
                {
                    var commandName = new LanguageKey
                    {
                        Key = $"dynamicLists.{dynamicList.Name}.commands.{command.Name}.displayName",
                        Value = command.DisplayName
                    };

                    langauges.Add(commandName);

                    switch (command.ActionCommandOptions.ActionType)
                    {
                        case ActionType.Redirect:
                            break;
                        case ActionType.ExecuteDatabase:
                        case ActionType.CallHttpService:
                        default:
                            if (command.ActionCommandOptions.ConfirmationOptions != null)
                            {
                                var confirmationText = new LanguageKey
                                {
                                    Key = $"dynamicLists.{dynamicList.Name}.commands.{command.Name}.confirmation.text",
                                    Value = command.ActionCommandOptions.ConfirmationOptions.ConfirmationText
                                };
                                langauges.Add(confirmationText);
                            }

                            if (command.ActionCommandOptions.NotificationOptions != null)
                            {
                                var notificationSuccess = new LanguageKey
                                {
                                    Key = $"dynamicLists.{dynamicList.Name}.commands.{command.Name}.notification.success",
                                    Value = command.ActionCommandOptions.NotificationOptions.CompleteMessage
                                };
                                var notificationFailed = new LanguageKey
                                {
                                    Key = $"dynamicLists.{dynamicList.Name}.commands.{command.Name}.notification.failed",
                                    Value = command.ActionCommandOptions.NotificationOptions.FailedMessage
                                };
                                langauges.Add(notificationSuccess);
                                langauges.Add(notificationFailed);
                            }
                            break;
                    }
                }
            }

            return langauges;
        }
    }
}
