using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Handlers.Pages.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class GetOnePageRequest : IRequest<Page>
    {
        private readonly GetOnePageQuery _getOnePageQuery;

        public GetOnePageRequest(GetOnePageQuery getOnePageQuery)
        {
            _getOnePageQuery = getOnePageQuery;
        }

        public GetOnePageQuery GetQuery()
        {
            return _getOnePageQuery;
        }
    }
}
