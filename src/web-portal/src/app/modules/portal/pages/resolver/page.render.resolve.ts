import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { PagesClient } from 'services/portal.service';

@Injectable()
export class PageRenderResolver implements Resolve<any> {

    constructor(private pagesClient: PagesClient){        
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('pageName'))
            return this.pagesClient.getOne(route.paramMap.get('pageName'));
    }
}
