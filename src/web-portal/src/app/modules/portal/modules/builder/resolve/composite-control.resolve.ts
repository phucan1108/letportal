import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { CompositeControlsClient } from 'services/portal.service';

@Injectable()
export class CompositeControlResolve implements Resolve<any> {

    constructor(private compositeControlClient: CompositeControlsClient){
    }

    resolve(
        route: ActivatedRouteSnapshot
    ) {
        if(route.paramMap.get('id'))
            return this.compositeControlClient.get(route.paramMap.get('id'));
    }
}
