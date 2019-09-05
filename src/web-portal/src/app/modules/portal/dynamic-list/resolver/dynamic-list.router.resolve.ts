import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { DynamicListClient } from 'services/portal.service';

@Injectable()
export class DynamicListRouterResolver implements Resolve<any> {

    constructor(private dynamicClient: DynamicListClient){        
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        return this.dynamicClient.getOne(route.paramMap.get('dynamicName'));
    }
}
