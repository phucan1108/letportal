using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace LetPortal.Identity.Repositories
{
    public class LetPortalIdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }
        
        public DbSet<IssuedToken> IssuedTokens { get; set; }

        public DbSet<UserSession> UserSessions { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }

        private readonly DatabaseOptions _options;

        public LetPortalIdentityDbContext(DatabaseOptions options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userBuilder = modelBuilder.Entity<User>();
            userBuilder.HasKey(a => a.Id);

            var jsonRolesConverter = new ValueConverter<List<string>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<string>>(v));

            var jsonClaimsConverter = new ValueConverter<List<BaseClaim>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<BaseClaim>>(v));

            userBuilder.Property(a => a.Roles).HasConversion(jsonRolesConverter);
            userBuilder.Property(a => a.Claims).HasConversion(jsonClaimsConverter);

            var roleBuilder = modelBuilder.Entity<Role>();
            roleBuilder.HasKey(a => a.Id);

            roleBuilder.Property(a => a.Claims).HasConversion(jsonClaimsConverter);

            var issueTokenBuilder = modelBuilder.Entity<IssuedToken>();
            issueTokenBuilder.HasKey(a => a.Id);
            issueTokenBuilder.HasOne(a => a.User).WithOne();

            var userSessionBuilder = modelBuilder.Entity<UserSession>();
            userSessionBuilder.HasKey(a => a.Id);
            userSessionBuilder.HasMany(a => a.UserActivities).WithOne().OnDelete(DeleteBehavior.Cascade);            

            var userActivityBuilder = modelBuilder.Entity<UserActivity>();
            userActivityBuilder.HasKey(a => a.Id);

            foreach(var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach(var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = ToCamelCase(property.Relational().ColumnName);
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_options.ConnectionType == ConnectionType.SQLServer)
            {
                optionsBuilder.UseSqlServer(_options.ConnectionString);
            }
            else if(_options.ConnectionType == ConnectionType.PostgreSQL)
            {
                optionsBuilder.UseNpgsql(_options.ConnectionString);
            }
            else if(_options.ConnectionType == ConnectionType.MySQL)
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
