import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AppsClient } from 'services/portal.service';

@Injectable()
export class MenuProfilesRouterResolver implements Resolve<any> {

    constructor(private appClient: AppsClient){        
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        return this.appClient.getOne(route.paramMap.get('appId'));
    }
}
