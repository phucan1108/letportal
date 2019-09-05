using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Models;
using LetPortal.Portal.Services.Databases;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Databases
{
    public class ExecuteDynamicHandler : IRequestHandler<ExecuteDynamicRequest, ExecuteDynamicResultModel>
    {
        private readonly IDatabaseService _databaseService;

        public ExecuteDynamicHandler(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<ExecuteDynamicResultModel> Handle(ExecuteDynamicRequest request, CancellationToken cancellationToken)
        {
            return await _databaseService.ExecuteDynamic(request.GetCommand().DatabaseId, request.GetCommand().FormattedCommand);
        }
    }
}
