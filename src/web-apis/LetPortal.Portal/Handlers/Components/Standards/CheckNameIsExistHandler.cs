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
    public class CheckNameIsExistHandler : IRequestHandler<CheckNameIsExistRequest, bool>
    {
        private readonly IStandardRepository _standardRepository;

        public CheckNameIsExistHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async Task<bool> Handle(CheckNameIsExistRequest request, CancellationToken cancellationToken)
        {
            return await _standardRepository.IsExistAsync(request.GetQuery().Name);
        }
    }
}
