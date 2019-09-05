using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Commands;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class FlushEntitySchemasInOneDatabaseRequest : IRequest<List<EntitySchema>>
    {
        private readonly FlushEntitySchemasInOneDatabaseCommand _flushEntitySchemasInOneDatabaseCommand;

        public FlushEntitySchemasInOneDatabaseRequest(FlushEntitySchemasInOneDatabaseCommand flushEntitySchemasInOneDatabaseCommand)
        {
            _flushEntitySchemasInOneDatabaseCommand = flushEntitySchemasInOneDatabaseCommand;
        }

        public FlushEntitySchemasInOneDatabaseCommand GetCommand()
        {
            return _flushEntitySchemasInOneDatabaseCommand;
        }
    }
}
