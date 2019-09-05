using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Datasources;
using LetPortal.Portal.Providers.EntitySchemas;
using LetPortal.Portal.Providers.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Portal.Providers
{
    public static class AddServiceProvidersExtensions
    {
        public static void AddServiceProviders(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDatabaseServiceProvider, InternalDatabaseServiceProvider>();
            serviceCollection.AddSingleton<IDatasourceServiceProvider, InternalDatasourceServiceProvider>();
            serviceCollection.AddSingleton<IEntitySchemaServiceProvider, InternalEntitySchemaServiceProvider>();
            serviceCollection.AddSingleton<IPageServiceProvider, InternalPageServiceProvider>();
        }
    }
}
