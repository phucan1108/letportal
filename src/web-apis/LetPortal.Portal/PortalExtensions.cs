using System;
using System.Text;
using System.Threading.Tasks;
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
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.EntitySchemas;
using LetPortal.Portal.Providers.Files;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories;
using LetPortal.Portal.Repositories.Apps;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Repositories.Datasources;
using LetPortal.Portal.Repositories.EntitySchemas;
using LetPortal.Portal.Repositories.Files;
using LetPortal.Portal.Repositories.Pages;
using LetPortal.Portal.Repositories.Recoveries;
using LetPortal.Portal.Services.Components;
using LetPortal.Portal.Services.Databases;
using LetPortal.Portal.Services.Datasources;
using LetPortal.Portal.Services.EntitySchemas;
using LetPortal.Portal.Services.Files;
using LetPortal.Portal.Services.Files.Validators;
using LetPortal.Portal.Services.Http;
using LetPortal.Portal.Services.Recoveries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            if (builder.ConnectionType == ConnectionType.MongoDB)
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
                builder.Services.AddSingleton<IChartRepository, ChartMongoRepository>();
                builder.Services.AddSingleton<IBackupRepository, BackupMongoRepository>();
                builder.Services.AddSingleton<IVersionRepository, VersionMongoRepository>();

                builder.Services.AddSingleton<IExecutionChartReport, MongoExecutionChartReport>();
                builder.Services.AddSingleton<IMongoQueryExecution, MongoQueryExecution>();
            }

            if (builder.ConnectionType == ConnectionType.SQLServer
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
                builder.Services.AddTransient<IFileRepository, FileEFRepository>();
                builder.Services.AddTransient<IChartRepository, ChartEFRepository>();
                builder.Services.AddTransient<IBackupRepository, BackupEFRepository>();
                builder.Services.AddTransient<IVersionRepository, VersionEFRepository>();
            }

            if (portalOptions.EnableFileServer)
            {
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

            if (builder.ConnectionType == ConnectionType.MongoDB)
            {
                builder.Services.AddTransient<IExecutionDatabase, MongoExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, MongoExtractionDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, MongoExtractionDatasource>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, MongoDynamicListQueryDatabase>();
                builder.Services.AddTransient<IAnalyzeDatabase, MongoAnalyzeDatabase>();
            }
            else if (builder.ConnectionType == ConnectionType.PostgreSQL)
            {
                builder.Services.AddTransient<IAnalyzeDatabase, PostgreAnalyzeDatabase>();
                builder.Services.AddTransient<IExecutionDatabase, PostgreExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, PostgreExtractionDatabase>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, PostgreDynamicListQueryDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, PostgreExtractionDatasource>();
                builder.Services.AddTransient<IExecutionChartReport, PostgreExecutionChartReport>();
                builder.Services.AddTransient<IExtractionChartQuery, PostgreExtractionChartQuery>();

                builder.Services.AddSingleton<IPostgreSqlMapper, PostgreSqlMapper>();
            }
            else if (builder.ConnectionType == ConnectionType.SQLServer)
            {
                builder.Services.AddTransient<IAnalyzeDatabase, SqlServerAnalyzeDatabase>();
                builder.Services.AddTransient<IExecutionDatabase, SqlServerExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, SqlServerExtractionDatabase>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, SqlServerDynamicListQueryDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, SqlServerExtractionDatasource>();
                builder.Services.AddTransient<IExecutionChartReport, SqlServerExecutionChartReport>();
                builder.Services.AddTransient<IExtractionChartQuery, SqlServerExtractionChartQuery>();

                builder.Services.AddSingleton<ISqlServerMapper, SqlServerMapper>();
            }
            else if (builder.ConnectionType == ConnectionType.MySQL)
            {
                builder.Services.AddTransient<IAnalyzeDatabase, MySqlAnalyzeDatabase>();
                builder.Services.AddTransient<IExecutionDatabase, MySqlExecutionDatabase>();
                builder.Services.AddTransient<IExtractionDatabase, MySqlExtractionDatabase>();
                builder.Services.AddTransient<IDynamicListQueryDatabase, MySqlDynamicListQueryDatabase>();
                builder.Services.AddTransient<IExtractionDatasource, MySqlExtractionDatasource>();
                builder.Services.AddTransient<IExecutionChartReport, MySqlExecutionChartReport>();
                builder.Services.AddTransient<IExtractionChartQuery, MySqlExtractionChartQuery>();

                builder.Services.AddSingleton<IMySqlMapper, MySqlMapper>();
            }
            builder.Services.AddSingleton<ICSharpMapper, CSharpMapper>();

            builder.Services.AddTransient<IDatabaseServiceProvider, InternalDatabaseServiceProvider>();
            builder.Services.AddTransient<IEntitySchemaServiceProvider, InternalEntitySchemaServiceProvider>();
            builder.Services.AddTransient<IPageServiceProvider, InternalPageServiceProvider>();
            builder.Services.AddTransient<IAppServiceProvider, InternalAppServiceProvider>();
            builder.Services.AddTransient<IStandardServiceProvider, InternalStandardServiceProvider>();
            builder.Services.AddTransient<IChartServiceProvider, InternalChartServiceProvider>();
            builder.Services.AddTransient<IDynamicListServiceProvider, InternalDynamicListServiceProvider>();
            builder.Services.AddTransient<IFileSeviceProvider, InternalFileServiceProvider>();

            builder.Services.AddTransient<IDynamicQueryBuilder, DynamicQueryBuilder>();
            builder.Services.AddTransient<IChartReportProjection, ChartReportProjection>();
            builder.Services.AddTransient<IChartReportQueryBuilder, ChartReportQueryBuilder>();

            builder.Services.AddTransient<IEntitySchemaService, EntitySchemaService>();
            builder.Services.AddTransient<IDynamicListService, DynamicListService>();
            builder.Services.AddTransient<IDatasourceService, DatasourceService>();
            builder.Services.AddTransient<IDatabaseService, DatabaseService>();
            builder.Services.AddTransient<IFileService, FileService>();
            builder.Services.AddTransient<IChartService, ChartService>();
            builder.Services.AddTransient<IBackupService, BackupService>();

            builder.Services.AddHttpClient<IHttpService, HttpService>();


            var jwtOptions = builder.Configuration.GetSection("JwtBearerOptions").Get<Core.Configurations.JwtBearerOptions>();
            builder.Services
                .AddAuthentication(
                 x =>
                 {
                     x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                     x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 })
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        NameClaimType = "sub",
                        // Important for testing purpose with zero but in production, it should be 5m (default)
                        ClockSkew =
                        Environment
                            .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ?
                            TimeSpan.Zero : TimeSpan.FromMinutes(5)
                    };

                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("X-Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            return builder;
        }
    }
}
