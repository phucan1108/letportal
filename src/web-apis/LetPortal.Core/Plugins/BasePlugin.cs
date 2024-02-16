using System.Threading.Tasks;

namespace LetPortal.Core.Plugins;

public abstract class BasePlugin : IPlugin<PluginActionResult>
{
    public string Name { get; }
    public string Version { get; }
    public bool Connected { get; }

    public  Task<PluginActionResult> OnConnecting()
    {
        throw new System.NotImplementedException();
    }

    public Task<PluginActionResult> OnDisconnecting()
    {
        throw new System.NotImplementedException();
    }
}
