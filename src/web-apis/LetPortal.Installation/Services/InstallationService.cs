using LetPortal.Core;
using LetPortal.Core.Versions;
using LetPortal.Installation.Models;

namespace LetPortal.Installation.Services;

public class InstallationService(IVersionRepository versionRepository) : IInstallationService
{
    public async Task<CheckedInstallationResult> CheckInstallation()
    {
        var latestPortalAppVersion = await versionRepository.GetLastestVersion(Constants.PortalApp);
        var latestIdentityAppVersion = await versionRepository.GetLastestVersion(Constants.IdentityApp);

        if (latestPortalAppVersion == null || latestIdentityAppVersion == null)
        {
            return CheckedInstallationResult.NotInstalled;
        }

        var installedResult = new CheckedInstallationResult { Installed = true, InstalledAppVersions = new List<InstalledAppVersion>
        {
            new()
            {
                Name = Constants.PortalApp,
                CurrentVersion = latestPortalAppVersion.VersionNumber
            },
            new()
            {
                Name = Constants.IdentityApp,
                CurrentVersion = latestIdentityAppVersion.VersionNumber
            }
        }};
        return installedResult;
    }
}
