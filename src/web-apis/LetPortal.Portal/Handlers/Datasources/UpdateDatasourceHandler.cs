using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Repositories.Datasources;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources
{
    public class UpdateDatasourceHandler : RequestHandler<UpdateDatasourceRequest>
    {
        private readonly IDatasourceRepository _datasourceRepository;

        public UpdateDatasourceHandler(IDatasourceRepository datasourceRepository)
        {
            _datasourceRepository = datasourceRepository;
        }

        protected override void Handle(UpdateDatasourceRequest request)
        {
            _datasourceRepository.UpdateAsync(request.GetCommand().DatasourceId, request.GetCommand().Datasource).Wait();
        }
    }
}
