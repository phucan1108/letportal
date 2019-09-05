using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    /// <summary>
    /// This request is using for analyzing all existed collections in one database
    /// E.g: The database is identity db, after executing, the result should be all of entity schemas
    /// </summary>
    public class FetchAllEntitiesFromDatabaseRequest : IRequest<List<EntitySchema>>
    {
        private readonly FetchAllEntitiesFromDatabaseQuery _fetchAllEntitiesFromDatabaseQuery;

        public FetchAllEntitiesFromDatabaseRequest(FetchAllEntitiesFromDatabaseQuery fetchAllEntitiesFromDatabaseQuery)
        {
            _fetchAllEntitiesFromDatabaseQuery = fetchAllEntitiesFromDatabaseQuery;
        }

        public FetchAllEntitiesFromDatabaseQuery GetQuery()
        {
            return _fetchAllEntitiesFromDatabaseQuery;
        }
    }
}
