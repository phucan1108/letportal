import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { DynamicListClient } from 'services/portal.service';

@Injectable()
export class DynamicListBuilderResolve implements Resolve<any> {

    constructor(private dynamicClient: DynamicListClient,
        private router: Router){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('dynamicId')){
            return this.dynamicClient.getOne(route.paramMap.get('dynamicId')).pipe(
                catchError(err => {
                    this.router.navigateByUrl('/404')
                    return EMPTY
                })
            );
        }
    }
}
