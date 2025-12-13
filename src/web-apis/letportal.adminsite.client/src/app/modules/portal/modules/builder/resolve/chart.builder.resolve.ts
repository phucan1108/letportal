import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { ChartsClient } from 'services/portal.service';

@Injectable()
export class ChartBuilderResolve implements Resolve<any> {

    constructor(private chartsClient: ChartsClient,
        private router: Router){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('chartid')){
            return this.chartsClient.getOneForBuilder(route.paramMap.get('chartid')).pipe(
                catchError(err => {
                    this.router.navigateByUrl('/404')
                    return EMPTY
                })
            );
        }
    }
}
