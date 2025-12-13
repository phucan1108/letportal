import { Injectable } from '@angular/core';
import { DatabasesClient, DatasourceOptions, SharedDatabaseOptions, DatasourceStaticOptions, HttpServiceOptions, DatasourceControlType } from './portal.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NGXLogger } from 'ngx-logger';
import { Translator } from '../shell/translates/translate.pipe';
import { PageShellData } from '../models/page.model';
import { Observable, of } from 'rxjs';
import { tap, map } from 'rxjs/operators';
 

@Injectable({
    providedIn: 'root'
})
export class DatasourceOptionsService {
    constructor(
        private databaseClient: DatabasesClient,
        private httpClient: HttpClient,
        private translator: Translator,
        private logger: NGXLogger
    ) { }

    executeDatasourceOptions(datasourceOpts: DatasourceOptions, pageShellData: PageShellData): Observable<any> {
        switch (datasourceOpts.type) {
            case DatasourceControlType.StaticResource:
                const staticData = this.executeStatic(datasourceOpts.datasourceStaticOptions, pageShellData)
                return of(staticData)
            case DatasourceControlType.Database:
                return this.executeDatabase(datasourceOpts.databaseOptions, pageShellData)
            case DatasourceControlType.WebService:
                return this.executeHttpService(datasourceOpts.httpServiceOptions, pageShellData)
        }
    }

    outputProjection(outputProjection: string, data: any) {
        const splitted = outputProjection.split(';')
        const fieldMaps: FieldMap[] = []
        splitted?.forEach(field => {
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
            const resData = new Array()
            data?.forEach(dt => {
                const obj = new Object()
                fieldMaps?.forEach(map => {
                    const evaluted = Function('data', 'return data.' + map.map)
                    obj[map.key] = evaluted(dt)
                })

                resData.push(obj)
            })

            return resData
        }
        else {
            const obj = new Object()
            fieldMaps?.forEach(map => {
                const evaluted = Function('data', 'return data.' + map.map)
                obj[map.key] = evaluted(data)
            })
            return obj
        }
    }

    private executeDatabase(databaseConfigs: SharedDatabaseOptions, pageShellData: PageShellData) {
        const command = this.translator.translateDataWithShell(databaseConfigs.query, pageShellData)
        this.logger.debug('Prepared command to execute database', command)
        return this.databaseClient
            .executionDynamic(databaseConfigs.databaseConnectionId, command)
            .pipe(
                map(res => res.isSuccess ? res.result : null)
            )
    }

    private executeStatic(staticConfigs: DatasourceStaticOptions, pageShellData: PageShellData) {
        return staticConfigs.jsonResource ? JSON.parse(staticConfigs.jsonResource) : null
    }

    private executeHttpService(httpConfigs: HttpServiceOptions, pageShellData: PageShellData) {
        const url = this.translator.translateDataWithShell(httpConfigs.httpServiceUrl, pageShellData)
        const body = this.translator.translateDataWithShell(httpConfigs.jsonBody, pageShellData)
        switch (httpConfigs.httpMethod.toUpperCase()) {
            case 'GET':
                return this.httpClient.get(url, {
                    headers: new HttpHeaders({
                        'Content-Type': 'application/json'
                    }), observe: 'response'
                })
            case 'POST':
                return this.httpClient.post(url, body, {
                    headers: new HttpHeaders({
                        'Content-Type': 'application/json'
                    }), observe: 'response'
                }).pipe(
                    tap(
                        result => {
                            if (httpConfigs.httpSuccessCode.split(';').find(code => code === result.status.toString())) {

                            }
                        },
                        err => {
                        })
                )
            case 'PUT':
                return this.httpClient.put(url, body, {
                    headers: new HttpHeaders({
                        'Content-Type': 'application/json'
                    }), observe: 'response'
                }).pipe(
                    tap(
                        result => {
                            if (httpConfigs.httpSuccessCode.split(';').find(code => code === result.status.toString())) {

                            }
                        },
                        err => {
                        })
                )
            case 'DELETE':
                return this.httpClient.delete(url, {
                    headers: new HttpHeaders({
                        'Content-Type': 'application/json'
                    }), observe: 'response'
                }).pipe(
                    tap(
                        result => {
                            if (httpConfigs.httpSuccessCode.split(';').find(code => code === result.status.toString())) {

                            }
                        },
                        err => {
                        })
                )
        }
    }
}

class FieldMap {
    key: string
    map: string
}