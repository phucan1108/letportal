import * as decode from 'jwt-decode';
import * as _ from 'lodash';
import { RolePortalClaimModel } from 'services/identity.service';

export class AuthToken {
    public jsonTokenPayload: any;
    public expireDate: Date
    public expireRefreshDate: Date
    constructor(
        public jwtToken: string,
        public expiresIn: number,
        public refreshToken: string,
        public expireRefresh: number) {
        if (jwtToken) {
            this.jsonTokenPayload = decode(jwtToken);
        }

        this.expireDate = new Date(0)
        this.expireDate.setUTCSeconds(expiresIn)

        this.expireRefreshDate = new Date(0)
        this.expireRefreshDate.setUTCSeconds(expireRefresh)
    }

    public toAuthUser() {
        return new AuthUser(this.jsonTokenPayload.id, this.jsonTokenPayload.sub, this.jsonTokenPayload.roles, this.jsonTokenPayload);
    }

    public isExpired(): boolean{
        const isExpired = this.expireDate < new Date()
        return isExpired
    }
}

export class AuthUser {

    claims: Array<RolePortalClaimModel> = []

    constructor(public userid: string, public username: string, public roles: string[], private tokenPayload: any) { }

    hasClaim(claimType: string, claimValue: string): boolean{
        const foundClaim = _.find(this.claims, claim => claim.name === claimType)
        if(foundClaim){
            return foundClaim.claims.indexOf(claimValue) > -1
        }
        return false
    }

    getClaimsPerPage(claimPageName: string){
        const claims = new Object();
        const foundClaim = _.find(this.claims, claim => claim.name === claimPageName)
        if(foundClaim){
            _.forEach(foundClaim.claims, claim => {
                claims[claim] = true
            })
        }
        return claims
    }

    isInRole(roleName: string){
        return this.roles.indexOf(roleName) > -1
    }
}