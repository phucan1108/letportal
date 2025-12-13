using System.Collections;
using System.Collections.Generic;

namespace LetPortal.Core.Versions;

public class InstallationAppPackageInfo
{
    public string Name { get; set; }

    public IEnumerable<IVersion> Versions { get; set; }
    
    public IEnumerable<AppDependency> Dependencies { get; set; }
}

public class AppDependency
{
    public string Name { get; set; }

    public string MinimumVersion { get; set; }

    public string MaximumVersion { get; set; }
}
