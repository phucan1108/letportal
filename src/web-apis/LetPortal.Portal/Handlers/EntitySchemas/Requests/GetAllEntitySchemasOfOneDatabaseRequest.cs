using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class GetAllEntitySchemasOfOneDatabaseRequest : IRequest<List<EntitySchema>>
    {
        private readonly GetAllEntitySchemasOfOneDatabaseQuery _getAllEntitySchemasOfOneDatabaseQuery;

        public GetAllEntitySchemasOfOneDatabaseRequest(GetAllEntitySchemasOfOneDatabaseQuery getAllEntitySchemasOfOneDatabaseQuery)
        {
            _getAllEntitySchemasOfOneDatabaseQuery = getAllEntitySchemasOfOneDatabaseQuery;
        }

        public GetAllEntitySchemasOfOneDatabaseQuery GetQuery()
        {
            return _getAllEntitySchemasOfOneDatabaseQuery;
        }
    }
}
