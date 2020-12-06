import { Injectable, Optional, InjectionToken, Inject, Injector } from '@angular/core';
import { Interceptor } from './interceptor';
import { NGXLogger } from 'ngx-logger';

export const PAGE_INTERCEPTORS = new InjectionToken<Interceptor[]>('PAGE_INTERCEPTORS');
@Injectable()
export class InterceptorsProvider {

    constructor(
        private injector: Injector,
        private logger:NGXLogger){}

    getPageInterceptor(pageName: string): Interceptor{
        try{
            const allInterceptors = this.injector.get(PAGE_INTERCEPTORS)
            return allInterceptors.find(a => a.pageName === pageName)
        }
        catch(ex){
            this.logger.error(ex)
            return null
        }
        
    }
}