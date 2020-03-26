
import { SessionService } from 'services/session.service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { SecurityService } from './security.service';

@Injectable()
export class AuthGuard implements CanActivate{
    constructor(
        private security: SecurityService,
        private router: Router){}

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
      ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
        if(this.security.isUserSignedIn()){
            return true
        }
        else{
            this.router.navigateByUrl('/')
        }
      }
}