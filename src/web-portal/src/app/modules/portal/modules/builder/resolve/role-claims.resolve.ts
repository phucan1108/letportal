import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AppsClient } from 'services/portal.service';
import { RolesClient } from 'services/identity.service';

@Injectable()
export class RoleClaimsResolve implements Resolve<any> {

    constructor(
        private roleClient: RolesClient){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        return this.roleClient.getPortalClaimsByRole(route.paramMap.get('roleName'))
    }
}
