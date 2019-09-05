using LetPortal.Core.Utils;
using LetPortal.Portal.Handlers.Components.Standards.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.Standards
{
    public class CreateStandardComponentHandler : IRequestHandler<CreateStandardComponentRequest, string>
    {
        private readonly IStandardRepository _standardRepository;

        public CreateStandardComponentHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }    

        public async Task<string> Handle(CreateStandardComponentRequest request, CancellationToken cancellationToken)
        {
            var standard = request.GetCommand().Standard;
            standard.Id = DataUtil.GenerateUniqueId();
            await _standardRepository.AddAsync(standard);

            return standard.Id;
        }
    }
}
