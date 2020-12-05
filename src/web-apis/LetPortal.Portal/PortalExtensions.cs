using System;
using LetPortal.Core;
using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Executions.Mongo;
using LetPortal.Portal.Executions.MySQL;
using LetPortal.Portal.Executions.PostgreSql;
using LetPortal.Portal.Executions.SqlServer;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Mappers.PostgreSql;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Options.Files;
using LetPortal.Portal.Options.Recoveries;
using LetPortal.Portal.Persistences;
using LetPortal.Portal.Providers.Apps;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.CompositeControls;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.EntitySchemas;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Localizations;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Repositories.Datasources;
using LetPortal.Portal.Repositories.EntitySchemas;
using LetPortal.Portal.Repositories.Files;
using LetPortal.Portal.Repositories.Localizations;
using LetPortal.Portal.Repositories.Pages;
using LetPortal.Portal.Repositories.Recoveries;
using LetPortal.Portal.Services.Apps;
using LetPortal.Portal.Services.Components;
using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Datasources;
using LetPortal.Portal.Services.EntitySchemas;
using LetPortal.Portal.Services.Files;
using LetPortal.Portal.Services.Files.Validators;
using LetPortal.Portal.Services.Http;
using LetPortal.Portal.Services.Recoveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Portal
{
    public static class PortalExtensions
    {
        public static ILetPortalBuilder AddPortalService(
            this ILetPortalBuilder builder,

            Action<PortalOptions> action = null)
        {
            var portalOptions = new PortalOptions();
            if (action != null)
            {
                action.Invoke(portalOptions);
            }

            builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
            builder.Services.Configure<MapperOptions>(builder.Configuration.GetSection("MapperOptions"));
            builder.Services.Configure<BackupOptions>(builder.Configuration.GetSection("BackupOptions"));
            var mapperOptions = builder.Configuration.GetSection("MapperOptions").Get<MapperOptions>();            
            builder.Services.AddSingleton(mapperOptions);
            if (portalOptions.EnableFileServer)
            {
                builder.Services.Configure<FilePublishOptions>(builder.Configuration.GetSection("FilePublishOptions"));
                builder.Services.Configure<FileOptions>(builder.Configuration.GetSection("FileOptions"));
                builder.Services.Configure<FileValidatorOptions>(builder.Configuration.GetSection("FileOptions").GetSection("FileValidatorOptions"));
                builder.Services.Configure<DiskStorageOptions>(builder.Configuration.GetSection("FileOptions").GetSection("DiskStorageOptions"));
                builder.Services.Configure<DatabaseStorageOptions>(builder.Configuration.GetSection("FileOptions").GetSection("DatabaseStorageOptions"));

                builder.Services.AddTransient<IFileConnectorExecution, DiskFileConnectorExecution>();
                builder.Services.AddTransient<IFileConnectorExecution, DatabaseFileConnectorExecution>();
                builder.Services.AddTransient<IFileValidatorRule, CheckFileExtensionRule>();
                builder.Services.AddTransient<IFileValidatorRule, CheckFileSizeRule>();
                builder.Services.AddTransient<IFileValidatorRule, CheckAllowedExtensionFileRule>();

                builder.Services.AddTransient<IStoreFileDatabase, MySqlStoreFileDatabase>();
                builder.Services.AddTransient<IStoreFileDatabase, SqlServerStoreFileDatabase>();
                builder.Services.AddTransient<IStoreFileDatabase, PostgreStoreFileDatabase>();
                builder.Services.AddTransient<IStoreFileDatabase, MongoStoreFileDatabase>();
            }
            builder.Services.AddSingleton<ICSharpMapper, CSharpMapper>();

            builder.Services.AddTransient<IDynamicQueryBuilder, DynamicQueryBuilder>();
            builder.Services.AddTransient<IChartReportProjection, ChartReportProjection>();
            builder.Services.AddTransient<IChartReportQueryBuilder, ChartReportQueryBuilder>();
            RegisterProviders(builder.Services);
            RegisterServices(builder.Services);

            return builder;
        }

        public static IDatabaseOptionsBuilder RegisterPortalRepos(this IDatabaseOptionsBuilder builder, bool skipMongoRegister = false)
        {
            builder.Services.RegisterRepos(builder.DatabaseOptions, skipMongoRegister);

            return builder;
        }

        public static void RegisterRepos(this IServiceCollection services, DatabaseOptions databaseOptions, bool skipMongoRegister = false)
        {
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                if (!skipMongoRegister)
                {
                    MongoDbRegistry.RegisterEntities();
                }

                // Register all mongo repositories
                services.AddSingleton<IDatabaseRepository, DatabaseMongoRepository>();
                services.AddSingleton<IDatasourceRepository, DatasourceMongoRepository>();
                services.AddSingleton<IEntitySchemaRepository, EntitySchemaMongoRepository>();
                services.AddSingleton<IAppRepository, AppMongoRepository>();
                services.AddSingleton<IAppVersionRepository, AppVersionMongoRepository>();
                services.AddSingleton<IPageRepository, PageMongoRepository>();
                services.AddSingleton<IDynamicListRepository, DynamicListMongoRepository>();
                services.AddSingleton<IStandardRepository, StandardMongoRepository>();
                services.AddSingleton<IFileRepository, FileMongoRepository>();
                services.AddSingleton<IChartRepository, ChartMongoRepository>();
                services.AddSingleton<IBackupRepository, BackupMongoRepository>();
                services.AddSingleton<IVersionRepository, VersionMongoRepository>();
                services.AddSingleton<ILocalizationRepository, LocalizationMongoRepository>();
                services.AddSingleton<ICompositeControlRepository, CompositeControlMongoRepository>();

                services.AddSingleton<IExecutionChartReport, MongoExecutionChartReport>();
                services.AddSingleton<IMongoQueryExecution, MongoQueryExecution>();
            }

            if (databaseOptions.ConnectionType == ConnectionType.SQLServer
                || databaseOptions.ConnectionType == ConnectionType.PostgreSQL
                || databaseOptions.ConnectionType == ConnectionType.MySQL)
            {
                services.AddTransient<PortalDbContext>();
                services.AddTransient<DbContext>((serviceProvider) =>
                {
                    return serviceProvider.GetService<PortalDbContext>();
                });
                // Register all EF repositories
                services.AddTransient<IDatabaseRepository, DatabaseEFRepository>();
                services.AddTransient<IDatasourceRepository, DatasourceEFRepository>();
                services.AddTransient<IEntitySchemaRepository, EntitySchemaEFRepository>();
                services.AddTransient<IAppRepository, AppEFRepository>();
                services.AddTransient<IPageRepository, PageEFRepository>();
                services.AddTransient<IDynamicListRepository, DynamicListEFRepository>();
                services.AddTransient<IStandardRepository, StandardEFRepository>();
                services.AddTransient<IFileRepository, FileEFRepository>();
                services.AddTransient<IChartRepository, ChartEFRepository>();
                services.AddTransient<IBackupRepository, BackupEFRepository>();
                services.AddTransient<IVersionRepository, VersionEFRepository>();
                services.AddTransient<ILocalizationRepository, LocalizationEFRepository>();
                services.AddSingleton<ICompositeControlRepository, CompositeControlEFRepository>();
            }

            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                services.AddTransient<IExecutionDatabase, MongoExecutionDatabase>();
                services.AddTransient<IExtractionDatabase, MongoExtractionDatabase>();
                services.AddTransient<IExtractionDatasource, MongoExtractionDatasource>();
                services.AddTransient<IDynamicListQueryDatabase, MongoDynamicListQueryDatabase>();
                services.AddTransient<IAnalyzeDatabase, MongoAnalyzeDatabase>();
            }
            else if (databaseOptions.ConnectionType == ConnectionType.PostgreSQL)
            {
                services.AddTransient<IAnalyzeDatabase, PostgreAnalyzeDatabase>();
                services.AddTransient<IExecutionDatabase, PostgreExecutionDatabase>();
                services.AddTransient<IExtractionDatabase, PostgreExtractionDatabase>();
                services.AddTransient<IDynamicListQueryDatabase, PostgreDynamicListQueryDatabase>();
                services.AddTransient<IExtractionDatasource, PostgreExtractionDatasource>();
                services.AddTransient<IExecutionChartReport, PostgreExecutionChartReport>();
                services.AddTransient<IExtractionChartQuery, PostgreExtractionChartQuery>();

                services.AddSingleton<IPostgreSqlMapper, PostgreSqlMapper>();
            }
            else if (databaseOptions.ConnectionType == ConnectionType.SQLServer)
            {
                services.AddTransient<IAnalyzeDatabase, SqlServerAnalyzeDatabase>();
                services.AddTransient<IExecutionDatabase, SqlServerExecutionDatabase>();
                services.AddTransient<IExtractionDatabase, SqlServerExtractionDatabase>();
                services.AddTransient<IDynamicListQueryDatabase, SqlServerDynamicListQueryDatabase>();
                services.AddTransient<IExtractionDatasource, SqlServerExtractionDatasource>();
                services.AddTransient<IExecutionChartReport, SqlServerExecutionChartReport>();
                services.AddTransient<IExtractionChartQuery, SqlServerExtractionChartQuery>();

                services.AddSingleton<ISqlServerMapper, SqlServerMapper>();
            }
            else if (databaseOptions.ConnectionType == ConnectionType.MySQL)
            {
                services.AddTransient<IAnalyzeDatabase, MySqlAnalyzeDatabase>();
                services.AddTransient<IExecutionDatabase, MySqlExecutionDatabase>();
                services.AddTransient<IExtractionDatabase, MySqlExtractionDatabase>();
                services.AddTransient<IDynamicListQueryDatabase, MySqlDynamicListQueryDatabase>();
                services.AddTransient<IExtractionDatasource, MySqlExtractionDatasource>();
                services.AddTransient<IExecutionChartReport, MySqlExecutionChartReport>();
                services.AddTransient<IExtractionChartQuery, MySqlExtractionChartQuery>();

                services.AddSingleton<IMySqlMapper, MySqlMapper>();
            }
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IEntitySchemaService, EntitySchemaService>();
            services.AddTransient<IDynamicListService, DynamicListService>();
            services.AddTransient<IDatasourceService, DatasourceService>();
            services.AddTransient<IDatabaseService, DatabaseService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IChartService, ChartService>();
            services.AddTransient<IBackupService, BackupService>();
            services.AddHttpClient<IHttpService, HttpService>();
            services.AddTransient<IAppService, AppService>();

        }

        public static void RegisterProviders(IServiceCollection services)
        {   
            services.AddTransient<IDatabaseServiceProvider, InternalDatabaseServiceProvider>();
            services.AddTransient<IEntitySchemaServiceProvider, InternalEntitySchemaServiceProvider>();
            services.AddTransient<IPageServiceProvider, InternalPageServiceProvider>();
            services.AddTransient<IAppServiceProvider, InternalAppServiceProvider>();
            services.AddTransient<IStandardServiceProvider, InternalStandardServiceProvider>();
            services.AddTransient<IChartServiceProvider, InternalChartServiceProvider>();
            services.AddTransient<IDynamicListServiceProvider, InternalDynamicListServiceProvider>();
            services.AddTransient<IFileSeviceProvider, InternalFileServiceProvider>();
            services.AddTransient<ILocalizationProvider, InternalLocalizationProvider>();
            services.AddTransient<ICompositeControlServiceProvider, InternalCompositeControlProvider>();
        }
    }
}
