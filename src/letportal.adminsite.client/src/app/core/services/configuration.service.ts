import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from 'environments/environment';
import { ConfigurationProvider } from '../configs/configProvider';

@Injectable({
    providedIn: 'root'
})
export class ConfigurationService {

    constructor(private http: HttpClient, private configurationProvider : ConfigurationProvider) { }

    fetchConfigs(): Promise<any> {
        return this.http.get(environment.configurationEndpoint)
            .toPromise().then((config: any)=> {
                // this.configurationProvider.setConfiguration(config)
            });
    }
}