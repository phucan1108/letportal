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

            if (builder.ConnectionType == ConnectionType.MongoDB)
            {
                MongoDbRegistry.RegisterEntities();
                builder.Services.AddSingleton<IUserRepository, UserMongoRepository>();
                builder.Services.AddSingleton<IRoleRepository, RoleMongoRepository>();
                builder.Services.AddSingleton<IIssuedTokenRepository, IssuedTokenMongoRepository>();
                builder.Services.AddSingleton<IUserSessionRepository, UserSessionMongoRepository>();
                builder.Services.AddSingleton<IVersionRepository, VersionMongoRepository>();
            }

            if (builder.ConnectionType == ConnectionType.PostgreSQL
                || builder.ConnectionType == ConnectionType.MySQL
                || builder.ConnectionType == ConnectionType.SQLServer)
            {
                builder.Services.AddTransient<IdentityDbContext>();
                builder.Services.AddTransient<IUserRepository, UserEFRepository>();
                builder.Services.AddTransient<IRoleRepository, RoleEFRepository>();
                builder.Services.AddTransient<IIssuedTokenRepository, IssuedTokenEFRepository>();
                builder.Services.AddTransient<IUserSessionRepository, UserSessionEFRepository>();
                builder.Services.AddTransient<IVersionRepository, VersionEFRepository>();
            }

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
