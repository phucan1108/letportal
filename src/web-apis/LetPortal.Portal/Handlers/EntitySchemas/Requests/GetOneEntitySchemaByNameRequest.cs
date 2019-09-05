using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class GetOneEntitySchemaByNameRequest : IRequest<EntitySchema>
    {
        private readonly GetOneEntitySchemaByNameQuery _getOneEntitySchemaByNameQuery;

        public GetOneEntitySchemaByNameRequest(GetOneEntitySchemaByNameQuery getOneEntitySchemaByNameQuery)
        {
            _getOneEntitySchemaByNameQuery = getOneEntitySchemaByNameQuery;
        }

        public GetOneEntitySchemaByNameQuery GetQuery()
        {
            return _getOneEntitySchemaByNameQuery;
        }
    }
}
