import{ PortalClaim, ClaimValueType } from 'services/portal.service'

export class PortalStandardClaims{
    public static AllowAccess: PortalClaim = {
        name: 'allowaccess',
        displayName: 'Allow Page Access',
        claimValueType: ClaimValueType.Boolean
    }
}