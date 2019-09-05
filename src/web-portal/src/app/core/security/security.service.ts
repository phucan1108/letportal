import { Injectable } from '@angular/core';
import { AuthToken, AuthUser } from './auth.model';
import { SessionService } from 'services/session.service';
import { RolesClient, PortalClaimModel } from 'services/identity.service';
import { Observable, of, timer } from 'rxjs';
import { map, mergeMap, tap } from 'rxjs/operators';

/**
 * This security service is helping to retrieve jwt token, user logged info
 */
@Injectable({
    providedIn: 'root'
})
export class SecurityService {
    private authUser: AuthUser;
    private authToken: AuthToken;
    private gotClaims$: Observable<any>
    private calledClaims = false

    constructor(
        private session: SessionService,
        private roleClient: RolesClient) {
        let tempAuthToken = this.session.getUserToken()

        if (tempAuthToken) {
            this.authToken = new AuthToken(tempAuthToken.jwtToken, tempAuthToken.expireseIn, tempAuthToken.refreshToken, tempAuthToken.expireRefresh)
            this.authUser = this.authToken ? this.authToken.toAuthUser() : null
        }
    }

    getPortalClaims(): Observable<any> {
        if (!this.calledClaims) {
            this.gotClaims$ = this.roleClient.getPortalClaims()
                .pipe(
                    mergeMap((result: PortalClaimModel[]) => {
                        this.setPortalClaims(result)
                        return of(this.authUser.claims)
                    })
                )

            return this.gotClaims$
        }
        return of(this.authUser.claims)
    }

    setAuthUser(authToken: AuthToken) {
        this.authToken = authToken
        this.authUser = authToken.toAuthUser()

        // Store in session for keeping user can refresh the page
        this.session.setUserToken(JSON.stringify(authToken))
    }

    setPortalClaims(claims: any) {
        this.calledClaims = true
        this.authUser.claims = claims
    }

    getAuthUser() {
        return this.authUser
    }

    getJwtToken() {
        return this.authToken.jwtToken
    }

    isUserSignedIn() {
        return this.authUser ? true : false;
    }

    userLogout() {
        this.authUser = null
        this.authToken = null
    }
}