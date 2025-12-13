using LetPortal.Core;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using LetPortal.Identity;
using LetPortal.Installation.Services;
using LetPortal.Portal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LetPortal.Installation
{
    public static class Dependencies
    {
        public static ILetPortalBuilder AddInstallation(this ILetPortalBuilder builder)
        {
            // Register IPatchVersionRepository if not already registered
            var databaseOptions = builder.Configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();
            if (databaseOptions?.ConnectionType == ConnectionType.MongoDB)
            {
                builder.Services.TryAddSingleton<IPatchVersionRepository, PatchVersionMongoRepository>();
            }

            // Register empty collections for patch tools if not registered
            // This allows the service to work even when no patches are configured
            builder.Services.TryAddSingleton<IEnumerable<IPortalPatchFeatureTool>>(Array.Empty<IPortalPatchFeatureTool>());
            builder.Services.TryAddSingleton<IEnumerable<IIdentityPatchFeatureTool>>(Array.Empty<IIdentityPatchFeatureTool>());

            builder.Services.AddTransient<IInstallationService, InstallationService>();

            return builder;
        }
    }
}
