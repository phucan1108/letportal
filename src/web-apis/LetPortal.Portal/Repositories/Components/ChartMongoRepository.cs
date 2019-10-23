using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components;

namespace LetPortal.Portal.Repositories.Components
{
    public class ChartMongoRepository : MongoGenericRepository<Chart>, IChartRepository
    {
        public ChartMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
