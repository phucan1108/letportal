using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Files;
using LetPortal.Portal.Services.Files.Validators;
using LetPortal.Portal.Services.Http;
using LetPortal.Core.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetPortal.Portal.Services.EntitySchemas;
using LetPortal.Portal.Services.Components;
using LetPortal.Portal.Services.Datasources;

namespace LetPortal.Portal.Services
{
    public static class AddServicesExtensions
    {
        public static void AddPortalServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
            serviceCollection.AddSingleton<IEntitySchemaService, EntitySchemaService>();
            serviceCollection.AddSingleton<IDynamicListService, DynamicListService>();
            serviceCollection.AddSingleton<IDatasourceService, DatasourceService>();

            serviceCollection.AddSingleton<IFileConnectorExecution, DiskFileConnectorExecution>();
            serviceCollection.AddSingleton<IFileConnectorExecution, DatabaseFileConnectorExecution>();
            serviceCollection.AddTransient<IFileValidatorRule, CheckFileExtensionRule>();
            serviceCollection.AddTransient<IFileValidatorRule, CheckFileSizeRule>();
            serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddTransient<HttpService>();
            serviceCollection.AddHttpClient<HttpService>();
        }
    }
}
