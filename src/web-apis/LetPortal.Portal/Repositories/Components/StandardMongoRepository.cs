using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Portal.Repositories.Components
{
    public class StandardMongoRepository : MongoGenericRepository<StandardComponent>, IStandardRepository
    {
        public StandardMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
