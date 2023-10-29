using System;
using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LetPortal.Core.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static void MakeCamelName(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName(), null));
                    property.SetColumnName(ToCamelCase(columnName));
                }
            }
        }

        /// <summary>
        /// Depends on the database type, we will init the correct connection. For ex: Type == SQL, use EntityFramework.SqlServer
        /// </summary>
        public static void ConstructConnection(this DbContextOptionsBuilder optionsBuilder, 
            DatabaseOptions databaseOptions,
            bool enableDetailError = false)
        {
            if (databaseOptions.ConnectionType == ConnectionType.SQLServer)
            {
                optionsBuilder
                    .UseSqlServer(databaseOptions.ConnectionString)
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error)
                    .EnableDetailedErrors(enableDetailError);
            }
            else if (databaseOptions.ConnectionType == ConnectionType.PostgreSQL)
            {
                optionsBuilder
                    .UseNpgsql(databaseOptions.ConnectionString)
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error)
                    .EnableDetailedErrors(enableDetailError);
            }
            else if (databaseOptions.ConnectionType == ConnectionType.MySQL)
            {
                // New Changes: From Pomelo 6+, we need to declare Server Version
                var serverVersion = ServerVersion.AutoDetect(databaseOptions.ConnectionString);
                optionsBuilder
                    .UseMySql(databaseOptions.ConnectionString, serverVersion)
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error)
                    .EnableDetailedErrors(enableDetailError);
            }
        }

        private static string ToCamelCase(string column)
        {
            return char.ToLowerInvariant(column[0]) + column.Substring(1);
        }
    }
}
