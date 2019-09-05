using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.ServiceManagement.Repositories
{
    public class LogEventMongoRepository : MongoGenericRepository<LogEvent>, ILogEventRepository
    {
        public LogEventMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
