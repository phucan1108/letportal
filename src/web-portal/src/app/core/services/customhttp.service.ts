import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponseBase, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import * as _ from 'lodash';

@Injectable({
    providedIn: 'root'
})
export class CustomHttpService {
    constructor(public httpClient: HttpClient) { }

    performHttp(url: string, method: string, body: string, successStatusCode: string, outputProjection: string): Observable<any> {
        let options_: any = {
            body: body,
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "application/json"
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
        const splitted = outputProjection.split(';')
        let fieldMaps: FieldMap[] = []
        _.forEach(splitted, field => {
            if (field.indexOf('=') > 0) {
                const fieldSplitted = field.split('=')
                fieldMaps.push({
                    key: fieldSplitted[0],
                    map: fieldSplitted[1]
                })
            }
            else {
                fieldMaps.push({
                    key: field,
                    map: field
                })
            }
        })
        if (data instanceof Array) {
            let resData = new Array()
            _.forEach(data, dt => {
                let obj = new Object()
                _.forEach(fieldMaps, map => {
                    const evaluted = Function('data', 'return data.' + map.map)
                    obj[map.key] = evaluted(dt)
                })

                resData.push(obj)
            })

            return resData
        }
        else {
            let obj = new Object()
            _.forEach(fieldMaps, map => {
                const evaluted = Function('data', 'return data.' + map.map)
                obj[map.key] = evaluted(data)
            })
            return obj
        }
    }
}

class FieldMap {
    key: string
    map: string
}