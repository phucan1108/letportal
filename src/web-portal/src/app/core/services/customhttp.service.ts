import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
 
import { ObjectUtils } from '../utils/object-util';

@Injectable({
    providedIn: 'root'
})
export class CustomHttpService {
    constructor(public httpClient: HttpClient) { }

    performHttp(url: string, method: string, body: string, successStatusCode: string, outputProjection: string): Observable<any> {
        const options_: any = {
            body,
            observe: 'response',
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                Accept: 'application/json'
            })
        };
        return this.httpClient.request(method, url, options_).pipe(
            mergeMap((res: any) => {
                return this.proceedHttpResponse(res, successStatusCode, outputProjection)
            })
        )
    }

    private proceedHttpResponse(response: HttpResponseBase, successStatusCode: string, outputProjection: string): Observable<any> {
        const status = response.status;
        const responseData =
            response instanceof HttpResponse ? response.body : undefined

        const successCodes = successStatusCode.replace(' ','').split(';')
        if (successCodes.indexOf(status.toString()) > -1) {
            return of(outputProjection ? this.proceedOutputProjection(outputProjection, responseData) : responseData)
        }
        else{
            return throwError(response.statusText)
        }
    }

    private proceedOutputProjection(outputProjection: string, data: any) {
        return ObjectUtils.projection(outputProjection, data)
    }
}

class FieldMap {
    key: string
    map: string
}