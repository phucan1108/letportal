using LetPortal.Portal.Handlers.Components.Standards.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.Standards
{
    public class CreateBulkStandardComponentsHandler : IRequestHandler<CreateBulkStandardComponentsRequest>
    {
        private readonly IStandardRepository _standardRepository;

        public CreateBulkStandardComponentsHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async Task<Unit> Handle(CreateBulkStandardComponentsRequest request, CancellationToken cancellationToken)
        {
            await _standardRepository.AddBulkAsync(request.GetCommand().Standards);
            return Unit.Value;
        }
    }
}
