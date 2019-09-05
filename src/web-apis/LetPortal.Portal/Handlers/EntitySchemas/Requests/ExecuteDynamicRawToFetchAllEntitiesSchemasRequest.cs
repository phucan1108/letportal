using LetPortal.Portal.Databases.Models;
using LetPortal.Portal.Handlers.EntitySchemas.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class ExecuteDynamicRawToFetchAllEntitiesSchemasRequest : IRequest<ParsingRawQueryModel>
    {
        private readonly ExecuteDynamicRawToFetchAllEntitiesSchemasQuery _executeDynamicRawToFetchAllEntitiesSchemasQuery;

        public ExecuteDynamicRawToFetchAllEntitiesSchemasRequest(ExecuteDynamicRawToFetchAllEntitiesSchemasQuery executeDynamicRawToFetchAllEntitiesSchemasQuery)
        {
            _executeDynamicRawToFetchAllEntitiesSchemasQuery = executeDynamicRawToFetchAllEntitiesSchemasQuery;
        }

        public ExecuteDynamicRawToFetchAllEntitiesSchemasQuery GetQuery()
        {
            return _executeDynamicRawToFetchAllEntitiesSchemasQuery;
        }
    }
}
