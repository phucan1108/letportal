import { Injectable, Optional, InjectionToken, Inject, Injector } from '@angular/core';
import { Interceptor } from './interceptor';

export const PAGE_INTERCEPTORS = new InjectionToken<Interceptor[]>('PAGE_INTERCEPTORS');
@Injectable()
export class InterceptorsProvider {

    constructor(private injector: Injector){}

    getPageInterceptor(pageName: string): Interceptor{
        const allInterceptors = this.injector.get(PAGE_INTERCEPTORS)
        return allInterceptors.find(a => a.pageName === pageName)
    }
}