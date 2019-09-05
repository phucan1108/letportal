using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Models.Apps;
using LetPortal.Portal.Providers.Pages;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class GetAvailableUrlsHandler : IRequestHandler<GetAvailableUrlsRequest, List<AvailableUrl>>
    {

        private readonly IPageServiceProvider _pageServiceProvider;

        public GetAvailableUrlsHandler(
            IPageServiceProvider pageServiceProvider)
        {
            _pageServiceProvider = pageServiceProvider;
        }

        public async Task<List<AvailableUrl>> Handle(GetAvailableUrlsRequest request, CancellationToken cancellationToken)
        {
            return (await _pageServiceProvider.GetAllPages()).Select(a => new AvailableUrl { Name = a.DisplayName, Url = a.UrlPath, PageId = a.Id }).ToList();
        }
    }
}
