import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { BackupsClient, AppsClient } from 'services/portal.service';

@Injectable()
export class AppPackageResolve implements Resolve<any> {

    constructor(private appsClient: AppsClient,
        private router: Router){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('appId')){
            return this.appsClient.preview(route.paramMap.get('appId')).pipe(
                catchError(err => {
                    this.router.navigateByUrl('/404')
                    return EMPTY
                })
            );
        }
    }
}
