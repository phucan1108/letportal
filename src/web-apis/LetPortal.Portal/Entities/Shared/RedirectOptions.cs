namespace LetPortal.Portal.Entities.Shared
{
    public class RedirectOptions
    {
        public RedirectType RedirectType { get; set; }

        public string RedirectUrl { get; set; }

        public string TargetPageId { get; set; }

        public string PassParams { get; set; }

        public bool IsSameDomain { get; set; }
    }

    public enum RedirectType
    {
        ThroughPage,
        ThroughUrl
    }
}
