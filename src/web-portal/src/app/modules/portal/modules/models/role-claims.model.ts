import { PortalClaim } from 'services/portal.service';

export interface ClaimNode {
    id: string
    level: number
    name: string
    checked: boolean
    expandable: boolean,
    parentId: string
}

export interface SelectablePortalClaim extends PortalClaim{
    allowSelected: boolean
    subClaims: SelectablePortalClaim[],
    parentId: string
}