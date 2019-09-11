using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.EntitySchemas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.EntitySchemas
{
    public interface IEntitySchemaService
    {
        Task<List<EntitySchema>> FetchAllEntitiesFromDatabase(string databaseId);
    }
}
