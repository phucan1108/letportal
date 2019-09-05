using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Handlers.Pages.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class GetOnePageByIdRequest : IRequest<Page>
    {
        private readonly GetOnePageByIdQuery _getOnePageByIdQuery;

        public GetOnePageByIdRequest(GetOnePageByIdQuery getOnePageByIdQuery)
        {
            _getOnePageByIdQuery = getOnePageByIdQuery;
        }

        public GetOnePageByIdQuery GetQuery()
        {
            return _getOnePageByIdQuery;
        }
    }
}
