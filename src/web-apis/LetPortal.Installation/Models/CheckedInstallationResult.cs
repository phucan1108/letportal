namespace LetPortal.Installation.Models;

public class CheckedInstallationResult
{
    public bool Installed { get; set; }

    public bool CanUpgrade { get; set; }

    public IEnumerable<InstalledAppVersion> InstalledAppVersions { get; set; }

    public static readonly CheckedInstallationResult NotInstalled = new CheckedInstallationResult
    {
        Installed = false
    };
}

public class InstalledAppVersion
{
    public string Name { get; set; }

    public string CurrentVersion { get; set; }

    public string AvailableVersion { get; set; }
}

