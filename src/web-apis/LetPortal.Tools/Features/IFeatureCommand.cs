using System.Threading.Tasks;

namespace LetPortal.Tools.Features
{
    public interface IFeatureCommand
    {
        string CommandName { get; }

        Task RunAsync(ToolsContext context);
    }
}
