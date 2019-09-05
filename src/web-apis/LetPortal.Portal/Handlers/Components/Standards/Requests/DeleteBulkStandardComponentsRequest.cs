using LetPortal.Portal.Handlers.Components.Standards.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
{
    public class DeleteBulkStandardComponentsRequest : IRequest
    {
        private readonly DeleteBulkStandardComponentsCommand _deleteBulkStandardComponentsCommand;

        public DeleteBulkStandardComponentsRequest(DeleteBulkStandardComponentsCommand deleteBulkStandardComponentsCommand)
        {
            _deleteBulkStandardComponentsCommand = deleteBulkStandardComponentsCommand;
        }

        public DeleteBulkStandardComponentsCommand GetCommand()
        {
            return _deleteBulkStandardComponentsCommand;
        }
    }
}
