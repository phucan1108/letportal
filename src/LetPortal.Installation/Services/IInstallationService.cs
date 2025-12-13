using LetPortal.Installation.Models;

namespace LetPortal.Installation.Services;

public interface IInstallationService
{
    Task<CheckedInstallationResult> CheckInstallation();
}
