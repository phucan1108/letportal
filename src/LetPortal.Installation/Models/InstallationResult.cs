namespace LetPortal.Installation.Models;

public class InstallationResult
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public string? InstalledVersion { get; set; }

    public IEnumerable<string> ExecutedSteps { get; set; } = [];

    public IEnumerable<string> Errors { get; set; } = [];

    public static InstallationResult Successful(string version, IEnumerable<string> steps)
    {
        return new InstallationResult
        {
            Success = true,
            InstalledVersion = version,
            ExecutedSteps = steps,
            Message = $"Successfully installed version {version}"
        };
    }

    public static InstallationResult Failed(string message, IEnumerable<string>? errors = null)
    {
        return new InstallationResult
        {
            Success = false,
            Message = message,
            Errors = errors ?? []
        };
    }
}
