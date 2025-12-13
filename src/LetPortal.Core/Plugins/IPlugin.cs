using System.Threading.Tasks;

namespace LetPortal.Core.Plugins;

public interface IPlugin<T> where T : PluginActionResult
{
    string Name { get; }

    string Version { get; }

    bool Connected { get; }
    
    Task<T> OnConnecting();

    Task<T> OnDisconnecting();
}
