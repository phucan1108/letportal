
import { SessionService } from 'services/session.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class CanActivePortal implements CanActivate{
    constructor(private sessionService: SessionService){}

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
      ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
        // There are some rules need to be checked before allowing user to view
        // 1) User must select one app before going anywhere
        // 2) User must have a permission at least accessment in this form/list

        return this.sessionService.getCurrentApp() 
              || state.url.indexOf('portal/page/user-info') > 0 ? true : false;
      }
}