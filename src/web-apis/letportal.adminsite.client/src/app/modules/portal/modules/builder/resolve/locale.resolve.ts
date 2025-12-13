import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { PagesClient, LocalizationClient } from 'services/portal.service';

@Injectable()
export class LocaleResolve implements Resolve<any> {

    constructor(private localeClient: LocalizationClient){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        if(route.paramMap.get('localeId') && route.paramMap.get('appId'))
            return this.localeClient.getOne(route.paramMap.get('appId'), route.paramMap.get('localeId'))
    }
}
