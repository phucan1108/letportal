import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { StandardComponentClient } from 'services/portal.service';

@Injectable()
export class StandardResolve implements Resolve<any> {

    constructor(private standard: StandardComponentClient){
    }

    resolve(
        route: ActivatedRouteSnapshot
    ) {
        if(route.paramMap.get('standardId'))
            return this.standard.getOne(route.paramMap.get('standardId'));
    }
}
