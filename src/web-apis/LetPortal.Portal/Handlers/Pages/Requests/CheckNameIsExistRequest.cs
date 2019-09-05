using LetPortal.Portal.Handlers.Pages.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class CheckNameIsExistRequest : IRequest<bool>
    {
        private readonly CheckNameIsExistQuery _checkNameIsExistQuery;
        public CheckNameIsExistRequest(CheckNameIsExistQuery checkNameIsExistQuery)
        {
            _checkNameIsExistQuery = checkNameIsExistQuery;
        }

        public CheckNameIsExistQuery GetQuery()
        {
            return _checkNameIsExistQuery;
        }
    }
}
