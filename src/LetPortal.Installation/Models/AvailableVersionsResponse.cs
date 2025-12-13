namespace LetPortal.Installation.Models;

public class AvailableVersionsResponse
{
    public string App { get; set; } = string.Empty;

    public string? CurrentVersion { get; set; }

    public IEnumerable<VersionInfo> AvailableVersions { get; set; } = [];
}

public class VersionInfo
{
    public string VersionNumber { get; set; } = string.Empty;

    public bool IsInstalled { get; set; }

    public bool CanUpgradeTo { get; set; }

    public IEnumerable<string> Components { get; set; } = [];
}
