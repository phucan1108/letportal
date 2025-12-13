namespace LetPortal.Installation.Models;

public class UninstallRequest
{
    /// <summary>
    /// App name to uninstall (portal or identity)
    /// </summary>
    public string App { get; set; } = string.Empty;

    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database type (mongodb, sqlserver, postgresql, mysql)
    /// </summary>
    public string DatabaseType { get; set; } = "mongodb";
}
