using LetPortal.Portal.Handlers.Pages.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class DeletePageRequest : IRequest
    {
        private readonly DeletePageCommand _deletePageCommand;

        public DeletePageRequest(DeletePageCommand deletePageCommand)
        {
            _deletePageCommand = deletePageCommand;
        }

        public DeletePageCommand GetCommand()
        {
            return _deletePageCommand;
        }
    }
}
