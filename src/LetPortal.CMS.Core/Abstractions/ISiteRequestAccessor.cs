namespace LetPortal.CMS.Core.Abstractions
{
    public interface ISiteRequestAccessor
    {
        SiteContext Current { get; }
    }
}
