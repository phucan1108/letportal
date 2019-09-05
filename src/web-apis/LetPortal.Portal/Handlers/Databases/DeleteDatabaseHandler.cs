using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Repositories.Databases;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases
{
    public class DeleteDatabaseHandler : RequestHandler<DeleteDatabaseRequest>
    {
        private readonly IDatabaseRepository _databaseRepository;

        public DeleteDatabaseHandler(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        protected override void Handle(DeleteDatabaseRequest request)
        {
            _databaseRepository.DeleteAsync(request.GetCommand().Id).Wait();
        }
    }
}
