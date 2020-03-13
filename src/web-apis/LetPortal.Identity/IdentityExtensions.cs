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
            builder.Services.Configure<Core.Configurations.JwtBearerOptions>(builder.Configuration.GetSection("JwtBearerOptions"));
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
                builder.Services.AddTransient<LetPortalIdentityDbContext>();
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

            var jwtOptions = builder.Configuration.GetSection("JwtBearerOptions").Get<Core.Configurations.JwtBearerOptions>();
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
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
                    // Be careful, if we use 'sub' as username, so we need to set http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
                    // Because .NET Core Identity will map sub -> nameidentifier
                    NameClaimType = ClaimTypes.NameIdentifier,
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
                        else
                        {
                            Console.WriteLine("There are some unexpected erros while trying to validate JWT token. Exception: " + context.Exception.ToString());
                        }
                        return Task.CompletedTask;
                    }
                };
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
