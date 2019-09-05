using LetPortal.Portal.Handlers.Components.Standards.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
{
    public class CreateBulkStandardComponentsRequest : IRequest
    {
        private readonly CreateBulkStandardComponentsCommand _createBulkStandardComponentsCommand;

        public CreateBulkStandardComponentsRequest(CreateBulkStandardComponentsCommand createBulkStandardComponentsCommand)
        {
            _createBulkStandardComponentsCommand = createBulkStandardComponentsCommand;
        }

        public CreateBulkStandardComponentsCommand GetCommand()
        {
            return _createBulkStandardComponentsCommand;
        }
    }
}
