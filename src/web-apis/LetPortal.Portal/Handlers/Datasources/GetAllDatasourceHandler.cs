using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Repositories.Datasources;
using MediatR;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Datasources
{
    public class GetAllDatasourceHandler : IRequestHandler<GetAllDatasourceRequest, List<Datasource>>
    {
        private readonly IDatasourceRepository _datasourceRepository;

        public GetAllDatasourceHandler(IDatasourceRepository datasourceRepository)
        {
            _datasourceRepository = datasourceRepository;
        }

        public Task<List<Datasource>> Handle(GetAllDatasourceRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_datasourceRepository.GetAsQueryable().ToList());
        }
    }
}
