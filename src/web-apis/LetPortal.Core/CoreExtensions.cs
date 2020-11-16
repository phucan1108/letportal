using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Https;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Core.Persistences;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace LetPortal.Core
{
    public static class CoreExtensions
    {
        private const string PORTAL_CORS = "LETPortalCors";

        public static IDatabaseOptionsBuilder AddDatabaseOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"));
            var databaseOptions = configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();
            var databaseOptionsBuilder = new DatabaseOptionsBuilder(services, configuration, databaseOptions);
            services.AddSingleton(databaseOptions);
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                ConventionPackDefault.Register();
                services.AddSingleton<MongoConnection>();
            }
            return databaseOptionsBuilder;
        }


        public static ILetPortalBuilder AddLetPortal(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            Action<LetPortalOptions> action = null)
        {
            var letPortalOptions = new LetPortalOptions();
            if (action != null)
            {
                action.Invoke(letPortalOptions);
            }
            var corsOptions = configuration.GetSection("CorsPortalOptions").Get<Core.Configurations.CorsPortalOptions>();
            var builder = new LetPortalBuilder(
                serviceCollection,
                configuration,
                letPortalOptions,
                corsOptions);

            return builder;
        }

        public static ILetPortalBuilder AddJwtValidator(this ILetPortalBuilder builder, Configurations.JwtBearerOptions jwtOptions = null, List<string> hubSegments = null)
        {
            if (jwtOptions == null)
            {
                builder.Services.Configure<Core.Configurations.JwtBearerOptions>(builder.Configuration.GetSection("JwtBearerOptions"));
                jwtOptions = builder.Configuration.GetSection("JwtBearerOptions").Get<Core.Configurations.JwtBearerOptions>();
            }
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = Environment
                            .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
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
                    OnMessageReceived = context =>
                    {
                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (hubSegments != null && hubSegments.Any(url => (path.StartsWithSegments(url, StringComparison.OrdinalIgnoreCase))))
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                        }
                        else if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                        {

                            // Keep read from Authorization header
                            var authToken = context.HttpContext.Request.GetJwtToken();
                            context.Token = authToken.RawData;
                        }

                        return Task.CompletedTask;
                    },
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


        public static bool IsLocalEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("Local");
        }

        public static bool IsDockerEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("Docker");
        }

        public static bool IsDockerLocalEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("DockerLocal");
        }


    }
}
