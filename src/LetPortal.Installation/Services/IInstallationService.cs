using LetPortal.Installation.Models;

namespace LetPortal.Installation.Services;

public interface IInstallationService
{
    Task<CheckedInstallationResult> CheckInstallation();

    Task<AvailableVersionsResponse> GetAvailableVersions(string app);

    Task<InstallationResult> Install(InstallRequest request);

    Task<InstallationResult> Upgrade(UpgradeRequest request);

    Task<InstallationResult> Uninstall(UninstallRequest request);

    // Patch operations
    Task<AvailablePatchesResponse> GetAvailablePatches(string app);

    Task<PatchVersionsResponse> GetPatchVersions(string app, string patchName);

    Task<InstallationResult> InstallPatch(InstallPatchRequest request);
}
