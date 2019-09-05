using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.Standards.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.Standards
{
    public class GetAllStandardComponentsByIdsHandler : IRequestHandler<GetAllStandardComponentsByIdsRequest, IEnumerable<StandardComponent>>
    {
        private readonly IStandardRepository _standardRepository;

        public GetAllStandardComponentsByIdsHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async Task<IEnumerable<StandardComponent>> Handle(GetAllStandardComponentsByIdsRequest request, CancellationToken cancellationToken)
        {
            if(request.GetQuery().Ids == null)
            {
                return await _standardRepository.GetAllAsync();
            }
            return await _standardRepository.GetAllByIdsAsync(request.GetQuery().Ids);
        }
    }
}
