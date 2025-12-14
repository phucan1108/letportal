namespace LetPortal.Installation.Models;

public class UpgradeRequest
{
    /// <summary>
    /// App name to upgrade (portal or identity)
    /// </summary>
    public string App { get; set; } = string.Empty;

    /// <summary>
    /// Target version number to upgrade to (e.g., "0.9.0")
    /// </summary>
    public string VersionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database type (mongodb, sqlserver, postgresql, mysql)
    /// </summary>
    public string DatabaseType { get; set; } = "mongodb";
}
