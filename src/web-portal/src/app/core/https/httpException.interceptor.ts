import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { environment } from 'environments/environment';
import { SessionService } from 'services/session.service';
import * as _ from 'lodash';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ShortcutUtil } from 'app/shared/components/shortcuts/shortcut-util';

@Injectable()
export class HttpExceptionInterceptor implements HttpInterceptor {

    constructor(
        private session: SessionService,
        private router: Router,
        private shortUtil: ShortcutUtil
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((err) => {
                console.log(err)
                if (err instanceof HttpErrorResponse) {
                    // do error handling here
                }

                return throwError(err)
            })
        )
    }
}
