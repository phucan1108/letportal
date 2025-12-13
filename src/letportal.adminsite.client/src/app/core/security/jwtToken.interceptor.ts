import { SecurityService } from './security.service';
import StringUtils from '../utils/string-util';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { environment } from 'environments/environment';
import { SessionService } from 'services/session.service';
 
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { MatDialog } from '@angular/material/dialog';
import { UnlockScreenDialogComponent } from './components/unlock-screen.component';
import { RouterExtService } from '../ext-service/routerext.service';

@Injectable()
export class JwtTokenInterceptor implements HttpInterceptor {

    private isOpenningUnlock = false
    constructor(
        private routerEx: RouterExtService,
        private security: SecurityService,
        private session: SessionService,
        private router: Router,
        private dialog: MatDialog,
        private logger: NGXLogger
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let headers = req.headers

        const ignorePaths = environment.ignoreSendTokenEndpoints.split(';')
        let ignore = false
        ignorePaths?.forEach(path => {
            if (req.url.indexOf(path) > 0) {
                ignore = true
                return false
            }
        })

        if (!ignore) {
            headers = headers.set('Authorization', 'Bearer ' + this.security.getJwtToken());
            headers = headers.set('X-User-Session-Id', window.btoa(this.session.getUserSession()))
            const authReq = req.clone({ headers });
            return next.handle(authReq).pipe(
                catchError(err => {
                    const httpErrorResponse: HttpErrorResponse = err as HttpErrorResponse
                    if (httpErrorResponse.status === 401) {

                        this.logger.debug('Response error', httpErrorResponse.headers.getAll('X-Token-Expired'))
                        // Check actual token expire
                        if (httpErrorResponse.headers.has('X-Token-Expired')) {
                            // Remove current token and user session id
                            this.session.clearUserSession()
                            this.session.clearUserToken()
                            // Popup dialog for letting user types his password
                            if(!this.isOpenningUnlock){
                                const dialogRef = this.dialog.open(UnlockScreenDialogComponent, {
                                    disableClose: true
                                });
                                this.isOpenningUnlock = true
                                dialogRef.afterClosed().subscribe(res => {
                                    if (!!res) {
                                        this.router.navigateByUrl(this.routerEx.getCurrentUrl())
                                    }
                                })
                            }
                        }
                        else {
                            // There are some hacking cheats, force back to login page
                            this.session.clear()
                            this.security.userLogout()
                            this.router.navigateByUrl('/')
                        }
                    }
                    return throwError(err)
                })
            )
        }
        else {
            return next.handle(req);
        }
    }
}
