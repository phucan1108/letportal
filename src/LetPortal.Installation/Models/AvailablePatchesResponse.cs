namespace LetPortal.Installation.Models;

public class AvailablePatchesResponse
{
    public string App { get; set; } = string.Empty;

    public IEnumerable<PatchInfo> Patches { get; set; } = [];
}

public class PatchInfo
{
    public string PatchName { get; set; } = string.Empty;

    public string? CurrentVersion { get; set; }

    public string? LatestVersion { get; set; }

    public bool IsInstalled { get; set; }

    public bool CanUpgrade { get; set; }
}

public class PatchVersionsResponse
{
    public string App { get; set; } = string.Empty;

    public string PatchName { get; set; } = string.Empty;

    public string? CurrentVersion { get; set; }

    public IEnumerable<VersionInfo> AvailableVersions { get; set; } = [];
}
