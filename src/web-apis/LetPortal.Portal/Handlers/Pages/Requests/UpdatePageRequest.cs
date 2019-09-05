using LetPortal.Portal.Handlers.Pages.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class UpdatePageRequest : IRequest
    {
        private readonly UpdatePageCommand _updatePageCommand;

        public UpdatePageRequest(UpdatePageCommand updatePageCommand)
        {
            _updatePageCommand = updatePageCommand;
        }

        public UpdatePageCommand GetCommand()
        {
            return _updatePageCommand;
        }
    }
}
