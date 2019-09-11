import * as decode from 'jwt-decode';
import * as _ from 'lodash';
import { RolePortalClaimModel } from 'services/identity.service';

export class AuthToken {
    public jsonTokenPayload: any;

    constructor(
        public jwtToken: string, 
        public expiresIn: number, 
        public refreshToken: string, 
        public expireRefresh: number) {
        if (jwtToken) {
            this.jsonTokenPayload = decode(jwtToken);
        }
    }

    public toAuthUser() {
        return new AuthUser(this.jsonTokenPayload.id, this.jsonTokenPayload.name, this.jsonTokenPayload.roles, this.jsonTokenPayload);
    }
}

export class AuthUser {

    claims: Array<RolePortalClaimModel> = []

    constructor(public userid: string, public username: string, private roles: string[], private tokenPayload: any) { }

    hasClaim(claimType: string, claimValue: string): boolean{
        const foundClaim = _.find(this.claims, claim => claim.name === claimType)
        if(foundClaim){
            return foundClaim.claims.indexOf(claimValue) > -1
        }
        return false
    }

    getClaimsPerPage(claimPageName: string){
        let claims = new Object();
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