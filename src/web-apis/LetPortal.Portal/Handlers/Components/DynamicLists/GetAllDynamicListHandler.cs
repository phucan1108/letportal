using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class GetAllDynamicListHandler : IRequestHandler<GetAllDynamicListRequest, List<DynamicList>>
    {
        private readonly IDynamicListRepository _dynamicListRepository;

        public GetAllDynamicListHandler(IDynamicListRepository dynamicListRepository)
        {
            _dynamicListRepository = dynamicListRepository;
        }

        public Task<List<DynamicList>> Handle(GetAllDynamicListRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dynamicListRepository.GetAsQueryable().ToList());
        }
    }
}
