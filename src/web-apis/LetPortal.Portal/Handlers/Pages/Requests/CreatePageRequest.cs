using LetPortal.Portal.Handlers.Pages.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class CreatePageRequest : IRequest<string>
    {
        private readonly CreatePageCommand _createPageCommand;

        public CreatePageRequest(CreatePageCommand createPageCommand)
        {
            _createPageCommand = createPageCommand;
        }

        public CreatePageCommand GetCommand()
        {
            return _createPageCommand;
        }
    }
}