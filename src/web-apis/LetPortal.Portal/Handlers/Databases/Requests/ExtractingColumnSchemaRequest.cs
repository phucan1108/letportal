using LetPortal.Portal.Handlers.Databases.Queries;
using LetPortal.Portal.Models.Databases;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class ExtractingColumnSchemaRequest : IRequest<ExtractingSchemaQueryModel>
    {
        private readonly ExtractingColumnSchemaQuery _extractingColumnSchemaQuery;

        public ExtractingColumnSchemaRequest(ExtractingColumnSchemaQuery extractingColumnSchemaQuery)
        {
            _extractingColumnSchemaQuery = extractingColumnSchemaQuery;
        }

        public ExtractingColumnSchemaQuery GetQuery()
        {
            return _extractingColumnSchemaQuery;
        }
    }
}
