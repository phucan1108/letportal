namespace LetPortal.Installation.Models;

public class InstallRequest
{
    /// <summary>
    /// App name to install (portal or identity)
    /// </summary>
    public string App { get; set; } = string.Empty;

    /// <summary>
    /// Target version number to install (e.g., "0.9.0"). If null, installs latest version.
    /// </summary>
    public string? VersionNumber { get; set; }

    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database type (mongodb, sqlserver, postgresql, mysql)
    /// </summary>
    public string DatabaseType { get; set; } = "mongodb";
}
