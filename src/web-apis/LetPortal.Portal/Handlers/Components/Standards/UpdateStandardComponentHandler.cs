using LetPortal.Portal.Handlers.Components.Standards.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.Standards
{
    public class UpdateStandardComponentHandler : IRequestHandler<UpdateStandardComponentRequest>
    {
        private readonly IStandardRepository _standardRepository;

        public UpdateStandardComponentHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async  Task<Unit> Handle(UpdateStandardComponentRequest request, CancellationToken cancellationToken)
        {
            await _standardRepository.UpdateAsync(request.GetCommand().StandardId, request.GetCommand().Standard);
            return Unit.Value;
        }
    }
}
