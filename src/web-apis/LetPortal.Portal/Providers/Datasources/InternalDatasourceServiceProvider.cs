using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Repositories.Datasources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Datasources
{
    public class InternalDatasourceServiceProvider : IDatasourceServiceProvider
    {
        private readonly IDatasourceRepository _datasourceRepository;

        public InternalDatasourceServiceProvider(IDatasourceRepository datasourceRepository)
        {
            _datasourceRepository = datasourceRepository;
        }
    }
}

