import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { PagesClient } from 'services/portal.service';

@Injectable()
export class PageRenderResolve implements Resolve<any> {

    constructor(private pagesClient: PagesClient){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('pageName'))
            return this.pagesClient.getOneForRender(route.paramMap.get('pageName'));
    }
}
