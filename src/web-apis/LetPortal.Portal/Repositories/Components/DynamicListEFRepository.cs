using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Components
{
    public class DynamicListEFRepository : EFGenericRepository<DynamicList>, IDynamicListRepository
    {
        private readonly PortalDbContext _context;

        public DynamicListEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneList = await _context.DynamicLists.AsNoTracking().FirstAsync(a => a.Id == cloneId);
            cloneList.Id = DataUtil.GenerateUniqueId();
            cloneList.Name = cloneName;
            cloneList.DisplayName += " Clone";
            await AddAsync(cloneList);
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeysAsync(string dynamicListId)
        {
            var dynamicList = await GetOneAsync(dynamicListId);

            return GetDynamicListLanguages(dynamicList);
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortDynamicLists(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var dynamicLists = await _context.DynamicLists.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName }).ToListAsync();
                return dynamicLists?.AsEnumerable();
            }
            else
            {
                return (await _context.DynamicLists.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).ToListAsync())?.AsEnumerable();
            }
        }

        private IEnumerable<LanguageKey> GetDynamicListLanguages(DynamicList dynamicList)
        {
            var langauges = new List<LanguageKey>();

            var dynamicListName = new LanguageKey
            { 
                Key = $"{dynamicList.Name}.options.displayName",
                Value = dynamicList.DisplayName
            };

            langauges.Add(dynamicListName);
            foreach (var column in dynamicList.ColumnsList.ColumndDefs)
            {
                if (!column.IsHidden)
                {
                    var columnName = new LanguageKey
                    {
                        Key = $"{dynamicList.Name}.cols.{column.Name}.displayName",
                        Value = column.DisplayName
                    };

                    langauges.Add(columnName);
                }                  
            }

            if(dynamicList.CommandsList != null && dynamicList.CommandsList.CommandButtonsInList != null)
            {
                foreach(var command in dynamicList.CommandsList.CommandButtonsInList)
                {
                    var commandName = new LanguageKey
                    {
                        Key = $"{dynamicList.Name}.commands.{command.Name}.displayName",
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
                            if(command.ActionCommandOptions.ConfirmationOptions != null)
                            {
                                var confirmationText = new LanguageKey
                                {
                                    Key = $"{dynamicList.Name}.commands.{command.Name}.confirmation.text",
                                    Value = command.ActionCommandOptions.ConfirmationOptions.ConfirmationText
                                };
                                langauges.Add(confirmationText);
                            }

                            if(command.ActionCommandOptions.NotificationOptions != null)
                            {
                                var notificationSuccess = new LanguageKey
                                {
                                    Key = $"{dynamicList.Name}.commands.{command.Name}.notification.success",
                                    Value = command.ActionCommandOptions.NotificationOptions.CompleteMessage
                                };
                                var notificationFailed = new LanguageKey
                                {
                                    Key = $"{dynamicList.Name}.commands.{command.Name}.notification.failed",
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
