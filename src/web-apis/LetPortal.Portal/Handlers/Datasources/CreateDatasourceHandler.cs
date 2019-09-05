using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Repositories.Datasources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Datasources
{
    public class CreateDatasourceHandler : IRequestHandler<CreateDatasourceRequest, Datasource>
    {
        private readonly IDatasourceRepository _datasourceRepository;

        public CreateDatasourceHandler(IDatasourceRepository datasourceRepository)
        {
            _datasourceRepository = datasourceRepository;
        }

        public async Task<Datasource> Handle(CreateDatasourceRequest request, CancellationToken cancellationToken)
        {
            Datasource datasource = request.GetCommand().Datasource;

            datasource.Id = DataUtil.GenerateUniqueId();

            await _datasourceRepository.AddAsync(datasource);

            return datasource;
        }
    }
}
