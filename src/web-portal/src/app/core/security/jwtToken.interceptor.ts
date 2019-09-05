import { SecurityService } from './security.service';
import StringUtils from '../utils/string-util';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { environment } from 'environments/environment';
import { SessionService } from 'services/session.service';
import * as _ from 'lodash';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class JwtTokenInterceptor implements HttpInterceptor {

    constructor(private security: SecurityService, private session: SessionService, private router: Router
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let headers = req.headers
        
        let ignorePaths = environment.ignoreSendTokenEndpoints.split(';')
        let ignore = false
        _.forEach(ignorePaths, path => {
            if(req.url.indexOf(path) > 0){
                ignore = true
                return false
            }
        })

        if(!ignore){
            headers = headers.set('Authorization', 'Bearer ' + this.security.getJwtToken());
            const authReq = req.clone({ headers });
            return next.handle(authReq).pipe(
                catchError(err => {
                    if(err.status === 401){
                        this.session.clear()
                        this.security.userLogout()
                        this.router.navigateByUrl('/login')
                    }
                    return throwError(err)
                })
            )
        }
        else{
            return next.handle(req);
        }
    }
}
