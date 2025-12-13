using System.Threading.Tasks;

namespace LetPortal.Core.Tools
{
    public interface IFeatureCommand
    {
        string CommandName { get; }

        Task RunAsync(object context);
    }
}
