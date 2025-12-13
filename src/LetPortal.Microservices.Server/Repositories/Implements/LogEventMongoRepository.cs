using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Repositories.Implements
{
    public class LogEventMongoRepository : MongoGenericRepository<LogEvent>, ILogEventRepository
    {
        public LogEventMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
