import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { SessionService } from 'services/session.service';
 
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ShortcutUtil } from 'app/modules/shared/components/shortcuts/shortcut-util';

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
                if (err instanceof HttpErrorResponse) {
                    if(err.error instanceof Blob){
                        const reader = new FileReader();
                        reader.onload = event => {
                            // TODO: we will send an error back to BE to trace
                        };
                        reader.readAsText(err.error);
                    }
                }
                return throwError(err)
            })
        )
    }
}
