using LetPortal.Core.Persistences;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Repositories.Datasources;
using LetPortal.Portal.Repositories.EntitySchemas;
using LetPortal.Portal.Repositories.Files;
using LetPortal.Portal.Repositories.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Portal.Repositories
{
    public static class RepositoriesRegistry
    {
        public static void AddPortalRepositories(this IServiceCollection serviceCollection, ConnectionType connectionType = ConnectionType.MongoDB)
        {
            // We will change these repos to another based on DB type, Default is MongoDB
            serviceCollection.AddSingleton<IDatabaseRepository, DatabaseMongoRepository>();
            serviceCollection.AddSingleton<IDatasourceRepository, DatasourceMongoRepository>();
            serviceCollection.AddSingleton<IEntitySchemaRepository, EntitySchemaMongoRepository>();            
            serviceCollection.AddSingleton<IAppRepository, AppMongoRepository>();
            serviceCollection.AddSingleton<IAppVersionRepository, AppVersionMongoRepository>();
            serviceCollection.AddSingleton<IPageRepository, PageMongoRepository>();
            serviceCollection.AddSingleton<IDynamicListRepository, DynamicListMongoRepository>();
            serviceCollection.AddSingleton<IStandardRepository, StandardMongoRepository>();
            serviceCollection.AddSingleton<IFileRepository, FileMongoRepository>();
        }
    }
}
