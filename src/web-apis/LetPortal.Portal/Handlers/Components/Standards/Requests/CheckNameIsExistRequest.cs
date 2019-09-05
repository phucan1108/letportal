using LetPortal.Portal.Handlers.Components.Standards.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
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
