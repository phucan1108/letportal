using System.Collections;

namespace LetPortal.Core.Versions;

public interface IInstallationVersionScanner<T> where T : IVersion
{
    InstallationAppPackageInfo GetInfo();
}
