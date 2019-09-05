namespace LetPortal.Core.Versions
{
    public interface IVersion
    {
        string VersionNumber { get; }

        void Upgrade(IVersionContext versionContext);

        void Downgrade(IVersionContext versionContext);
    }
}
