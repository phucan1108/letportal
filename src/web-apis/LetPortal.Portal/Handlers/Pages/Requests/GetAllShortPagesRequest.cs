using LetPortal.Portal.Handlers.Pages.Queries;
using LetPortal.Portal.Models.Pages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class GetAllShortPagesRequest : IRequest<List<ShortPageModel>>
    {
        private readonly GetAllShortPagesQuery _getAllShortPagesQuery;

        public GetAllShortPagesRequest(GetAllShortPagesQuery getAllShortPagesQuery)
        {
            _getAllShortPagesQuery = getAllShortPagesQuery;
        }   
    }
}
