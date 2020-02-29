using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.SectionParts;
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

        public async Task<StandardComponent> GetOneForRenderAsync(string id)
        {
            var standard = await GetOneAsync(id);

            // Remove some security risks
            foreach(var control in standard.Controls)
            {
                if(control.AsyncValidators != null && control.AsyncValidators.Count > 0)
                {
                    foreach(var validator in control.AsyncValidators)
                    {
                        if(validator.AsyncValidatorOptions.ValidatorType == Entities.Components.Controls.AsyncValidatorType.DatabaseValidator)
                        {
                            validator.AsyncValidatorOptions.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(validator.AsyncValidatorOptions.DatabaseOptions.Query, true));
                        }
                    }
                }

                if(control.Type == Entities.SectionParts.Controls.ControlType.Select
                    || control.Type == Entities.SectionParts.Controls.ControlType.AutoComplete)
                {
                    if(control.DatasourceOptions.Type == Entities.Shared.DatasourceControlType.Database)
                    {
                        control.DatasourceOptions.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(control.DatasourceOptions.DatabaseOptions.Query, true));
                    }
                }

                foreach(var controlEvent in control.PageControlEvents)
                {
                    if(controlEvent.EventActionType == Entities.Components.Controls.EventActionType.QueryDatabase)
                    {
                        controlEvent.EventDatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(controlEvent.EventDatabaseOptions.Query, true));
                    }
                }
            }

            return standard;
        }

        public Task<IEnumerable<ShortEntityModel>> GetShortStandards(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var regexFilter = Builders<StandardComponent>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));
                var discriminatorFilter = Builders<StandardComponent>.Filter.Eq("_t", typeof(StandardComponent).Name);
                var combineFilter = Builders<StandardComponent>.Filter.And(discriminatorFilter, regexFilter);
                return Task.FromResult(Collection.Find(combineFilter).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }
    }
}
