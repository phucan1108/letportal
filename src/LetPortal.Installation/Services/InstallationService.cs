using System.Reflection;
using LetPortal.Core.Extensions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Identity;
using LetPortal.Installation.Models;
using LetPortal.Portal;
using LetPortal.Versions;
using CoreConstants = LetPortal.Core.Constants;

namespace LetPortal.Installation.Services;

public class InstallationService(
    IVersionRepository versionRepository,
    IPatchVersionRepository patchVersionRepository,
    IEnumerable<IPortalPatchFeatureTool> portalPatches,
    IEnumerable<IIdentityPatchFeatureTool> identityPatches,
    IServiceProvider serviceProvider) : IInstallationService
{
    public async Task<CheckedInstallationResult> CheckInstallation()
    {
        var latestPortalAppVersion = await versionRepository.GetLastestVersion(CoreConstants.PortalApp);
        var latestIdentityAppVersion = await versionRepository.GetLastestVersion(CoreConstants.IdentityApp);

        if (latestPortalAppVersion == null && latestIdentityAppVersion == null)
        {
            return CheckedInstallationResult.NotInstalled;
        }

        var installedAppVersions = new List<InstalledAppVersion>();

        if (latestPortalAppVersion != null)
        {
            var availablePortalVersions = Scanner.GetAllPortalVersions(serviceProvider);
            var latestAvailablePortal = availablePortalVersions
                .OrderByDescending(v => v.VersionNumber.ToVersionNumber())
                .FirstOrDefault();

            installedAppVersions.Add(new InstalledAppVersion
            {
                Name = CoreConstants.PortalApp,
                CurrentVersion = latestPortalAppVersion.VersionNumber,
                AvailableVersion = latestAvailablePortal?.VersionNumber
            });
        }

        if (latestIdentityAppVersion != null)
        {
            var availableIdentityVersions = Scanner.GetAllIdentityVersions(serviceProvider);
            var latestAvailableIdentity = availableIdentityVersions
                .OrderByDescending(v => v.VersionNumber.ToVersionNumber())
                .FirstOrDefault();

            installedAppVersions.Add(new InstalledAppVersion
            {
                Name = CoreConstants.IdentityApp,
                CurrentVersion = latestIdentityAppVersion.VersionNumber,
                AvailableVersion = latestAvailableIdentity?.VersionNumber
            });
        }

        var canUpgrade = installedAppVersions.Any(v => 
            v.AvailableVersion != null && 
            v.AvailableVersion.ToVersionNumber() > v.CurrentVersion.ToVersionNumber());

        return new CheckedInstallationResult
        {
            Installed = true,
            CanUpgrade = canUpgrade,
            InstalledAppVersions = installedAppVersions
        };
    }

    public Task<AvailableVersionsResponse> GetAvailableVersions(string app)
    {
        var versions = GetVersionsForApp(app);
        var groupedVersions = versions
            .GroupBy(v => v.VersionNumber)
            .OrderBy(g => g.Key.ToVersionNumber())
            .Select(g => new VersionInfo
            {
                VersionNumber = g.Key,
                Components = g.Select(v => v.GetType().Name).ToList()
            })
            .ToList();

        return Task.FromResult(new AvailableVersionsResponse
        {
            App = app,
            AvailableVersions = groupedVersions
        });
    }

    public async Task<InstallationResult> Install(InstallRequest request)
    {
        try
        {
            var latestVersion = await versionRepository.GetLastestVersion(request.App);
            if (latestVersion != null)
            {
                return InstallationResult.Failed($"App '{request.App}' is already installed with version {latestVersion.VersionNumber}. Use upgrade instead.");
            }

            var versions = GetVersionsForApp(request.App);
            if (!versions.Any())
            {
                return InstallationResult.Failed($"No versions found for app '{request.App}'.");
            }

            var orderedVersions = versions.OrderBy(v => v.VersionNumber.ToVersionNumber());
            IEnumerable<IVersion> versionsToInstall;

            if (string.IsNullOrEmpty(request.VersionNumber))
            {
                versionsToInstall = orderedVersions;
            }
            else
            {
                var targetVersionNumber = request.VersionNumber.ToVersionNumber();
                versionsToInstall = orderedVersions.Where(v => v.VersionNumber.ToVersionNumber() <= targetVersionNumber);
            }

            if (!versionsToInstall.Any())
            {
                return InstallationResult.Failed($"No versions found matching version '{request.VersionNumber}'.");
            }

            var dbType = request.DatabaseType.ToEnum<ConnectionType>(true);
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = request.ConnectionString,
                ConnectionType = dbType
            };

            var versionContext = CreateVersionContext(dbType, databaseOptions);

            var executedSteps = new List<string>();
            var installedVersionNumbers = new Dictionary<string, List<string>>();

            foreach (var versionGroup in versionsToInstall.GroupBy(v => v.VersionNumber).OrderBy(g => g.Key.ToVersionNumber()))
            {
                var componentNames = new List<string>();
                foreach (var version in versionGroup)
                {
                    await version.Upgrade(versionContext);
                    var componentName = version.GetType().Name;
                    componentNames.Add(componentName);
                    executedSteps.Add($"Installed {componentName} v{version.VersionNumber}");
                }
                installedVersionNumbers[versionGroup.Key] = componentNames;
            }

            // Save version records
            foreach (var kvp in installedVersionNumbers)
            {
                await versionRepository.AddAsync(new Core.Versions.Version
                {
                    Id = DataUtil.GenerateUniqueId(),
                    AppName = request.App,
                    VersionNumber = kvp.Key,
                    AffectiveFiles = ConvertUtil.SerializeObject(kvp.Value),
                    Executor = "API",
                    CreatedDate = DateTime.UtcNow
                });
            }

            var installedVersion = versionsToInstall.Max(v => v.VersionNumber);
            return InstallationResult.Successful(installedVersion, executedSteps);
        }
        catch (Exception ex)
        {
            return InstallationResult.Failed($"Installation failed: {ex.Message}", [ex.ToString()]);
        }
    }

    public async Task<InstallationResult> Upgrade(UpgradeRequest request)
    {
        try
        {
            var latestVersion = await versionRepository.GetLastestVersion(request.App);
            if (latestVersion == null)
            {
                return InstallationResult.Failed($"App '{request.App}' is not installed. Use install instead.");
            }

            var currentVersionNumber = latestVersion.VersionNumber.ToVersionNumber();
            var targetVersionNumber = request.VersionNumber.ToVersionNumber();

            if (targetVersionNumber <= currentVersionNumber)
            {
                return InstallationResult.Failed($"Target version '{request.VersionNumber}' must be higher than current version '{latestVersion.VersionNumber}'.");
            }

            var versions = GetVersionsForApp(request.App);
            var versionsToUpgrade = versions
                .Where(v => v.VersionNumber.ToVersionNumber() > currentVersionNumber && 
                            v.VersionNumber.ToVersionNumber() <= targetVersionNumber)
                .OrderBy(v => v.VersionNumber.ToVersionNumber());

            if (!versionsToUpgrade.Any())
            {
                return InstallationResult.Failed($"No versions found between '{latestVersion.VersionNumber}' and '{request.VersionNumber}'.");
            }

            var dbType = request.DatabaseType.ToEnum<ConnectionType>(true);
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = request.ConnectionString,
                ConnectionType = dbType
            };

            var versionContext = CreateVersionContext(dbType, databaseOptions);

            var executedSteps = new List<string>();
            var upgradedVersionNumbers = new Dictionary<string, List<string>>();

            foreach (var versionGroup in versionsToUpgrade.GroupBy(v => v.VersionNumber).OrderBy(g => g.Key.ToVersionNumber()))
            {
                var componentNames = new List<string>();
                foreach (var version in versionGroup)
                {
                    await version.Upgrade(versionContext);
                    var componentName = version.GetType().Name;
                    componentNames.Add(componentName);
                    executedSteps.Add($"Upgraded {componentName} to v{version.VersionNumber}");
                }
                upgradedVersionNumbers[versionGroup.Key] = componentNames;
            }

            // Save version records
            foreach (var kvp in upgradedVersionNumbers)
            {
                await versionRepository.AddAsync(new Core.Versions.Version
                {
                    Id = DataUtil.GenerateUniqueId(),
                    AppName = request.App,
                    VersionNumber = kvp.Key,
                    AffectiveFiles = ConvertUtil.SerializeObject(kvp.Value),
                    Executor = "API",
                    CreatedDate = DateTime.UtcNow
                });
            }

            return InstallationResult.Successful(request.VersionNumber, executedSteps);
        }
        catch (Exception ex)
        {
            return InstallationResult.Failed($"Upgrade failed: {ex.Message}", [ex.ToString()]);
        }
    }

    public async Task<InstallationResult> Uninstall(UninstallRequest request)
    {
        try
        {
            var latestVersion = await versionRepository.GetLastestVersion(request.App);
            if (latestVersion == null)
            {
                return InstallationResult.Failed($"App '{request.App}' is not installed.");
            }

            var versions = GetVersionsForApp(request.App);
            var versionsToUninstall = versions
                .Where(v => v.VersionNumber.ToVersionNumber() <= latestVersion.VersionNumber.ToVersionNumber())
                .OrderByDescending(v => v.VersionNumber.ToVersionNumber());

            var dbType = request.DatabaseType.ToEnum<ConnectionType>(true);
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = request.ConnectionString,
                ConnectionType = dbType
            };

            var versionContext = CreateVersionContext(dbType, databaseOptions);

            var executedSteps = new List<string>();

            foreach (var versionGroup in versionsToUninstall.GroupBy(v => v.VersionNumber).OrderByDescending(g => g.Key.ToVersionNumber()))
            {
                foreach (var version in versionGroup)
                {
                    await version.Downgrade(versionContext);
                    executedSteps.Add($"Uninstalled {version.GetType().Name} v{version.VersionNumber}");
                }
            }

            // Remove all version records for this app
            var allVersions = await versionRepository.GetAllAsync(isRequiredDiscriminator: false);
            foreach (var version in allVersions.Where(v => v.AppName == request.App))
            {
                await versionRepository.DeleteAsync(version.Id);
            }

            return InstallationResult.Successful(latestVersion.VersionNumber, executedSteps);
        }
        catch (Exception ex)
        {
            return InstallationResult.Failed($"Uninstall failed: {ex.Message}", [ex.ToString()]);
        }
    }

    public async Task<AvailablePatchesResponse> GetAvailablePatches(string app)
    {
        var patches = new List<PatchInfo>();

        if (app.Equals(CoreConstants.PortalApp, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var patchTool in portalPatches)
            {
                var patchInfo = await GetPatchInfoAsync(app, patchTool.PatchName, patchTool.GetAllVersions(serviceProvider));
                patches.Add(patchInfo);
            }
        }
        else if (app.Equals(CoreConstants.IdentityApp, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var patchTool in identityPatches)
            {
                var patchInfo = await GetPatchInfoAsync(app, patchTool.PatchName, patchTool.GetAllVersions(serviceProvider));
                patches.Add(patchInfo);
            }
        }

        return new AvailablePatchesResponse
        {
            App = app,
            Patches = patches
        };
    }

    private async Task<PatchInfo> GetPatchInfoAsync(string app, string patchName, IEnumerable<IVersion> allVersions)
    {
        var latestPatchVersion = await patchVersionRepository.GetLatestAsync(app, patchName);
        var latestAvailableVersion = allVersions
            .OrderByDescending(v => v.VersionNumber.ToVersionNumber())
            .FirstOrDefault()?.VersionNumber;

        return new PatchInfo
        {
            PatchName = patchName,
            CurrentVersion = latestPatchVersion?.VersionNumber,
            LatestVersion = latestAvailableVersion,
            IsInstalled = latestPatchVersion != null,
            CanUpgrade = latestPatchVersion != null &&
                         latestAvailableVersion != null &&
                         latestAvailableVersion.ToVersionNumber() > latestPatchVersion.VersionNumber.ToVersionNumber()
        };
    }

    public async Task<PatchVersionsResponse> GetPatchVersions(string app, string patchName)
    {
        IEnumerable<IVersion>? allVersions = null;

        if (app.Equals(CoreConstants.PortalApp, StringComparison.OrdinalIgnoreCase))
        {
            var patchTool = portalPatches.FirstOrDefault(p => p.PatchName == patchName);
            allVersions = patchTool?.GetAllVersions(serviceProvider);
        }
        else if (app.Equals(CoreConstants.IdentityApp, StringComparison.OrdinalIgnoreCase))
        {
            var patchTool = identityPatches.FirstOrDefault(p => p.PatchName == patchName);
            allVersions = patchTool?.GetAllVersions(serviceProvider);
        }

        if (allVersions == null)
        {
            return new PatchVersionsResponse
            {
                App = app,
                PatchName = patchName,
                AvailableVersions = []
            };
        }

        var latestPatchVersion = await patchVersionRepository.GetLatestAsync(app, patchName);

        var groupedVersions = allVersions
            .GroupBy(v => v.VersionNumber)
            .OrderBy(g => g.Key.ToVersionNumber())
            .Select(g => new VersionInfo
            {
                VersionNumber = g.Key,
                IsInstalled = latestPatchVersion != null &&
                              g.Key.ToVersionNumber() <= latestPatchVersion.VersionNumber.ToVersionNumber(),
                Components = g.Select(v => v.GetType().Name).ToList()
            })
            .ToList();

        return new PatchVersionsResponse
        {
            App = app,
            PatchName = patchName,
            CurrentVersion = latestPatchVersion?.VersionNumber,
            AvailableVersions = groupedVersions
        };
    }

    public async Task<InstallationResult> InstallPatch(InstallPatchRequest request)
    {
        try
        {
            var latestPatchVersion = await patchVersionRepository.GetLatestAsync(request.App, request.PatchName);
            if (latestPatchVersion != null)
            {
                return InstallationResult.Failed($"Patch '{request.PatchName}' for app '{request.App}' is already installed with version {latestPatchVersion.VersionNumber}.");
            }

            IEnumerable<IVersion>? versions = null;

            if (request.App.Equals(CoreConstants.PortalApp, StringComparison.OrdinalIgnoreCase))
            {
                var patchTool = portalPatches.FirstOrDefault(p => p.PatchName == request.PatchName);
                versions = patchTool?.GetAllVersions(serviceProvider);
            }
            else if (request.App.Equals(CoreConstants.IdentityApp, StringComparison.OrdinalIgnoreCase))
            {
                var patchTool = identityPatches.FirstOrDefault(p => p.PatchName == request.PatchName);
                versions = patchTool?.GetAllVersions(serviceProvider);
            }

            if (versions == null || !versions.Any())
            {
                return InstallationResult.Failed($"Patch '{request.PatchName}' not found for app '{request.App}'.");
            }

            var orderedVersions = versions.OrderBy(v => v.VersionNumber.ToVersionNumber());
            IEnumerable<IVersion> versionsToInstall;

            if (string.IsNullOrEmpty(request.VersionNumber))
            {
                versionsToInstall = orderedVersions;
            }
            else
            {
                var targetVersionNumber = request.VersionNumber.ToVersionNumber();
                versionsToInstall = orderedVersions.Where(v => v.VersionNumber.ToVersionNumber() <= targetVersionNumber);
            }

            if (!versionsToInstall.Any())
            {
                return InstallationResult.Failed($"No versions found matching version '{request.VersionNumber}'.");
            }

            var dbType = request.DatabaseType.ToEnum<ConnectionType>(true);
            var databaseOptions = new DatabaseOptions
            {
                ConnectionString = request.ConnectionString,
                ConnectionType = dbType
            };

            var versionContext = CreateVersionContext(dbType, databaseOptions);

            var executedSteps = new List<string>();
            var installedVersionNumbers = new Dictionary<string, List<string>>();

            foreach (var versionGroup in versionsToInstall.GroupBy(v => v.VersionNumber).OrderBy(g => g.Key.ToVersionNumber()))
            {
                var componentNames = new List<string>();
                foreach (var version in versionGroup)
                {
                    await version.Upgrade(versionContext);
                    var componentName = version.GetType().Name;
                    componentNames.Add(componentName);
                    executedSteps.Add($"Installed patch {componentName} v{version.VersionNumber}");
                }
                installedVersionNumbers[versionGroup.Key] = componentNames;
            }

            // Save patch version records
            foreach (var kvp in installedVersionNumbers)
            {
                await patchVersionRepository.AddAsync(new PatchVersion
                {
                    Id = DataUtil.GenerateUniqueId(),
                    AppName = request.App,
                    PatchName = request.PatchName,
                    VersionNumber = kvp.Key,
                    AffectiveFiles = ConvertUtil.SerializeObject(kvp.Value),
                    Executor = "API",
                    CreatedDate = DateTime.UtcNow
                });
            }

            var installedVersion = versionsToInstall.Max(v => v.VersionNumber);
            return InstallationResult.Successful(installedVersion, executedSteps);
        }
        catch (Exception ex)
        {
            return InstallationResult.Failed($"Patch installation failed: {ex.Message}", [ex.ToString()]);
        }
    }

    private IEnumerable<IVersion> GetVersionsForApp(string app)
    {
        return app.ToLowerInvariant() switch
        {
            CoreConstants.PortalApp => Scanner.GetAllPortalVersions(serviceProvider),
            CoreConstants.IdentityApp => Scanner.GetAllIdentityVersions(serviceProvider),
            _ => []
        };
    }

    private static IVersionContext CreateVersionContext(ConnectionType dbType, DatabaseOptions databaseOptions)
    {
        return dbType switch
        {
            ConnectionType.MongoDB => new MongoVersionContext(databaseOptions)
            {
                ConnectionType = dbType,
                DatabaseOptions = databaseOptions
            },
            _ => throw new NotSupportedException($"Database type '{dbType}' is not supported yet for API installation.")
        };
    }
}
