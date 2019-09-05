using LetPortal.Portal.Handlers.Components.Standards.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.Standards
{
    public class DeleteBulkStandardComponentsHandler : IRequestHandler<DeleteBulkStandardComponentsRequest>
    {
        private readonly IStandardRepository _standardRepository;

        public DeleteBulkStandardComponentsHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async Task<Unit> Handle(DeleteBulkStandardComponentsRequest request, CancellationToken cancellationToken)
        {
            await _standardRepository.DeleteBulkAsync(request.GetCommand().StandardIds);
            return Unit.Value;
        }
    }
}
