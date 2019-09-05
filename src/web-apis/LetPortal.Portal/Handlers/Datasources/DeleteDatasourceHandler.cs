using LetPortal.Portal.Handlers.Datasources.Requests;
using LetPortal.Portal.Repositories.Datasources;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources
{
    public class DeleteDatasourceHandler : RequestHandler<DeleteDatasourceRequest>
    {
        private readonly IDatasourceRepository _datasourceRepository;

        public DeleteDatasourceHandler(IDatasourceRepository datasourceRepository)
        {
            _datasourceRepository = datasourceRepository;
        }

        protected override void Handle(DeleteDatasourceRequest request)
        {
            _datasourceRepository.DeleteAsync(request.GetCommand().DatasourceId).Wait();
        }
    }
}
