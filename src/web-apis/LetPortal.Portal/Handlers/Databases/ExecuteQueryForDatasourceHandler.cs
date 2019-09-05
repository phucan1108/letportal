using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Models;
using LetPortal.Portal.Services.Databases;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Databases
{
    public class ExecuteQueryForDatasourceHandler : IRequestHandler<ExecuteQueryForDatasourceRequest, ExecuteDynamicResultModel>
    {
        private readonly IDatabaseService _databaseService;

        public ExecuteQueryForDatasourceHandler(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<ExecuteDynamicResultModel> Handle(ExecuteQueryForDatasourceRequest request, CancellationToken cancellationToken)
        {
            return await _databaseService.ExecuteDynamic(request.GetQuery().DatabaseId, request.GetQuery().Query);
        }
    }
}
