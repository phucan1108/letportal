import { Injectable } from '@angular/core';
import { TreeBoundSection } from 'app/core/context/bound-section';
import { Interceptor } from 'app/core/interceptors/interceptor';
import { InterceptorContext } from 'app/core/interceptors/interceptorcontext';
import { NGXLogger } from 'ngx-logger';

@Injectable()
export class AppMenuInterceptor implements Interceptor{
    pageName: string = 'app-menu-builder'

    constructor(private logger: NGXLogger){}
    executeControlEvent(section: string, control: string, event: string, context: InterceptorContext): boolean {
        if(section === 'appmenu' && control === 'availableUrl' && event === 'change'){
            // Set 'url' to chosen value
            const treeBoundSection: TreeBoundSection = context.sectionRef as TreeBoundSection
            const urlControl = treeBoundSection.getOpenedSection().get('url')
            urlControl.change(context.controlRef.value)
        }   
        return true
    }
    executeCommand(command: string, context: InterceptorContext): boolean {
        return true
    }
}