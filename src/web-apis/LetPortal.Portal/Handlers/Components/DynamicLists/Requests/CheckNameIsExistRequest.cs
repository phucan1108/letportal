using LetPortal.Portal.Handlers.Components.DynamicLists.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
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
