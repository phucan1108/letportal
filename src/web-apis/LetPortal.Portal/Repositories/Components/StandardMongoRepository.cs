using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.SectionParts.Controls;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Extensions;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories.Components
{
    public class StandardMongoRepository : MongoGenericRepository<StandardComponent>, IStandardRepository
    {
        public StandardMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneStandard = await GetOneAsync(cloneId);
            cloneStandard.Id = DataUtil.GenerateUniqueId();
            cloneStandard.Name = cloneName;
            cloneStandard.DisplayName += " Clone";
            await AddAsync(cloneStandard);
        }

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var allStandards = await GetAllAsync(a => a.AppId == appId, isRequiredDiscriminator: true);

            var languages = new List<LanguageKey>();

            foreach(var standard in allStandards)
            {
                languages.AddRange(GetStandardLanguages(standard));
            }

            return languages;
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeysAsync(string standardId)
        {
            var standard = await GetOneAsync(standardId);

            return GetStandardLanguages(standard);
        }

        public async Task<StandardComponent> GetOneForRenderAsync(string id)
        {
            var standard = await GetOneAsync(id);

            // Remove some security risks
            foreach(var control in standard.Controls)
            {
                control.HideSensitive();
            }

            return standard;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortArrayStandards(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<StandardComponent>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var arrayFilter = Builders<StandardComponent>.Filter.Eq(a => a.Type, StandardType.Array);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, regexFilter, arrayFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
            else
            {                
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var arrayFilter = Builders<StandardComponent>.Filter.Eq(a => a.Type, StandardType.Array);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, arrayFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }            
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortStandards(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<StandardComponent>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var nonArrayFilter = Builders<StandardComponent>.Filter.Eq(a => a.Type, StandardType.Standard);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, regexFilter, nonArrayFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
            else
            {                   
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var nonArrayFilter = Builders<StandardComponent>.Filter.Eq(a => a.Type, StandardType.Standard);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, nonArrayFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }            
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortTreeStandards(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<StandardComponent>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var treeFilter = Builders<StandardComponent>.Filter.Eq(a => a.Type, StandardType.Tree);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, regexFilter, treeFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }
            else
            {
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var treeFilter = Builders<StandardComponent>.Filter.Eq(a => a.Type, StandardType.Tree);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, treeFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName, AppId = a.AppId }).AsEnumerable());
            }            
        }

        private List<LanguageKey> GetStandardLanguages(StandardComponent standard)
        {
            var languages = new List<LanguageKey>();

            var standardName = new LanguageKey
            {
                 Key = $"standardComponents.{standard.Name}.options.displayName",
                 Value = standard.DisplayName
            };

            languages.Add(standardName);

            if (standard.Controls != null && standard.Controls.Count > 0)
            {
                foreach (var control in standard.Controls)
                {
                    if (control.Options.First(a => a.Key == "hidden").Value != "true")
                    {
                        // Control options
                        var labelLang = new LanguageKey
                        {
                            Key = $"standardComponents.{standard.Name}.{control.Name}.options.label",
                            Value = control.Options.First(a => a.Key == "label").Value
                        };

                        var placeholderLang = new LanguageKey
                        {
                            Key = $"standardComponents.{standard.Name}.{control.Name}.options.placeholder",
                            Value = control.Options.First(a => a.Key == "placeholder").Value
                        };

                        languages.Add(labelLang);
                        languages.Add(placeholderLang);
                        // Control validators
                        foreach (var validator in control.Validators)
                        {
                            if (validator.IsActive)
                            {
                                var validatorLang = new LanguageKey
                                {
                                    Key = $"standardComponents.{standard.Name}.{control.Name}.validators.{Enum.GetName(typeof(ValidatorType), validator.ValidatorType)}",
                                    Value = validator.ValidatorMessage
                                };

                                languages.Add(validatorLang);
                            }
                        }

                        // Control asyncValidators
                        foreach (var validator in control.AsyncValidators)
                        {
                            if (validator.IsActive)
                            {
                                var asyncValidatorLang = new LanguageKey
                                {
                                    Key = $"standardComponents.{standard.Name}.{control.Name}.asyncValidators.{validator.ValidatorName}",
                                    Value = validator.ValidatorMessage
                                };
                                languages.Add(asyncValidatorLang);
                            }
                        }
                    }
                }
            }

            return languages;
        }
    }
}
