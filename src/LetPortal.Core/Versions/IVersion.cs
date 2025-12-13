using System.Threading.Tasks;

namespace LetPortal.Core.Versions
{
    public interface IVersion
    {
        string VersionNumber { get; }

        Task Upgrade(IVersionContext versionContext);

        Task Downgrade(IVersionContext versionContext);
    }
}
