using LetPortal.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        NameClaimType = "name"
                    };
                });

            return builder;
        }
    }
}
