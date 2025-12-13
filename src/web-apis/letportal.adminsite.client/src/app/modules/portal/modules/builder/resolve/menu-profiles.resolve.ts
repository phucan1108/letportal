import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AppsClient } from 'services/portal.service';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class MenuProfilesResolve implements Resolve<any> {

    constructor(
        private appClient: AppsClient,
        private translate: TranslateService){
    }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ) {
        return this.appClient.getOne(route.paramMap.get('appId'), this.translate.currentLang);
    }
}
