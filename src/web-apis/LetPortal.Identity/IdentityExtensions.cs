using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Identity.Configurations;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Persistences;
using LetPortal.Identity.Providers.Emails;
using LetPortal.Identity.Providers.Identity;
using LetPortal.Identity.Repositories;
using LetPortal.Identity.Repositories.Identity;
using LetPortal.Identity.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LetPortal.Identity
{
    public static class IdentityExtensions
    {
        public static ILetPortalBuilder AddIdentity(this ILetPortalBuilder builder)
        {               
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailOptions"));

            builder.Services.AddTransient<IIdentityServiceProvider, InternalIdentityServiceProvider>();
            builder.Services.AddSingleton<IEmailServiceProvider, EmailServiceProvider>();
            builder.Services.AddIdentity<User, Role>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()                
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password options
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;

                // User options
                options.User.RequireUniqueEmail = true;
                // Lockout options
                options.Lockout.AllowedForNewUsers = true;
            });

            return builder;
        }

        public static IDatabaseOptionsBuilder RegisterIdentityRepos(this IDatabaseOptionsBuilder builder)
        {
            builder.Services.RegisterRepos(builder.DatabaseOptions);

            return builder;
        }

        public static void RegisterRepos(
            this IServiceCollection services,
            DatabaseOptions databaseOptions)
        {
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                MongoDbRegistry.RegisterEntities();
                services.AddSingleton<IUserRepository, UserMongoRepository>();
                services.AddSingleton<IRoleRepository, RoleMongoRepository>();
                services.AddSingleton<IIssuedTokenRepository, IssuedTokenMongoRepository>();
                services.AddSingleton<IUserSessionRepository, UserSessionMongoRepository>();
                services.AddSingleton<IVersionRepository, VersionMongoRepository>();
            }

            if (databaseOptions.ConnectionType == ConnectionType.PostgreSQL
                || databaseOptions.ConnectionType == ConnectionType.MySQL
                || databaseOptions.ConnectionType == ConnectionType.SQLServer)
            {
                services.AddTransient<IdentityDbContext>();
                services.AddTransient<DbContext>((serviceProvider) =>
                {
                    return serviceProvider.GetService<IdentityDbContext>();
                });
                services.AddTransient<IUserRepository, UserEFRepository>();
                services.AddTransient<IRoleRepository, RoleEFRepository>();
                services.AddTransient<IIssuedTokenRepository, IssuedTokenEFRepository>();
                services.AddTransient<IUserSessionRepository, UserSessionEFRepository>();
                services.AddTransient<IVersionRepository, VersionEFRepository>();
            }
        }

        public static BaseClaim ToBaseClaim(this Claim claim)
        {
            return new BaseClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                ClaimValueType = claim.ValueType,
                Issuer = claim.Issuer
            };
        }

        public static Claim ToClaim(this BaseClaim baseClaim)
        {
            return new Claim(baseClaim.ClaimType, baseClaim.ClaimValue, baseClaim.ClaimValueType, baseClaim.Issuer);
        }
    }
}
