using System;

namespace LetPortal.Core.Plugins;

public class PluginActionResult
{
    public bool Success { get; set;  }

    public Exception Exception { get; set; }
}
