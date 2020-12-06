import * as decode from 'jwt-decode';
 
import { RolePortalClaimModel } from 'services/identity.service';
import { ObjectUtils } from '../utils/object-util';

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
        return new AuthUser(
            this.jsonTokenPayload.id, 
            this.jsonTokenPayload.sub, 
            this.jsonTokenPayload.roles,
            this.jsonTokenPayload.given_name,
            this.jsonTokenPayload.picture, 
            this.jsonTokenPayload);
    }

    public isExpired(): boolean{
        const isExpired = this.expireDate < new Date()
        return isExpired
    }
}

export class AuthUser {

    claims: Array<RolePortalClaimModel> = []

    constructor(
        public userid: string, 
        public username: string, 
        public roles: string[], 
        public fullName: string,
        public avatar: string,
        private tokenPayload: any) { 
            if(!ObjectUtils.isNotNull(fullName)){
                this.fullName = this.username
            }
        }

    hasClaim(claimType: string, claimValue: string): boolean{
        const foundClaim = this.claims.find(claim => claim.name === claimType)
        if(foundClaim){
            return foundClaim.claims.indexOf(claimValue) > -1
        }
        return false
    }

    getClaimsPerPage(claimPageName: string){
        const claims = new Object();
        const foundClaim = this.claims.find(claim => claim.name === claimPageName)
        if(foundClaim){
            foundClaim.claims?.forEach(claim => {
                claims[claim] = true
            })
        }
        return claims
    }

    isInRole(roleName: string){
        return this.roles.indexOf(roleName) > -1
    }

    getShortName(){
        const splitted = this.fullName.split(' ')
        if(splitted.length >= 2){
            return splitted[0][0].toUpperCase() + splitted[1][0].toUpperCase()
        }
        else{
            return splitted[0][0].toUpperCase() + splitted[0][1].toUpperCase()
        }
    }

    getFirstName(){
        const splitted = this.fullName.split(' ')
        return splitted[0]
    }
}