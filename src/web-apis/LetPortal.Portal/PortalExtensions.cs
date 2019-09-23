using LetPortal.Core;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Options.Files;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.EntitySchemas;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Repositories.Datasources;
using LetPortal.Portal.Repositories.EntitySchemas;
using LetPortal.Portal.Repositories.Files;
using LetPortal.Portal.Repositories.Pages;
using LetPortal.Portal.Services.Components;
using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Datasources;
using LetPortal.Portal.Services.EntitySchemas;
using LetPortal.Portal.Services.Files;
using LetPortal.Portal.Services.Files.Validators;
using LetPortal.Portal.Services.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LetPortal.Portal
{
    public static class PortalExtensions
    {
        public static ILetPortalBuilder AddPortalService(
            this ILetPortalBuilder builder,
            Action<PortalOptions> action = null)
        {
            var portalOptions = new PortalOptions();
            if(action != null)
            {
                action.Invoke(portalOptions);
            }
            if(builder.ConnectionType == ConnectionType.MongoDB)
            {
                MongoDbRegistry.RegisterEntities();

                // Register all mongo repositories
                builder.Services.AddSingleton<IDatabaseRepository, DatabaseMongoRepository>();
                builder.Services.AddSingleton<IDatasourceRepository, DatasourceMongoRepository>();
                builder.Services.AddSingleton<IEntitySchemaRepository, EntitySchemaMongoRepository>();
                builder.Services.AddSingleton<IAppRepository, AppMongoRepository>();
                builder.Services.AddSingleton<IAppVersionRepository, AppVersionMongoRepository>();
                builder.Services.AddSingleton<IPageRepository, PageMongoRepository>();
                builder.Services.AddSingleton<IDynamicListRepository, DynamicListMongoRepository>();
                builder.Services.AddSingleton<IStandardRepository, StandardMongoRepository>();
                builder.Services.AddSingleton<IFileRepository, FileMongoRepository>();
            }

            if(builder.ConnectionType == ConnectionType.SQLServer
                || builder.ConnectionType == ConnectionType.PostgreSQL
                || builder.ConnectionType == ConnectionType.MySQL)
            {
                builder.Services.AddTransient<LetPortalDbContext>();
                // Register all EF repositories
                builder.Services.AddTransient<IDatabaseRepository, DatabaseEFRepository>();
                builder.Services.AddTransient<IDatasourceRepository, DatasourceEFRepository>();
                builder.Services.AddTransient<IEntitySchemaRepository, EntitySchemaEFRepository>();
                builder.Services.AddTransient<IAppRepository, AppEFRepository>();
                builder.Services.AddTransient<IPageRepository, PageEFRepository>();
                builder.Services.AddTransient<IDynamicListRepository, DynamicListEFRepository>();
                builder.Services.AddTransient<IStandardRepository, StandardEFRepository>();
                builder.Services.AddSingleton<IFileRepository, FileEFRepository>();
            }

            if(portalOptions.EnableFileServer)
            {
                builder.Services.Configure<FileOptions>(builder.Configuration.GetSection("FileOptions"));
                builder.Services.Configure<FileValidatorOptions>(builder.Configuration.GetSection("FileOptions").GetSection("FileValidatorOptions"));
                builder.Services.Configure<DiskStorageOptions>(builder.Configuration.GetSection("FileOptions").GetSection("DiskStorageOptions"));
                builder.Services.Configure<DatabaseStorageOptions>(builder.Configuration.GetSection("FileOptions").GetSection("DatabaseStorageOptions"));

                builder.Services.AddSingleton<IFileConnectorExecution, DiskFileConnectorExecution>();
                builder.Services.AddSingleton<IFileConnectorExecution, DatabaseFileConnectorExecution>();
                builder.Services.AddTransient<IFileValidatorRule, CheckFileExtensionRule>();
                builder.Services.AddTransient<IFileValidatorRule, CheckFileSizeRule>();
            }

            builder.Services.AddSingleton<IExecutionDatabase, MongoExecutionDatabase>();
            builder.Services.AddSingleton<IExtractionDatabase, MongoExtractionDatabase>();
            builder.Services.AddSingleton<IExtractionDatasource, MongoExtractionDatasource>();
            builder.Services.AddSingleton<IDynamicListQueryDatabase, MongoDynamicListQueryDatabase>();

            builder.Services.AddSingleton<IDatabaseServiceProvider, InternalDatabaseServiceProvider>();            
            builder.Services.AddSingleton<IEntitySchemaServiceProvider, InternalEntitySchemaServiceProvider>();
            builder.Services.AddSingleton<IPageServiceProvider, InternalPageServiceProvider>();

            builder.Services.AddSingleton<IEntitySchemaService, EntitySchemaService>();
            builder.Services.AddSingleton<IDynamicListService, DynamicListService>();
            builder.Services.AddSingleton<IDatasourceService, DatasourceService>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IFileService, FileService>();
            builder.Services.AddTransient<HttpService>();
            builder.Services.AddHttpClient<HttpService>();
            return builder;
        }
    }
}
