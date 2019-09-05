using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Repositories.Datasources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Datasources
{
    public class GetOneDatasourceHandler : IRequestHandler<GetOneDatasourceRequest, Datasource>
    {
        private readonly IDatasourceRepository _datasourceRepository;

        public GetOneDatasourceHandler(IDatasourceRepository datasourceRepository)
        {
            _datasourceRepository = datasourceRepository;
        }

        public async Task<Datasource> Handle(GetOneDatasourceRequest request, CancellationToken cancellationToken)
        {
            return await _datasourceRepository.GetOneAsync(request.GetQuery().DatasourceId);
        }
    }
}
