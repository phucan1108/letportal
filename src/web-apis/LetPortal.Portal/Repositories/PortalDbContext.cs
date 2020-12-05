using System;
using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Entities.Files;
using LetPortal.Portal.Entities.Localizations;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Recoveries;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Entities.SectionParts.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LetPortal.Portal.Repositories
{
    public class PortalDbContext : DbContext
    {
        public ConnectionType ConnectionType => _options.ConnectionType;
        public DbSet<App> Apps { get; set; }

        public DbSet<Component> Components { get; set; }

        public DbSet<DynamicList> DynamicLists { get; set; }

        public DbSet<StandardComponent> StandardComponents { get; set; }

        public DbSet<Chart> Charts { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<DatabaseConnection> Databases { get; set; }

        public DbSet<Datasource> Datasources { get; set; }

        public DbSet<EntitySchema> EntitySchemas { get; set; }

        public DbSet<Localization> Localizations { get; set; }

        public DbSet<LocalizationContent> LocalizationContents { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<LetPortal.Core.Versions.Version> Versions { get; set; }

        public DbSet<Backup> Backups { get; set; }

        public DbSet<CompositeControl> CompositeControls { get; set; }

        private readonly DatabaseOptions _options;

        public PortalDbContext(DatabaseOptions options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Declare all model builder
            var jsonMenusConverter = new ValueConverter<List<Menu>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<Menu>>(v));

            var jsonMenuProfilesConverter = new ValueConverter<List<MenuProfile>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<MenuProfile>>(v));

            // App 
            var appBuilder = modelBuilder.Entity<App>();
            appBuilder.HasKey(a => a.Id);
            appBuilder.Property(a => a.Menus).HasConversion(jsonMenusConverter);
            appBuilder.Property(a => a.MenuProfiles).HasConversion(jsonMenuProfilesConverter);

            // Portal Version
            var portalVersionBuilder = modelBuilder.Entity<LetPortal.Core.Versions.Version>();
            portalVersionBuilder.HasKey(a => a.Id);

            // Component
            var componentBuilder = modelBuilder.Entity<Component>();
            componentBuilder.HasKey(a => a.Id);

            if (_options.ConnectionType == ConnectionType.MySQL)
            {
                componentBuilder.Property(a => a.AllowOverrideOptions).HasColumnType("BIT");
                componentBuilder.Property(a => a.AllowPassingDatasource).HasColumnType("BIT");
            }

            var jsonShellOptionsConverter = new ValueConverter<List<ShellOption>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<ShellOption>>(v));
            componentBuilder.Property(a => a.Options).HasConversion(jsonShellOptionsConverter);

            // Dynamic List
            var dynamicListBuilder = modelBuilder.Entity<DynamicList>();
            dynamicListBuilder.HasBaseType<Component>();

            var jsonDynamicListDatasourceConverter = new ValueConverter<DynamicListDatasource, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<DynamicListDatasource>(v));
            var jsonParamsListConverter = new ValueConverter<ParamsList, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<ParamsList>(v));
            var jsonFiltersListConverter = new ValueConverter<FiltersList, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<FiltersList>(v));
            var jsonColumnsListConverter = new ValueConverter<ColumnsList, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<ColumnsList>(v));
            var jsonCommandsListConverter = new ValueConverter<CommandsList, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<CommandsList>(v));

            dynamicListBuilder.Property(a => a.ListDatasource).HasConversion(jsonDynamicListDatasourceConverter);
            dynamicListBuilder.Property(a => a.ParamsList).HasConversion(jsonParamsListConverter);
            dynamicListBuilder.Property(a => a.FiltersList).HasConversion(jsonFiltersListConverter);
            dynamicListBuilder.Property(a => a.ColumnsList).HasConversion(jsonColumnsListConverter);
            dynamicListBuilder.Property(a => a.CommandsList).HasConversion(jsonCommandsListConverter);

            // Standard
            var standardBuilder = modelBuilder.Entity<StandardComponent>();
            standardBuilder.HasBaseType<Component>();

            var jsonControlsConverter = new ValueConverter<List<PageControl>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<PageControl>>(v));
            standardBuilder.Property(a => a.Controls).HasConversion(jsonControlsConverter);

            // Chart
            var chartBuilder = modelBuilder.Entity<Chart>();
            chartBuilder.HasBaseType<Component>();

            var jsonChartDatabaseOptions = new ValueConverter<Entities.Shared.SharedDatabaseOptions, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<Entities.Shared.SharedDatabaseOptions>(v));
            chartBuilder.Property(a => a.DatabaseOptions).HasConversion(jsonChartDatabaseOptions);

            var jsonChartDefinitions = new ValueConverter<ChartDefinitions, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<ChartDefinitions>(v));
            chartBuilder.Property(a => a.Definitions).HasConversion(jsonChartDefinitions);

            var jsonChartFiltersConverter = new ValueConverter<List<ChartFilter>, string>(
               v => ConvertUtil.SerializeObject(v, true),
               v => ConvertUtil.DeserializeObject<List<ChartFilter>>(v));
            chartBuilder.Property(a => a.ChartFilters).HasConversion(jsonChartFiltersConverter);

            // Page
            var pageBuilder = modelBuilder.Entity<Page>();
            pageBuilder.HasKey(a => a.Id);

            var jsonPortalClaimConverter = new ValueConverter<List<PortalClaim>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<PortalClaim>>(v));

            var jsonPageBuilderConverter = new ValueConverter<PageBuilder, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<PageBuilder>(v));

            var jsonPageDatasourceConverter = new ValueConverter<List<PageDatasource>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<PageDatasource>>(v));

            var jsonPageEventConverter = new ValueConverter<List<PageEvent>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<PageEvent>>(v));

            var jsonPageButtonConverter = new ValueConverter<List<PageButton>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<PageButton>>(v));

            pageBuilder.Property(a => a.ShellOptions).HasConversion(jsonShellOptionsConverter);
            pageBuilder.Property(a => a.Claims).HasConversion(jsonPortalClaimConverter);
            pageBuilder.Property(a => a.Builder).HasConversion(jsonPageBuilderConverter);
            pageBuilder.Property(a => a.PageDatasources).HasConversion(jsonPageDatasourceConverter);
            pageBuilder.Property(a => a.Events).HasConversion(jsonPageEventConverter);
            pageBuilder.Property(a => a.Commands).HasConversion(jsonPageButtonConverter);

            // Database Connection
            var databaseConnectionBuilder = modelBuilder.Entity<DatabaseConnection>();
            databaseConnectionBuilder.HasKey(a => a.Id);

            var datasourceBuilder = modelBuilder.Entity<Datasource>();
            datasourceBuilder.HasKey(a => a.Id);
            if (_options.ConnectionType == ConnectionType.MySQL)
            {
                datasourceBuilder.Property(a => a.CanCache).HasColumnType("BIT");
            }

            // Entity Schema
            var entitySchemaBuilder = modelBuilder.Entity<EntitySchema>();
            entitySchemaBuilder.HasKey(a => a.Id);

            var jsonEntityFieldConverter = new ValueConverter<List<EntityField>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<EntityField>>(v));

            entitySchemaBuilder.Property(a => a.EntityFields).HasConversion(jsonEntityFieldConverter);

            // File
            var fileBuilder = modelBuilder.Entity<File>();
            fileBuilder.HasKey(a => a.Id);
            if (_options.ConnectionType == ConnectionType.MySQL)
            {
                fileBuilder.Property(a => a.AllowCompress).HasColumnType("BIT");
            }

            // Backup
            var backupBuilder = modelBuilder.Entity<Backup>();
            backupBuilder.HasKey(a => a.Id);
            var jsonBackupElementsConverter = new ValueConverter<BackupElements, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<BackupElements>(v));
            backupBuilder.Property(a => a.BackupElements).HasConversion(jsonBackupElementsConverter);

            // Localization
            var localizationBuilder = modelBuilder.Entity<Localization>();
            localizationBuilder.HasKey(a => a.Id);
            localizationBuilder.HasMany(a => a.LocalizationContents)
                               .WithOne(b => b.Localization)
                               .OnDelete(DeleteBehavior.Cascade);

            var localizationContentBuilder = modelBuilder.Entity<LocalizationContent>();
            localizationContentBuilder.HasKey(a => a.Id);

            // Composite control
            var compositeControlBuilder = modelBuilder.Entity<CompositeControl>();
            compositeControlBuilder.HasKey(a => a.Id);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToCamelCase(property.GetColumnName()));
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_options.ConnectionType == ConnectionType.SQLServer)
            {
                optionsBuilder.UseSqlServer(_options.ConnectionString);
            }
            else if (_options.ConnectionType == ConnectionType.PostgreSQL)
            {
                optionsBuilder.UseNpgsql(_options.ConnectionString);
            }
            else if (_options.ConnectionType == ConnectionType.MySQL)
            {
                optionsBuilder.UseMySql(_options.ConnectionString);
            }
        }

        private string ToCamelCase(string column)
        {
            return char.ToLowerInvariant(column[0]) + column.Substring(1);
        }
    }
}
