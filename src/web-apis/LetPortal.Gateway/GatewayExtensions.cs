using System;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LetPortal.Gateway
{
    public static class GatewayExtensions
    {
        public static readonly string AUTH_KEY = "LetPortal";

        public static ILetPortalBuilder AddGateway(this ILetPortalBuilder builder)
        {
            var jwtOptions = builder.Configuration.GetSection("JwtBearerOptions").Get<Core.Configurations.JwtBearerOptions>();
            builder.Services
                .AddAuthentication(
                 x =>
                 {
                     x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                     x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 })
                .AddJwtBearer(AUTH_KEY, x =>
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
                        NameClaimType = "name",
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
