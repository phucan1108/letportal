using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.Standards.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.Standards
{
    public class GetOneStandardComponentHandler : IRequestHandler<GetOneStandardComponentRequest, StandardComponent>
    {
        private readonly IStandardRepository _standardRepository;

        public GetOneStandardComponentHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async Task<StandardComponent> Handle(GetOneStandardComponentRequest request, CancellationToken cancellationToken)
        {
            return await _standardRepository.GetOneAsync(request.GetQuery().StandardId);
        }
    }
}
