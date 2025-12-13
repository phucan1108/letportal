namespace LetPortal.Core.Security
{
    public class PortalClaimStandards
    {
        public static readonly PortalClaim AllowAccess = new PortalClaim
        {
            Name = "allowaccess",
            DisplayName = "Allow Access",
            ClaimValueType = ClaimValueType.Boolean
        };
    }
}
