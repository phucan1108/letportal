import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { BackupsClient } from 'services/portal.service';

@Injectable()
export class BackupResolve implements Resolve<any> {

    constructor(private backupClient: BackupsClient,
        private router: Router){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('backupid')){
            return this.backupClient.getOne(route.paramMap.get('backupid')).pipe(
                catchError(err => {
                    this.router.navigateByUrl('/404')
                    return EMPTY
                })
            );
        }
    }
}
